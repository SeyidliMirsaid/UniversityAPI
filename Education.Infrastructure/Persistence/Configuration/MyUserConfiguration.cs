using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class MyUserConfiguration : IEntityTypeConfiguration<MyUser>
    {
        public void Configure(EntityTypeBuilder<MyUser> builder)
        {
            // Table name
            builder.ToTable("MyUsers");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Relationships
            builder.HasOne(u => u.Student)
                .WithOne(s => s.MyUser)
                .HasForeignKey<Student>(s => s.MyUserId);

            builder.HasOne(u => u.Teacher)
                .WithOne(t => t.MyUser)
                .HasForeignKey<Teacher>(t => t.MyUserId);

            builder.HasMany(u => u.MyUserRoles)
                .WithOne(ur => ur.MyUser)
                .HasForeignKey(ur => ur.MyUserId);

            builder.HasMany(u => u.Tokens)
                .WithOne(t => t.MyUser)
                .HasForeignKey(t => t.MyUserId);
        }
    }
}
