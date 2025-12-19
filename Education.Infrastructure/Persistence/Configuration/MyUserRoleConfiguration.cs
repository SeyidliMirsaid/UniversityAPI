using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class MyUserRoleConfiguration : IEntityTypeConfiguration<MyUserRole>
    {
        public void Configure(EntityTypeBuilder<MyUserRole> builder)
        {
            builder.ToTable("MyUserRoles");

            // Composite Primary Key
            builder.HasKey(ur => new { ur.MyUserId, ur.MyRoleId });

            // Foreign Keys
            builder.HasOne(ur => ur.MyUser)
                .WithMany(u => u.MyUserRoles)
                .HasForeignKey(ur => ur.MyUserId)
                .OnDelete(DeleteBehavior.Cascade); // User silinərsə UserRoles DƏ silinsin

            builder.HasOne(ur => ur.MyRole)
                .WithMany(r => r.MyUserRoles)
                .HasForeignKey(ur => ur.MyRoleId)
                .OnDelete(DeleteBehavior.Cascade); // Role silinərsə UserRoles DƏ silinsin

            // Index
            builder.HasIndex(ur => ur.MyUserId);
            builder.HasIndex(ur => ur.MyRoleId);
        }
    }
}
