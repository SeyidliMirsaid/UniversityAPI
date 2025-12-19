using System.ComponentModel.DataAnnotations;

namespace Education.Business.DTOs.Request.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
        public string Email { get; set; } = string.Empty;
        // Email: MyUser entity-də Email property ilə eşleşir

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır")]
        public string Password { get; set; } = string.Empty;
        // Password: MyUser.PasswordHash ilə yoxlanacaq (hash edilmiş)
    }
}
