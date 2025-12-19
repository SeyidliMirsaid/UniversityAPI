namespace Education.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        // Soft Delete
        public bool IsDeleted { get; set; } = false;
    }
}
