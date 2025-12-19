namespace Education.Business.DTOs.Request
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty; // ↔ Generated JWT 
        public string RefreshToken { get; set; } = string.Empty; // ↔ Token.TokenValue
        public DateTime AccessTokenExpiry { get; set; } // ↔ Token.ExpiryDate
    }
}
