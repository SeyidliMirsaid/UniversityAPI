using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class MyUser : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];

       
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

       
        public bool EmailConfirmed { get; set; } = false;
        public bool PhoneConfirmed { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
    

        
        [JsonIgnore]
        public virtual ICollection<MyUserRole> MyUserRoles { get; set; } = [];

        [JsonIgnore]
        public virtual Student? Student { get; set; }

        [JsonIgnore]
        public virtual Teacher? Teacher { get; set; }

        [JsonIgnore]
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();


        

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}"; // Database-də saxlamırıq, amma kodda lazımdır:

        [NotMapped]
        public List<string> Roles
        {
            get
            {
                if (MyUserRoles == null)
                    return new List<string>();

                var roles = new List<string>();
                foreach (var userRole in MyUserRoles)
                {
                    if (userRole?.MyRole?.Name != null)
                        roles.Add(userRole.MyRole.Name);
                }
                return roles;
            }
        }

        [NotMapped]
        public bool HasStudentProfile => Student != null;

        [NotMapped]
        public bool HasTeacherProfile => Teacher != null;

        [NotMapped]
        public string UserType
        {
            get
            {
                if (HasStudentProfile && HasTeacherProfile) return "Both";
                if (HasStudentProfile) return "Student";
                if (HasTeacherProfile) return "Teacher";
                return "User";
            }
        }

        
        public bool IsInRole(string roleName)
        {
            return Roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
