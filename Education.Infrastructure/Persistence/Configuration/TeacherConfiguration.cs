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

            // One-to-one with MyUser
            builder.HasOne(t => t.MyUser)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
