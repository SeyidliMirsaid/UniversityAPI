using Education.Domain.Entities;

namespace Education.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Hər entity üçün ayrı repository

        IGenericRepository<MyUser> MyUsers { get; }
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<StudentCourse> StudentCourses { get; }
        IGenericRepository<MyRole> MyRoles { get; }
        IGenericRepository<MyUserRole> MyUserRoles { get; }
        IGenericRepository<Token> Tokens { get; }
        IGenericRepository<StudentDiscipline> StudentDisciplines { get; }

        // ========== ƏSAS ƏMƏLİYYATLAR ==========

        /// Bütün dəyişiklikləri save edir
        Task<int> SaveChangesAsync();

        /// Transaction başladır
        Task BeginTransactionAsync();

        /// Transaction commit edir (save edir)
        Task CommitTransactionAsync();

        /// Transaction rollback edir (ləğv edir)
        Task RollbackTransactionAsync();
    }
}
