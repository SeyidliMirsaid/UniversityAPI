using Education.Domain.Entities;
using Education.Infrastructure.Persistence.Data;
using Education.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Education.Infrastructure.Persistence.Repositories.Impl
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly UniversityDbContext _database;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(UniversityDbContext database)
        {
            _database = database;
            _dbSet = _database.Set<TEntity>();
        }

        // Read
        public async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        // Create
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await _dbSet.AddRangeAsync(entities);

        // Update
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            entity.UpdateAt = DateTime.UtcNow;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                entity.UpdateAt = DateTime.UtcNow;

            _dbSet.UpdateRange(entities);
        }

        // Delete
        public async Task SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdateAt = DateTime.UtcNow;
                Update(entity);
            }
        }

        public void Remove(TEntity entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        // Check
        public async Task<bool> ExistsAsync(int id) => await _dbSet.AnyAsync(e => e.Id == id);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public async Task<int> CountAsync() => await _dbSet.CountAsync();

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.CountAsync(predicate);
    }
}
