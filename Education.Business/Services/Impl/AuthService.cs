using Education.Business.DTOs.Request;
using Education.Business.DTOs.Request.Auth;
using Education.Business.Services.Interface;
using Education.Domain.Entities;
using Education.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Education.Business.Services.Impl
{
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConfiguration _configuration = configuration;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // 1. İstifadəçini email ilə tap
            var user = await _unitOfWork.MyUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted) 
                ?? throw new UnauthorizedAccessException("Email və ya şifrə yanlışdır");

            // 2. Şifrəni yoxla
            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Email və ya şifrə yanlışdır");

            // 3. İstifadəçi aktiv deyilsə
            if (!user.IsActive)
                throw new UnauthorizedAccessException("İstifadəçi hesabı aktiv deyil");

            // 4. Rolları al (MyUserRoles və MyRole ilə)
            var userRoles = await _unitOfWork.MyUserRoles
                .FindAsync(ur => ur.MyUserId == user.Id && !ur.IsDeleted);

            if (!userRoles.Any())
                throw new UnauthorizedAccessException("İstifadəçinin rolu yoxdur");

            var roleIds = userRoles.Select(ur => ur.MyRoleId).Distinct().ToList();
            var roles = await _unitOfWork.MyRoles
                .FindAsync(r => roleIds.Contains(r.Id) && !r.IsDeleted);

            var roleNames = roles.Select(r => r.Name).Distinct().ToList();

            // 5. JWT token yarat
            var accessToken = GenerateJwtToken(user, roleNames);

            // 6. Refresh token yarat
            var refreshToken = GenerateRefreshToken();

            // 7. Refresh token'i user'a təyin et
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));
            user.LastLoginDate = DateTime.UtcNow;

            _unitOfWork.MyUsers.Update(user);

            // 8. Token'i database'də saxla
            var tokenEntity = new Token
            {
                MyUserId = user.Id,
                TokenValue = refreshToken,
                JwtId = Guid.NewGuid().ToString(),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = user.RefreshTokenExpiryTime.Value,
                TokenType = "Refresh"
            };

            await _unitOfWork.Tokens.AddAsync(tokenEntity);

            // 9. Bütün dəyişiklikləri save et
            await _unitOfWork.SaveChangesAsync();

            // 10. Response yarat
            return new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roleNames,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60"))
            };
        }

        public async Task LogoutAsync(int userId)
        {
            // 1. İstifadəçini tap
            var user = await _unitOfWork.MyUsers
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted) ??
                throw new ArgumentException("İstifadəçi tapılmadı");

            // 2. İstifadəçinin bütün aktiv refresh token'lərini tap
            var activeTokens = await _unitOfWork.Tokens
                .FindAsync(t =>
                    t.MyUserId == userId &&
                    t.TokenType == "Refresh" &&
                    !t.IsUsed &&
                    !t.IsRevoked &&
                    t.ExpiryDate > DateTime.UtcNow &&
                    !t.IsDeleted);

            // 3. Bütün aktiv token'ləri revoke et
            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.IsUsed = true;
                _unitOfWork.Tokens.Update(token);
            }

            // 4. İstifadəçinin refresh token'ini null et
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _unitOfWork.MyUsers.Update(user);

            // 5. Dəyişiklikləri save et
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("Refresh token tələb olunur");

            // 1. Refresh token'i database'də tap
            var tokenEntity = await _unitOfWork.Tokens
                .FirstOrDefaultAsync(t => t.TokenValue == refreshToken &&
                    t.TokenType == "Refresh" && !t.IsDeleted)
                ?? throw new UnauthorizedAccessException("Etibarsız refresh token");

            // 2. Token'in valid olub-olmadığını yoxla
            if (tokenEntity.IsUsed || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Token istifadə edilib və ya müddəti bitib");

            // 3. İstifadəçini tap
            var user = await _unitOfWork.MyUsers
                .FirstOrDefaultAsync(u => u.Id == tokenEntity.MyUserId && !u.IsDeleted)
            ??    throw new UnauthorizedAccessException("İstifadəçi tapılmadı");

            // 4. User'ın refresh token'i yoxla (security üçün)
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Etibarsız refresh token");

            // 5. Köhnə token'i etibarsız et
            tokenEntity.IsUsed = true;
            tokenEntity.IsRevoked = true;
            _unitOfWork.Tokens.Update(tokenEntity);

            // 6. User'ın rollarını al
            var userRoles = await _unitOfWork.MyUserRoles
                .FindAsync(ur => ur.MyUserId == user.Id && !ur.IsDeleted);

            var roleIds = userRoles.Select(ur => ur.MyRoleId).Distinct().ToList();
            var roles = await _unitOfWork.MyRoles
                .FindAsync(r => roleIds.Contains(r.Id) && !r.IsDeleted);

            var roleNames = roles.Select(r => r.Name).Distinct().ToList();

            // 7. Yeni access token yarat
            var newAccessToken = GenerateJwtToken(user, roleNames);

            // 8. Yeni refresh token yarat
            var newRefreshToken = GenerateRefreshToken();

            // 9. User'ı yeni refresh token ilə update et
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));

            _unitOfWork.MyUsers.Update(user);

            // 10. Yeni token'i database'də saxla
            var newTokenEntity = new Token
            {
                MyUserId = user.Id,
                TokenValue = newRefreshToken,
                JwtId = Guid.NewGuid().ToString(),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = user.RefreshTokenExpiryTime.Value,
                TokenType = "Refresh"
            };

            await _unitOfWork.Tokens.AddAsync(newTokenEntity);

            // 11. Bütün dəyişiklikləri save et
            await _unitOfWork.SaveChangesAsync();

            // 12. Yeni token'ləri qaytar
            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60"))
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Email artıq mövcuddurmu yoxla
            var existingUser = await _unitOfWork.MyUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted) ??
            throw new ArgumentException("Bu email artıq qeydiyyatdan keçib");
                

            // 2. Role-u yoxla (Student və ya Teacher olmalıdır)
            if (request.UserType != "Student" && request.UserType != "Teacher")
                throw new ArgumentException("İstifadəçi tipi 'Student' və ya 'Teacher' olmalıdır");

            // 3. Şifrəni hash et
            var (passwordHash, passwordSalt) = HashPassword(request.Password);

            // 4. Yeni MyUser yarat
            var newUser = new MyUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                EmailConfirmed = false, // İlkin olaraq təsdiqlənməyib
                PhoneConfirmed = false
            };

            await _unitOfWork.MyUsers.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync(); // User ID almaq üçün

            // 5. Role-u tap və ya yarat
            var role = await _unitOfWork.MyRoles.FirstOrDefaultAsync(r => r.Name == request.UserType && !r.IsDeleted);

            if (role == null)
            {
                // Role yoxdursa yarat
                role = new MyRole
                {
                    Name = request.UserType,
                    Description = $"{request.UserType} rolu"
                };
                await _unitOfWork.MyRoles.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
            }

            // 6. User'a role təyin et
            var userRole = new MyUserRole
            {
                MyUserId = newUser.Id,
                MyRoleId = role.Id
            };
            await _unitOfWork.MyUserRoles.AddAsync(userRole);

            // 7. Student və ya Teacher profili yarat
            if (request.UserType == "Student")
            {
                if (string.IsNullOrEmpty(request.StudentNumber))
                    throw new ArgumentException("Student nömrəsi tələb olunur");

                // Student nömrəsi unikallığını yoxla
                var existingStudent = await _unitOfWork.Students
                    .FirstOrDefaultAsync(s => s.StudentNumber == request.StudentNumber && !s.IsDeleted)
                  ??  throw new ArgumentException("Bu student nömrəsi artıq mövcuddur");

                var student = new Student
                {
                    MyUserId = newUser.Id,
                    StudentNumber = request.StudentNumber,
                    Major = request.Major,
                    EnrollmentDate = DateTime.UtcNow
                };
                await _unitOfWork.Students.AddAsync(student);
            }
            else if (request.UserType == "Teacher")
            {
                if (string.IsNullOrEmpty(request.TeacherCode))
                    throw new ArgumentException("Teacher kodu tələb olunur");

                // Teacher kodunun unikallığını yoxla
                var existingTeacher = await _unitOfWork.Teachers
                    .FirstOrDefaultAsync(t => t.TeacherCode == request.TeacherCode && !t.IsDeleted)
                    ?? throw new ArgumentException("Bu teacher kodu artıq mövcuddur");

                var teacher = new Teacher
                {
                    MyUserId = newUser.Id,
                    TeacherCode = request.TeacherCode,
                    Department = request.Department ?? "Ümumi",
                    HireDate = DateTime.UtcNow
                };
                await _unitOfWork.Teachers.AddAsync(teacher);
            }

            // 8. Bütün dəyişiklikləri save et
            await _unitOfWork.SaveChangesAsync();

            // 9. Response yarat
            return new RegisterResponse
            {
                UserId = newUser.Id,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                UserType = request.UserType,
                CreatedAt = DateTime.UtcNow,
                StudentNumber = request.UserType == "Student" ? request.StudentNumber : null,
                Major = request.UserType == "Student" ? request.Major : null,
                TeacherCode = request.UserType == "Teacher" ? request.TeacherCode : null,
                Department = request.UserType == "Teacher" ? request.Department : null
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("Refresh token tələb olunur");

            // 1. Token'i tap
            var tokenEntity = await _unitOfWork.Tokens
                .FirstOrDefaultAsync(t =>
                    t.TokenValue == refreshToken &&
                    t.TokenType == "Refresh" &&
                    !t.IsDeleted) ??
                throw new ArgumentException("Token tapılmadı");

            // 2. Token'i revoke et
            tokenEntity.IsRevoked = true;
            tokenEntity.IsUsed = true;
            _unitOfWork.Tokens.Update(tokenEntity);

            // 3. Əgər bu token istifadəçinin cari token'idirsə, null et
            var user = await _unitOfWork.MyUsers
                .FirstOrDefaultAsync(u => u.Id == tokenEntity.MyUserId && !u.IsDeleted);

            if (user != null && user.RefreshToken == refreshToken)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                _unitOfWork.MyUsers.Update(user);
            }

            // 4. Dəyişiklikləri save et
            await _unitOfWork.SaveChangesAsync();
        }

        // PRIVATE METHODS
        private string GenerateJwtToken(MyUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("fullName", user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not found")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private (byte[] hash, byte[] salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        /*
        private int? GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                return userId;
            }
            catch
            {
                return null;
            }
        } */
    }
}
