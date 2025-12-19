using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class Teacher : BaseEntity
    {
        // TEACHER INFO
        public string TeacherCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        // ACADEMIC INFO
        public string? AcademicTitle { get; set; } // Professor, Associate Professor, Lecturer
        public string? Specialization { get; set; }
        public string? OfficeLocation { get; set; }
        public string? OfficeHours { get; set; }
        public string? ResearchInterests { get; set; }


        //  FOREIGN KEY
        public int MyUserId { get; set; }

        // NAVIGATION PROPERTIES
        public virtual MyUser? MyUser { get; set; }

        [JsonIgnore]
        public virtual ICollection<Course> Courses { get; set; } = [];


        // COMPUTED PROPERTIES
        [NotMapped]
        public string FullName => $"{MyUser?.FirstName} {MyUser?.LastName}";

        [NotMapped]
        public string Email => MyUser?.Email ?? string.Empty;

        [NotMapped]
        public string PhoneNumber => MyUser?.PhoneNumber ?? string.Empty;

        [NotMapped]
        public int ExperienceYears =>
            DateTime.Now.Year - HireDate.Year -
            (DateTime.Now.DayOfYear < HireDate.DayOfYear ? 1 : 0);

        [NotMapped]
        public int ActiveCoursesCount => Courses?.Count(c => c.IsActive) ?? 0;

        [NotMapped]
        public bool CanTeachMoreCourses => ActiveCoursesCount < 5; // Max 5 kurs

        [NotMapped]
        public string Status
        {
            get
            {
                if (ActiveCoursesCount > 0) return "Active";
                if (DateTime.Now.Year - HireDate.Year >= 30) return "Retired";
                return "Available";
            }
        }

    }
}
