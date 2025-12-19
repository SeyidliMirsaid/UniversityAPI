using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class Student : BaseEntity
    {

        // Student məlumatları
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public decimal? GPA { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? Major { get; set; }
        public string? Faculty { get; set; }
        public int? CurrentSemester { get; set; }


        // Foreign Key
        public int MyUserId { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public virtual MyUser MyUser { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        [JsonIgnore]
        public virtual ICollection<StudentDiscipline> Disciplines { get; set; } = [];
        [JsonIgnore]


        // Sadə computed properties
        [NotMapped]
        public string FullName
        {
            get
            {
                string firstName = "";
                string lastName = "";
                if (MyUser != null)
                {
                    if (!string.IsNullOrEmpty(MyUser.FirstName))
                        firstName = MyUser.FirstName;

                    if (!string.IsNullOrEmpty(MyUser.LastName))
                        lastName = MyUser.LastName;
                }
                return firstName + " " + lastName;
            }
        }
        [NotMapped]
        public string Email
        {
            get
            {
                if (MyUser?.Email != null)
                {
                    return MyUser.Email;
                }
                else
                {
                    return string.Empty;
                }

            }
        }
        [NotMapped]
        public string PhoneNumber
        {
            get
            {

                if (MyUser?.PhoneNumber != null)
                {
                    return MyUser.PhoneNumber;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        [NotMapped]
        //public bool IsGraduated => GraduationDate.HasValue; Tələbə məzun olub?
        public bool IsGraduated
        {
            get
            {
                if (GraduationDate.HasValue)
                {
                    return true;
                }
                return false;
            }
        }
        [NotMapped]
        //public bool IsSuspended => Disciplines?.Any(d => d.Penalty == "Suspension" &&
        //        d.StartDate <= DateTime.UtcNow && (!d.EndDate.HasValue || d.EndDate > DateTime.UtcNow) ) == true;  // Tələbə aktiv cəza almış ? (suspended)
        public bool IsSuspended
        {
            get
            {
                if (Disciplines == null)
                {
                    return false;
                }

                foreach (var d in Disciplines)
                {
                    // Şərtlər:
                    // 1. Cəza "Suspension" olmalıdır
                    // 2. Başlanğıc tarixi artıq gəlib (StartDate <= indi)
                    // 3. Son tarixi ya yoxdur, ya da gələcəkdədir (EndDate > indi)
                    if (d.Penalty == "Suspension" &&
                        d.StartDate <= DateTime.UtcNow &&
                        (!d.EndDate.HasValue || d.EndDate > DateTime.UtcNow))
                    {
                        return true; // Əgər belə bir cəza varsa → tələbə suspenddir
                    }
                }

                // Heç bir uyğun cəza tapılmadı → suspend deyil
                return false;
            }
        }
    }
}
