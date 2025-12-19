namespace Education.Business.DTOs.Request
{
    public class LoginResponse
    {
        public int UserId { get; set; } // ↔ MyUser.Id    
        public string FullName { get; set; } = string.Empty; // ↔ MyUser.FirstName + LastName
        public string Email { get; set; } = string.Empty; // ↔ MyUser.Email
        public List<string> Roles { get; set; } = new(); // ↔ MyRole.Name (MyUser.MyUserRoles)
        public string AccessToken { get; set; } = string.Empty; // ↔ Generated JWT 
        public string RefreshToken { get; set; } = string.Empty; // ↔ Token.TokenValue  
        public DateTime AccessTokenExpiry { get; set; } // ↔ Token.ExpiryDate
    }
}
