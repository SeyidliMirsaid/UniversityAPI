namespace Education.Business.DTOs.Request
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        // ↔ Generated JWT - Yeni access token

        public string RefreshToken { get; set; } = string.Empty;
        // ↔ Token.TokenValue - Yeni refresh token

        public DateTime AccessTokenExpiry { get; set; }
        // ↔ Token.ExpiryDate - Token'in bitmə vaxtı
    }
}
