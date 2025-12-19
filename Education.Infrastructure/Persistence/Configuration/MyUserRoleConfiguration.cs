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

            // Composite primary key
            builder.HasKey(ur => new { ur.MyUserId, ur.MyRoleId });

            // Foreign key to MyUser
            builder.HasOne(ur => ur.MyUser)
                .WithMany(u => u.MyUserRoles)
                .HasForeignKey(ur => ur.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to MyRole
            builder.HasOne(ur => ur.MyRole)
                .WithMany(r => r.MyUserRoles)
                .HasForeignKey(ur => ur.MyRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
