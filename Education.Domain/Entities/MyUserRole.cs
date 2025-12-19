namespace Education.Domain.Entities
{
    public class MyUserRole : BaseEntity
    {
        public int MyUserId { get; set; }
        public int MyRoleId { get; set; }

        // Navigation Properties
        public virtual MyUser MyUser { get; set; } = null!;
        public virtual MyRole MyRole { get; set; } = null!;
    }
}
