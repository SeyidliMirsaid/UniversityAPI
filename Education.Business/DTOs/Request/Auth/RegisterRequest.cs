using System.ComponentModel.DataAnnotations;

namespace Education.Business.DTOs.Request.Auth
{
    public class RegisterRequest
    {
        // MyUser entity fields
        [Required(ErrorMessage = "Ad tələb olunur")]
        [MaxLength(50, ErrorMessage = "Ad maksimum 50 simvol olmalıdır")]
        public string FirstName { get; set; } = string.Empty;
        // ↔ MyUser.FirstName

        [Required(ErrorMessage = "Soyad tələb olunur")]
        [MaxLength(50, ErrorMessage = "Soyad maksimum 50 simvol olmalıdır")]
        public string LastName { get; set; } = string.Empty;
        // ↔ MyUser.LastName

        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
        public string Email { get; set; } = string.Empty;
        // ↔ MyUser.Email

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır")]
        public string Password { get; set; } = string.Empty;
        // ↔ MyUser.PasswordHash (hash ediləcək)

        [Required(ErrorMessage = "Telefon nömrəsi tələb olunur")]
        [Phone(ErrorMessage = "Düzgün telefon formatı daxil edin")]
        public string PhoneNumber { get; set; } = string.Empty;
        // ↔ MyUser.PhoneNumber

        // Role seçimi
        [Required(ErrorMessage = "İstifadəçi tipi seçilməlidir")]
        public string UserType { get; set; } = string.Empty;
        // ↔ MyRole.Name ("Student" ya "Teacher")

        // Student üçün əlavə field'lər
        public string? StudentNumber { get; set; }
        // ↔ Student.StudentNumber (əgər UserType = "Student")

        public string? Major { get; set; }
        // ↔ Student.Major

        // Teacher üçün əlavə field'lər
        public string? TeacherCode { get; set; }
        // ↔ Teacher.TeacherCode (əgər UserType = "Teacher")

        public string? Department { get; set; }
        // ↔ Teacher.Department
    }
}
