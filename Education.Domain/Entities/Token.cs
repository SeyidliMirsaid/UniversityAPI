using System.ComponentModel.DataAnnotations.Schema;

namespace Education.Domain.Entities
{
    public class Token : BaseEntity
    {
        // Foreign Key
        public int MyUserId { get; set; }

        // Token məlumatları
        public string TokenValue { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public string TokenType { get; set; } = "Refresh"; // Refresh, Access, ResetPassword

        // Navigation Property
        public virtual MyUser MyUser { get; set; } = null!;

        // ✅ Sadə computed property
        [NotMapped]
        public bool IsValid => !IsUsed && !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
