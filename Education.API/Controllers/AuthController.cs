using Education.Business.DTOs.Request;
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


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Xəta baş verdi", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Qeydiyyat zamanı xəta baş verdi", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize] // Yalnız authenticated user'lar
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _authService.LogoutAsync(userId);
                return Ok(new { message = "Uğurla çıxış edildi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Çıxış xətası", error = ex.Message });
            }
        }

        [HttpPost("revoke-token")]
        [Authorize] // Yalnız authenticated user'lar
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            try
            {
                await _authService.RevokeTokenAsync(request.RefreshToken);
                return Ok(new { message = "Token uğurla ləğv edildi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Token ləğv etmə xətası", error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Token yeniləmə xətası", error = ex.Message });
            }
        }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    // API/Models/RevokeTokenRequest.cs
    public class RevokeTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
