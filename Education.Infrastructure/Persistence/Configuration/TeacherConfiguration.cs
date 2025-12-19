using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TeacherCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(t => t.TeacherCode)
                .IsUnique();

            builder.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.HireDate)
                .IsRequired();

            builder.Property(t => t.AcademicTitle)
                .HasMaxLength(50);

            builder.Property(t => t.Specialization)
                .HasMaxLength(150);

            builder.Property(t => t.OfficeLocation)
                .HasMaxLength(100);

            builder.Property(t => t.OfficeHours)
                .HasMaxLength(500);

            builder.Property(t => t.ResearchInterests)
                .HasMaxLength(500);

            // Foreign Key - Cascade: MyUser silinərsə Teacher DƏ silinsin
            builder.HasOne(t => t.MyUser)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships - Restrict: Teacher silinərsə Courses QALSIN
            builder.HasMany(t => t.Courses)
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
