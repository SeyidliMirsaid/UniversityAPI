using System.ComponentModel.DataAnnotations.Schema;

namespace Education.Domain.Entities
{
    public class StudentCourse : BaseEntity
    {
        // Foreign Keys
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Akademik məlumatlar
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public decimal? Grade { get; set; }
        public string? GradeLetter { get; set; }
        public string Status { get; set; } = "Enrolled"; // Enrolled, Completed, Dropped, Failed

        // Navigation Properties
        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;

        // ✅ Sadə computed properties
        [NotMapped]
        public bool IsPassed => Grade >= 50;

        [NotMapped]
        public bool IsCompleted => Status == "Completed";
    }
}
