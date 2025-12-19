using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class StudentDisciplineConfiguration : IEntityTypeConfiguration<StudentDiscipline>
    {
        public void Configure(EntityTypeBuilder<StudentDiscipline> builder)
        {
            builder.ToTable("StudentDisciplines");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Penalty)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.StartDate)
                .IsRequired();

            builder.Property(d => d.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.IssuedBy)
                .IsRequired()
                .HasMaxLength(100);

            // Foreign key to Student
            builder.HasOne(d => d.Student)
                .WithMany(s => s.Disciplines)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
