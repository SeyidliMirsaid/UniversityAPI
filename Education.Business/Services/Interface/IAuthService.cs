using Education.Business.DTOs.Request;
using Education.Business.DTOs.Request.Auth;

namespace Education.Business.Services.Interface
{
    public interface IAuthService
    {
        // Login işləmi: Email və şifrə ilə autentifikasiya
        // ↗ LoginRequest -> Email və şifrə
        // ↘ LoginResponse -> Token və istifadəçi məlumatları
        // ↔ MyUser, Token entity'ləri ilə işləyir
        Task<LoginResponse> LoginAsync(LoginRequest request);

        // Register işləmi: Yeni istifadəçi qeydiyyatı
        // ↗ RegisterRequest -> İstifadəçi məlumatları
        // ↘ RegisterResponse -> Yaradılan istifadəçi məlumatları
        // ↔ MyUser, Student/Teacher, MyRole entity'ləri ilə işləyir
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        // Token yeniləmə: Köhnə refresh token ilə yeni access token almaq
        // ↗ refreshToken -> Köhnə refresh token dəyəri
        // ↘ TokenResponse -> Yeni token'lər
        // ↔ Token entity ilə işləyir
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);

        // Token ləğv etmə: Refresh token'i etibarsız etmək (logout)
        // ↗ refreshToken -> Ləğv ediləcək token
        // ↔ Token entity'sini update edir (IsRevoked = true)
        Task RevokeTokenAsync(string refreshToken);

        // Logout: İstifadəçinin bütün aktiv token'lərini ləğv etmək
        // ↗ userId -> Logout ediləcək istifadəçi ID
        // ↔ Token entity'lərini update edir
        Task LogoutAsync(int userId);
    }
}
