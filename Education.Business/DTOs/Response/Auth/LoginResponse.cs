namespace Education.Business.DTOs.Request
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        // ↔ MyUser.Id - JWT token yaratmaq üçün

        public string FullName { get; set; } = string.Empty;
        // ↔ MyUser.FirstName + LastName - İstifadəçi adı

        public string Email { get; set; } = string.Empty;
        // ↔ MyUser.Email - İstifadəçi identifikatoru

        public List<string> Roles { get; set; } = new();
        // ↔ MyUser.Roles - Authorization üçün rollar

        public string AccessToken { get; set; } = string.Empty;
        // ↔ Generated JWT - API çağırışları üçün

        public string RefreshToken { get; set; } = string.Empty;
        // ↔ Token.TokenValue - Access token yeniləmək üçün

        public DateTime AccessTokenExpiry { get; set; }
        // ↔ Token.ExpiryDate - Token'in bitmə vaxtı
    }
}
