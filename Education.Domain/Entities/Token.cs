using System.ComponentModel.DataAnnotations.Schema;

namespace Education.Domain.Entities
{
    public class Token : BaseEntity
    {
        public int MyUserId { get; set; }
        public string TokenValue { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public string TokenType { get; set; } = "Refresh";

        // Navigation Property
        public virtual MyUser MyUser { get; set; } = null!;

        // Computed Property
        [NotMapped]
        public bool IsValid => !IsUsed && !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
