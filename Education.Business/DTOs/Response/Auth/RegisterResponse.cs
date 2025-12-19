namespace Education.Business.DTOs.Request
{
    /// Registration uğurlu olduqdan sonra qaytarılan məlumatlar
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // "Student" or "Teacher"
        public DateTime CreatedAt { get; set; }

        // Student-specific (əgər Student-dırsa)
        public string? StudentNumber { get; set; }
        public string? Major { get; set; }

        // Teacher-specific (əgər Teacher-dırsa)
        public string? TeacherCode { get; set; }
        public string? Department { get; set; }
    }
}
