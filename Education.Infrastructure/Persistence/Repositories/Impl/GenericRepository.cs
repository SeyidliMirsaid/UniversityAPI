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

        // Read Methods
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // Create Methods
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // Update Methods
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            entity.UpdateDate = DateTime.UtcNow;
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            // 1. Hər entity üçün UpdatedAt set edirik
            foreach (var entity in entities)
            {
                entity.UpdateDate = DateTime.UtcNow;
            }

            // 2. Toplu update
            _dbSet.UpdateRange(entities);
        }

        // Delete Methods
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdateDate = DateTime.UtcNow;
                Update(entity);
                // UpdatedAt avtomatik set olunur
                // Soft delete olur burada silinmir
            }
        }
        public void Remove(TEntity entity)
        {
            // HARD DELETE - DB-dən tam silir
            _dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            // 3. Toplu HARD DELETE
            _dbSet.RemoveRange(entities);
        }

        // CHECK methods
        public async Task<bool> ExistsAsync(int id)
        {
            // ID-yə görə mövcudluq yoxlayır
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Şərtə görə mövcudluq yoxlayır
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<int> CountAsync()
        {
            // Ümumi say(soft delete filter olur)
            return await _dbSet.CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Şərtə görə say
            return await _dbSet.CountAsync(predicate);
        }
    }
}
