using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Education.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public decimal? GPA { get; set; }

        public DateTime? GraduationDate { get; set; }
        public string? Major { get; set; }
        public string? Faculty { get; set; }
        public int? CurrentSemester { get; set; }
        

        public int MyUserId { get; set; }
        [JsonIgnore]
        public virtual MyUser MyUser { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = [];
        [JsonIgnore]
        public virtual ICollection<StudentDiscipline> Disciplines { get; set; } = [];

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
        
        public int TotalCreditsCompleted
        {
            get
            {
                if (StudentCourses == null)
                {
                    return 0;
                }

                int totalCredits = 0;


                foreach (var sc in StudentCourses)
                {
                    if (sc.Status == "Completed" && sc.Grade >= 50)
                    {
                        if (sc.Course != null)
                        {
                            totalCredits += sc.Course.Credits;
                        }
                    }
                }

                return totalCredits;
            }
        }

        [NotMapped]
        public int CurrentCoursesCount
        {
            get
            {
                if (StudentCourses == null)
                {
                    return 0;
                }

                int count = 0;


                foreach (var sc in StudentCourses)
                {
                    if (sc.Status == "Enrolled")
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        
        [NotMapped]
        public bool CanEnrollInNewCourse
        {
            get
            {
                // Əgər tələbə aktivdirsə və hal-hazırda qeydiyyatda olduğu kurs sayı 8-dən azdırsa
                if (IsActive && CurrentCoursesCount < 8)
                {
                    return true;
                }
                else
                {
                    return false;
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

        // Tələbə xaric edilib? (expelled)
        [NotMapped]
        public bool IsExpelled =>
            Disciplines?.Any(d =>
                d.Penalty == "Expulsion" &&
                d.StartDate <= DateTime.UtcNow
            ) == true;

        // Tələbə aktiv oxuyur? (heç bir kursda enrolled)
        [NotMapped]
        public bool HasActiveEnrollments =>
            StudentCourses?.Any(sc => sc.Status == "Enrolled") == true;

        // Tələbə ümumilikdə aktivdir? (məntiq)
        [NotMapped]
        public bool IsActive =>
            !IsExpelled &&           // Xaric edilməyib
            !IsSuspended &&          // Cəza almamış
            HasActiveEnrollments &&  // Aktiv kursları var
            !IsGraduated;            // Məzun olmayıb
    }
}
