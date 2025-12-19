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
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(t => t.TokenValue) //  Eyni token 2 dəfə ola bilməz
                .IsUnique(); 

            builder.Property(t => t.JwtId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.IsUsed)
                .HasDefaultValue(false);

            builder.Property(t => t.IsRevoked)
                .HasDefaultValue(false);

            builder.Property(t => t.AddedDate)
                .IsRequired();

            builder.Property(t => t.ExpiryDate)
                .IsRequired();

            builder.Property(t => t.TokenType)
                .HasMaxLength(50)
                .HasDefaultValue("Refresh");

            // Foreign Key
            builder.HasOne(t => t.MyUser)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.MyUserId)
                .OnDelete(DeleteBehavior.Cascade); // User silinərsə Token-lar DƏ silinsin

            // Indexes for performance  -- Sürətli axtarış üçün
            builder.HasIndex(t => t.MyUserId);
            builder.HasIndex(t => t.ExpiryDate);
            builder.HasIndex(t => new { t.IsUsed, t.IsRevoked });
        }
    }
}
