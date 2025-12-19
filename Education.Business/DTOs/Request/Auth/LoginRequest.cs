using System.ComponentModel.DataAnnotations;

namespace Education.Business.DTOs.Request.Auth
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // ↔ MyUser.Email

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty; // ↔ MyUser.PasswordHash (hash ediləcək)
    }
}
