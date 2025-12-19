using Education.Domain.Entities;
using System.Linq.Expressions;

namespace Education.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        // READ 
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        // CREATE Operations  
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        // UPDATE Operations
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        // DELETE Operations
        Task DeleteAsync(int id); // Soft delete
        void Remove(TEntity entity);    // Hard delete
        void RemoveRange(IEnumerable<TEntity> entities);

        // CHECK Operations
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
