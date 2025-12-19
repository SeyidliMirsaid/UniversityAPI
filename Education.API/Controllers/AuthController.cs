using Education.Business.DTOs.Request.Auth;
using Education.Business.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Education.API.Controllers
{
    /// İstifadəçi autentifikasiya və autorizasiya əməliyyatları üçün controller.
    /// Bu controller user login, register, token refresh və logout funksionallığını təmin edir.
    
    /// Dependency Injection ilə IAuthService inject edirik.
    /// Bu, SOLID prinsiplərindən Dependency Inversion prinsipinə uyğundur.

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;


        /// İstifadəçi login endpoint-i.
        /// Email və password ilə autentifikasiya edir, JWT token qaytarır.
        /// POST /api/auth/login

        /// 200 OK: LoginResponse (token və user məlumatları)
        /// 401 Unauthorized: Email/şifrə yanlışdır
        /// 400 BadRequest: Validation xətası
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // AuthService-də login məntiqi icra olunur
            var result = await _authService.LoginAsync(request);

            // Niyə Ok(result)? - Uğurlu login olduqda 200 status kodu ilə token qaytarırıq
            return Ok(result);
        }

        /// <summary>
        /// Yeni istifadəçi qeydiyyatı endpoint-i.
        /// User, Student və ya Teacher yaradır.
        /// POST /api/auth/register
        /// </summary>
        /// <param name="request">RegisterRequest DTO - user məlumatlarını ehtiva edir</param>
        /// <returns>
        /// 200 OK: RegisterResponse (yaradılan user məlumatları)
        /// 400 BadRequest: Validation xətası (email artıq mövcud, şifrə zəif və s.)
        /// 409 Conflict: Unikal constraint pozulması
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // AuthService-də qeydiyyat məntiqi icra olunur
            var result = await _authService.RegisterAsync(request);

            // Niyə Ok(result)? - User uğurla yaradıldı, 200 status kodu ilə məlumat qaytarırıq
            return Ok(result);
        }

        /// <summary>
        /// Token yeniləmə endpoint-i.
        /// Köhnə refresh token ilə yeni access və refresh token alır.
        /// POST /api/auth/refresh-token
        /// </summary>
        /// <param name="request">TokenRequest DTO - refresh token ehtiva edir</param>
        /// <returns>
        /// 200 OK: TokenResponse (yeni access və refresh token)
        /// 401 Unauthorized: Refresh token yanlış və ya expired
        /// 400 BadRequest: Token təmin edilməyib
        /// </returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            // AuthService-də token refresh məntiqi icra olunur
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            // Niyə Ok(result)? - Token uğurla yeniləndi, yeni tokenlər qaytarırıq
            return Ok(result);
        }

        /// <summary>
        /// İstifadəçi logout endpoint-i.
        /// User-in bütün aktiv refresh token-lərini revoke edir.
        /// POST /api/auth/logout
        /// </summary>
        /// <returns>
        /// 200 OK: Uğurlu logout mesajı
        /// 401 Unauthorized: Token yoxdur və ya expired
        /// </returns>
        [HttpPost("logout")]
        [Authorize] // Niyə [Authorize]? - Yalnız login olmuş user-lər logout edə bilər
        public async Task<IActionResult> Logout()
        {
            // JWT token-dan user ID-ni çıxarırıq
            // ClaimTypes.NameIdentifier - JWT token-də olan user ID claim-i
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // AuthService-də logout məntiqi icra olunur
            await _authService.LogoutAsync(userId);

            // Niyə Ok(new { message = ... })? - Sadəcə uğurlu əməliyyat mesajı qaytarırıq
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Cari user profil məlumatlarını gətirən endpoint.
        /// GET /api/auth/profile
        /// </summary>
        /// <returns>
        /// 200 OK: User profil məlumatları
        /// 401 Unauthorized: Login olmayıb
        /// </returns>
        [HttpGet("profile")]
        [Authorize] // Niyə [Authorize]? - Yalnız login olmuş user-lər profilə baxa bilər
        public IActionResult GetProfile()
        {
            // JWT token-dan claim-ləri çıxarırıq
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst("fullName")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Niyə anonim object qaytarırıq? - Sadə profil məlumatları üçün DTO yaratmağa ehtiyac yoxdur
            return Ok(new
            {
                UserId = userId,
                Email = email,
                FullName = fullName,
                Roles = roles
            });
        }
    }

    /// <summary>
    /// TokenRequest - Refresh token üçün DTO (Data Transfer Object).
    /// Niyə ehtiyac var? Çünki:
    /// 1. Client yalnız refresh token göndərir
    /// 2. Clean Code - aydın və bir məqsədli class
    /// 3. Validation əlavə etmək asan olur
    /// 4. Swagger/OpenAPI dokumentasiyasında aydın görünür
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Refresh token dəyəri.
        /// [Required] - Validation üçün, boş ola bilməz
        /// string.Empty - Null reference exception qarşısını almaq üçün default dəyər
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
