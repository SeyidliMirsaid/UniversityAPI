using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class MyRole : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual ICollection<MyUserRole> MyUserRoles { get; set; } = [];
    }
}
