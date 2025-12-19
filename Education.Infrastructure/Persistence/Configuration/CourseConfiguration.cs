using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CourseCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.CourseCode)
                .IsUnique();

            builder.Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.Credits)
                .IsRequired();

            builder.Property(c => c.Semester);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.MaxStudents)
                .HasDefaultValue(30);

            builder.Property(c => c.MinStudents)
                .HasDefaultValue(5);

            // Foreign Key - Restrict: Course silinərsə Teacher QALSIN
            builder.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationships - Cascade: Course silinərsə StudentCourses DƏ silinsin
            builder.HasMany(c => c.StudentCourses)
                .WithOne(sc => sc.Course)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
