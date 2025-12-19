using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class StudentDiscipline : BaseEntity
    {
        public int StudentId { get; set; }

        // Cəzanın növü
        public string Penalty { get; set; } = string.Empty;
        // Mümkün dəyərlər: "Warning", "Suspension", "Expulsion"


        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string IssuedBy { get; set; } = string.Empty; // Kim verib?


        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;


        [NotMapped]
        public bool IsActive =>
            StartDate <= DateTime.UtcNow &&
            (!EndDate.HasValue || EndDate > DateTime.UtcNow);
    }
}
