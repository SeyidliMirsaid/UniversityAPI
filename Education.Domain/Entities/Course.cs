using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class Course : BaseEntity
    {
        // Əsas məlumatlar
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Semester { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public int MaxStudents { get; set; } = 30;
        public int MinStudents { get; set; } = 5;

        // Foreign Key
        public int TeacherId { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public virtual Teacher Teacher { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = [];


        // ✅ Sadə computed properties
        [NotMapped]
        public int EnrolledStudentsCount => StudentCourses?.Count(sc => sc.Status == "Enrolled") ?? 0;

        [NotMapped]
        public bool HasAvailableSeats => EnrolledStudentsCount < MaxStudents;

        [NotMapped]
        public bool CanStartCourse => EnrolledStudentsCount >= MinStudents;

        [NotMapped]
        public string TeacherName => Teacher?.FullName ?? string.Empty;
    }
}
