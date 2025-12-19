using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configuration
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TokenValue)
                .IsRequired();

            builder.Property(t => t.JwtId)
                .IsRequired();

            builder.Property(t => t.IsUsed)
                .HasDefaultValue(false);

            builder.Property(t => t.IsRevoked)
                .HasDefaultValue(false);

            builder.Property(t => t.AddedDate)
                .IsRequired();

            builder.Property(t => t.ExpiryDate)
                .IsRequired();

            builder.Property(t => t.TokenType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Refresh");

            // Foreign key to MyUser
            builder.HasOne(t => t.MyUser)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.MyUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
