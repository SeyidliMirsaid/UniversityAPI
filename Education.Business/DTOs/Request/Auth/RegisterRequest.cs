using System.ComponentModel.DataAnnotations;

namespace Education.Business.DTOs.Request.Auth
{
    public class RegisterRequest
    {
        // MyUser Entity fields:
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty; // ↔ MyUser.FirstName

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty; // ↔ MyUser.LastName

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;  // ↔ MyUser.Email

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty; // ↔ MyUser.Password

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty; // ↔ MyUser.PhoneNumber


        //  Role təyin etmək üçün:
        [Required]
        public string UserType { get; set; } = string.Empty;  // ↔ MyRole.Name ("Student" ya "Teacher")

        // Student Entity fields (əgər UserType = "Student"):
        public string? StudentNumber { get; set; }
        public string? Major { get; set; }

        // Teacher Entity fields (əgər UserType = "Teacher"):
        public string? TeacherCode { get; set; }
        public string? Department { get; set; }
    }
}
