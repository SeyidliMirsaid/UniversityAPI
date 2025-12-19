using Education.Business.DTOs.Request;
using Education.Business.DTOs.Request.Auth;

namespace Education.Business.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
    }
}
