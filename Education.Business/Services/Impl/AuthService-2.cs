//using Education.Business.DTOs.Request;
//using Education.Business.DTOs.Request.Auth;
//using Education.Business.Services.Interface;
//using Education.Domain.Entities;
//using Education.Infrastructure.Persistence.Repositories.Interfaces;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Cryptography;

//namespace Education.Business.Services.Impl
//{
//    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
//    {
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly IConfiguration _configuration = configuration;
//        public async Task<LoginResponse> LoginAsync(LoginRequest request)
//        {
//            // 1. User tap
//            var user = await _unitOfWork.MyUsers
//                .FirstOrDefaultAsync(u => u.Email == request.Email) ?? throw new Exception("Email or password is incorrect");

//            // 2. Password yoxla
//            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
//                throw new Exception("Email or password is incorrect");

//            // 3. Email confirmed?
//            if (!user.EmailConfirmed)
//                throw new Exception("Email not confirmed. Please confirm your email.");

//            // 4. User rollarını tap
//            var roles = await GetUserRolesAsync(user.Id);

//            // 5. JWT token yarat (ROLLAR ilə)
//            var accessToken = GenerateJwtToken(user, roles);
//            var refreshToken = GenerateRefreshToken();

//            // 6. Update login info
//            user.LastLoginDate = DateTime.UtcNow;
//            _unitOfWork.MyUsers.Update(user);

//            // 7. Save refresh token
//            var tokenEntity = new Token
//            {
//                MyUserId = user.Id,
//                TokenValue = refreshToken,
//                JwtId = Guid.NewGuid().ToString(),
//                ExpiryDate = DateTime.UtcNow.AddDays(7),
//                TokenType = "Refresh"
//            };

//            await _unitOfWork.Tokens.AddAsync(tokenEntity);
//            await _unitOfWork.SaveChangesAsync();

//            // 8. Return response
//            return new LoginResponse
//            {
//                UserId = user.Id,
//                FullName = $"{user.FirstName} {user.LastName}",
//                Email = user.Email,
//                Roles = roles,
//                AccessToken = accessToken,
//                RefreshToken = refreshToken,
//                AccessTokenExpiry = DateTime.UtcNow.AddHours(2)
//            };
//        }

//        public Task LogoutAsync(int userId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<TokenResponse> RefreshTokenAsync(string refreshToken)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
//        {
//            // 1. Validation
//            await ValidateRegistrationRequest(request);

//            // 2. Password hash yarat
//            var (passwordHash, passwordSalt) = CreatePasswordHash(request.Password);

//            // 3. MyUser entity yarat
//            var user = new MyUser
//            {
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                Email = request.Email,
//                PhoneNumber = request.PhoneNumber,
//                PasswordHash = passwordHash,
//                PasswordSalt = passwordSalt,
//                EmailConfirmed = false, // İlk başda false, email confirmation lazım
//                CreatedAt = DateTime.UtcNow
//            };

//            // 4. Role tap
//            var role = await GetRoleByNameAsync(request.UserType);
//            if (role == null)
//                throw new Exception($"Role '{request.UserType}' not found");

//            // 5. User-i save et
//            await _unitOfWork.MyUsers.AddAsync(user);
//            await _unitOfWork.SaveChangesAsync(); // ID almaq üçün

//            // 6. UserRole əlavə et
//            var userRole = new MyUserRole
//            {
//                MyUserId = user.Id,
//                MyRoleId = role.Id
//            };

//            await _unitOfWork.MyUserRoles.AddAsync(userRole);

//            // 7. Student/Teacher profile yarat (əgər lazımdırsa)
//            if (request.UserType == "Student")
//            {
//                await CreateStudentProfileAsync(user.Id, request);
//            }
//            else if (request.UserType == "Teacher")
//            {
//                await CreateTeacherProfileAsync(user.Id, request);
//            }

//            // 8. Save changes
//            await _unitOfWork.SaveChangesAsync();

