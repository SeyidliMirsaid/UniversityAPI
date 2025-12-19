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

            // Composite Primary Key (StudentId + CourseId)
            builder.HasKey(sc => new { sc.StudentId, sc.CourseId });

            // Properties
            builder.Property(sc => sc.EnrollmentDate)
                .IsRequired();

            builder.Property(sc => sc.Grade)
                .HasPrecision(5, 2); // 100.00 kimi

            builder.Property(sc => sc.GradeLetter)
                .HasMaxLength(2); // A, B+, C-

            builder.Property(sc => sc.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Enrolled");

            // Foreign Keys ve Relationships

            // Student tərəfi - Cascade: Student silinərsə StudentCourse də silinsin
            builder.HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course tərəfi - Cascade: Course silinərsə StudentCourse də silinsin  
            builder.HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(sc => sc.Status);
            builder.HasIndex(sc => sc.Grade);
            builder.HasIndex(sc => sc.EnrollmentDate);
        }
    }
}
