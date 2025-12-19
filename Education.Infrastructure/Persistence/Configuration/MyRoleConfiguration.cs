using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class MyRoleConfiguration : IEntityTypeConfiguration<MyRole>
    {
        public void Configure(EntityTypeBuilder<MyRole> builder)
        {
            builder.ToTable("MyRoles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.Property(r => r.Description)
                .HasMaxLength(200);
        }
    }
}