//            // 9. Response qaytar
//            return new RegisterResponse
//            {
//                UserId = user.Id,
//                FirstName = user.FirstName,
//                LastName = user.LastName,
//                Email = user.Email,
//                PhoneNumber = user.PhoneNumber,
//                UserType = request.UserType,
//                CreatedAt = (DateTime)user.CreatedAt,
//                StudentNumber = request.StudentNumber,
//                Major = request.Major,
//                TeacherCode = request.TeacherCode,
//                Department = request.Department
//            };
//        }

//        public Task RevokeTokenAsync(string refreshToken)
//        {
//            throw new NotImplementedException();
//        }

//        // ========== HELPER METHODS ==========

//        /// Password hash yaradır
//        private (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
//        {
//            using var hmac = new HMACSHA512();
//            var passwordSalt = hmac.Key;
//            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
//            return (passwordHash, passwordSalt);
//        }

//        /// Password hash-i yoxlayır
//        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
//        {
//            if (storedHash.Length != 32 || storedSalt.Length != 64)
//                return false;

//            using var hmac = new HMACSHA512(storedSalt);
//            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
//            return computedHash.SequenceEqual(storedHash);
//        }

//        /// JWT token yaradır
//        private string GenerateJwtToken(MyUser user, List<string> roles)
//        {
//            // 1. Claims yarat
//            var claims = new List<System.Security.Claims.Claim>
//        {
//            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
//            new(System.Security.Claims.ClaimTypes.Email, user.Email),
//            new(System.Security.Claims.ClaimTypes.GivenName, user.FirstName),
//            new(System.Security.Claims.ClaimTypes.Surname, user.LastName),
//            new("fullName", $"{user.FirstName} {user.LastName}")
//        };

//            // 2. Rolları əlavə et
//            foreach (var role in roles)
//            {
//                claims.Add(new(System.Security.Claims.ClaimTypes.Role, role));
//            }

//            // 3. Secret key
//            var key = new SymmetricSecurityKey(
//                System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default_secret_key_here"));

//            // 4. Credentials
//            var creds = new SigningCredentials(
//                key, SecurityAlgorithms.HmacSha256);

//            // 5. Token descriptor
//            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
//            {
//                Subject = new System.Security.Claims.ClaimsIdentity(claims),
//                Expires = DateTime.UtcNow.AddHours(2),
//                SigningCredentials = creds,
//                Issuer = _configuration["Jwt:Issuer"],
//                Audience = _configuration["Jwt:Audience"]
//            };

//            // 6. Token yarat
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }

//        /// Refresh token yaradır
//        private string GenerateRefreshToken()
//        {
//            var randomNumber = new byte[32];
//            using var rng = RandomNumberGenerator.Create();
//            rng.GetBytes(randomNumber);
//            return Convert.ToBase64String(randomNumber);
//        }

//        /// User-in rollarını gətirir
//        private async Task<List<string>> GetUserRolesAsync(int userId)
//        {
//            var userRoles = await _unitOfWork.MyUserRoles.FindAsync(ur => ur.MyUserId == userId);

//            var roleIds = userRoles.Select(ur => ur.MyRoleId).ToList();

//            var roles = await _unitOfWork.MyRoles.FindAsync(r => roleIds.Contains(r.Id));

//            return roles.Select(r => r.Name).ToList();
//        }

//        // ========== VALIDATION HELPERS ==========

//        /// Email unikal olduğunu yoxlayır
//        private async Task<bool> IsEmailUniqueAsync(string email)
//        {
//            return !await _unitOfWork.MyUsers.ExistsAsync(u => u.Email == email);
//        }

//        /// Student number unikal olduğunu yoxlayır
//        private async Task<bool> IsStudentNumberUniqueAsync(string studentNumber)
//        {
//            return !await _unitOfWork.Students.ExistsAsync(s => s.StudentNumber == studentNumber);
//        }

//        /// Teacher code unikal olduğunu yoxlayır
//        private async Task<bool> IsTeacherCodeUniqueAsync(string teacherCode)
//        {
//            return !await _unitOfWork.Teachers.ExistsAsync(t => t.TeacherCode == teacherCode);
//        }

//        /// Role adına görə Role entity tapır
//        private async Task<MyRole?> GetRoleByNameAsync(string roleName)
//        {
//            return await _unitOfWork.MyRoles.FirstOrDefaultAsync(r => r.Name == roleName);
//        }

