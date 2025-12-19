using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.StudentNumber)
                .IsUnique();

            builder.Property(s => s.EnrollmentDate)
                .IsRequired();

            builder.Property(s => s.GPA)
                .HasPrecision(3, 2);

            builder.Property(s => s.Major)
                .HasMaxLength(100);

            builder.Property(s => s.Faculty)
                .HasMaxLength(100);

            // One-to-one with MyUser
            builder.HasOne(s => s.MyUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
