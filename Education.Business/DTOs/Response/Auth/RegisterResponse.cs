namespace Education.Business.DTOs.Request
{
    /// Registration uğurlu olduqdan sonra qaytarılan məlumatlar
    public class RegisterResponse
    {
        public int UserId { get; set; }
        // ↔ MyUser.Id - Yeni yaradılan istifadəçi ID

        public string FirstName { get; set; } = string.Empty;
        // ↔ MyUser.FirstName

        public string LastName { get; set; } = string.Empty;
        // ↔ MyUser.LastName

        public string FullName => $"{FirstName} {LastName}";
        // Computed property - Tam ad

        public string Email { get; set; } = string.Empty;
        // ↔ MyUser.Email

        public string PhoneNumber { get; set; } = string.Empty;
        // ↔ MyUser.PhoneNumber

        public string UserType { get; set; } = string.Empty;
        // ↔ MyUser.Roles - "Student" ya "Teacher"

        public DateTime CreatedAt { get; set; }
        // ↔ MyUser.CreatedAt - Qeydiyyat tarixi

        // Student üçün əlavə
        public string? StudentNumber { get; set; }
        // ↔ Student.StudentNumber

        public string? Major { get; set; }
        // ↔ Student.Major

        // Teacher üçün əlavə
        public string? TeacherCode { get; set; }
        // ↔ Teacher.TeacherCode

        public string? Department { get; set; }
        // ↔ Teacher.Department
    }
}