//        /// Register request-i validate edir
//        /// Niyə: Qəbul edilən məlumatların düzgünlüyünü yoxlamaq
//        /// Nə edir: Hər bir validation rule yoxlayır, səhv olarsa exception atır
//        /// Validation keçməsə exception
//        private async Task ValidateRegistrationRequest(RegisterRequest request)
//        {
//            // 1. TƏLƏB OLUNAN FIELD-LAR
//            // Niyə: Boş data qəbul etməmək üçün
//            if (string.IsNullOrEmpty(request.FirstName))
//                throw new Exception("First name is required");

//            if (string.IsNullOrEmpty(request.Email))
//                throw new Exception("Email is required");

//            if (string.IsNullOrEmpty(request.Password))
//                throw new Exception("Password is required");

//            if (string.IsNullOrEmpty(request.UserType))
//                throw new Exception("User type is required");

//            // 2. EMAIL FORMAT VALIDASIYA
//            // Niyə: "test@mail.com" formatında olmalıdır
//            // Nə edir: Email-in düzgün formatda olub-olmadığını yoxlayır
//            //if (!IsValidEmail(request.Email))
//                //throw new Exception("Invalid email format");

//            // 3. EMAIL UNIKALLIQ
//            // Niyə: Eyni email ilə 2 user ola bilməz
//            // Nə edir: Database-də bu email-in artıq mövcud olub-olmadığını yoxlayır
//            if (!await IsEmailUniqueAsync(request.Email))
//                throw new Exception("Email already registered");

//            // 4. PASSWORD GÜCÜ
//            // Niyə: Zəif password-ları qəbul etməmək
//            // Nə edir: Password-un minimum uzunluğunu yoxlayır
//            if (request.Password.Length < 6)
//                throw new Exception("Password must be at least 6 characters");

//            // 5. USER TYPE VALIDASIYA
//            // Niyə: Yalnız "Student" və ya "Teacher" qəbul etmək
//            // Nə edir: UserType-in düzgün dəyər olduğunu yoxlayır
//            if (request.UserType != "Student" && request.UserType != "Teacher")
//                throw new Exception("User type must be 'Student' or 'Teacher'");

//            // 6. STUDENT-XÜSUSİ VALIDASIYA
//            // Niyə: Student üçün xüsusi field-lar tələb olunur
//            // Nə edir: StudentNumber-un təmin edildiyini və unikal olduğunu yoxlayır
//            if (request.UserType == "Student")
//            {
//                if (string.IsNullOrEmpty(request.StudentNumber))
//                    throw new Exception("Student number is required for students");

//                if (!await IsStudentNumberUniqueAsync(request.StudentNumber))
//                    throw new Exception("Student number already exists");
//            }

//            // 7. TEACHER-XÜSUSİ VALIDASIYA
//            // Niyə: Teacher üçün xüsusi field-lar tələb olunur
//            // Nə edir: TeacherCode və Department-un təmin edildiyini yoxlayır
//            if (request.UserType == "Teacher")
//            {
//                if (string.IsNullOrEmpty(request.TeacherCode))
//                    throw new Exception("Teacher code is required for teachers");

//                if (!await IsTeacherCodeUniqueAsync(request.TeacherCode))
//                    throw new Exception("Teacher code already exists");

//                if (string.IsNullOrEmpty(request.Department))
//                    throw new Exception("Department is required for teachers");
//            }
//        }


//        private async Task CreateStudentProfileAsync(int userId, RegisterRequest request)
//        {
//            var student = new Student
//            {
//                MyUserId = userId,
//                StudentNumber = request.StudentNumber!,
//                EnrollmentDate = DateTime.UtcNow,
//                Major = request.Major,
//                //Status = "Active"
//            };

//            await _unitOfWork.Students.AddAsync(student);
//        }

//        private async Task CreateTeacherProfileAsync(int userId, RegisterRequest request)
//        {
//            var teacher = new Teacher
//            {
//                MyUserId = userId,
//                TeacherCode = request.TeacherCode!,
//                Department = request.Department!,
//                HireDate = DateTime.UtcNow,
//                //Status = "Active"
//            };

//            await _unitOfWork.Teachers.AddAsync(teacher);
//        }

//    }
//}
