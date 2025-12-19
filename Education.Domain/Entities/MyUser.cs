using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class MyUser : BaseEntity
    {
        // Şəxsi məlumatlar
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Təhlükəsizlik
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Təsdiqləmə
        public bool EmailConfirmed { get; set; } = false;
        public bool PhoneConfirmed { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }


        // Navigation Properties - JSON serialize zamanı ignore et
        [JsonIgnore]
        public virtual ICollection<MyUserRole> MyUserRoles { get; set; } = [];
        [JsonIgnore]
        public virtual Student? Student { get; set; }
        [JsonIgnore]
        public virtual Teacher? Teacher { get; set; }
        [JsonIgnore]
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();


        // Hesablanan property'lər - Database'də saxlanmır
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
        [NotMapped]
        public List<string> Roles => MyUserRoles == null ? new List<string>(): MyUserRoles
            .Where(ur => ur.MyRole != null && !string.IsNullOrEmpty(ur.MyRole.Name))
            .Select(ur => ur.MyRole.Name).Distinct().ToList();
        [NotMapped]
        public bool IsActive => !IsDeleted && EmailConfirmed;
    }
}
