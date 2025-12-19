using Education.Domain.Entities;
using System.Linq.Expressions;

namespace Education.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        // Read
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        // Create
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        // Update
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        // Delete
        Task SoftDeleteAsync(int id);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        // Check
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
