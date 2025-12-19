using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.ToTable("StudentCourses");

            // Composite primary key
            builder.HasKey(sc => new { sc.StudentId, sc.CourseId });

            builder.Property(sc => sc.EnrollmentDate)
                .IsRequired();

            builder.Property(sc => sc.Grade)
                .HasPrecision(5, 2);

            builder.Property(sc => sc.GradeLetter)
                .HasMaxLength(2);

            builder.Property(sc => sc.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Enrolled");

            // Foreign key to Student
            builder.HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Course
            builder.HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
