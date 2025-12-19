using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
             //Table name
            builder.ToTable("Students");

            // Primary Key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.StudentNumber)
                .IsUnique();

            builder.Property(s => s.EnrollmentDate)
                .IsRequired();

            builder.Property(s => s.GPA)
                .HasPrecision(3, 2); // 3.50 kimi

            builder.Property(s => s.Major)
                .HasMaxLength(100);

            builder.Property(s => s.Faculty)
                .HasMaxLength(100);

            builder.Property(s => s.CurrentSemester);


            /*
              MyUser silinərsə → Student də silinir
              Student silinərsə → StudentCourses də silinir
              Cascade
            */
            // Foreign Key
            builder.HasOne(s => s.MyUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships
            builder.HasMany(s => s.StudentCourses)
                .WithOne(sc => sc.Student)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Disciplines)
                .WithOne(d => d.Student)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
