using Core.Entities.IdentityBase;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseIdentity
    {
        protected readonly StoreContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(StoreContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.IsDelete = false;
            entity.CreatedDate = DateTime.UtcNow;

            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            var utcNow = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                entity.IsDelete = false;
                entity.CreatedDate = utcNow;
                _dbSet.Add(entity);
            }

            await _context.SaveChangesAsync();
            return entities;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            var trackedEntity = await _dbSet.FindAsync(entity.Id);
            if (trackedEntity == null) return false;

            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            trackedEntity.ModifiedDate = DateTime.UtcNow;
            trackedEntity.IsDelete = false;

            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;
            entity.IsDelete = true;
            entity.ModifiedDate = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id && !x.IsDelete);
        }

        public virtual async Task<PagedList<T>> GetAllAsync(PaginationParams paginationParams)
        {
            var q = _dbSet.Where(x => !x.IsDelete).AsNoTracking();
            q = string.IsNullOrEmpty(paginationParams.OrderBy)
            ? q.OrderBy(x => x.CreatedDate)
            : q.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<T>.CreateAsync(q, paginationParams.PageIndex, paginationParams.PageSize);
        }

        public async Task<PagedList<T>> SearchAsync(
         SearchParams p,
         params Expression<Func<T, string>>[] searchFields)
        {
            var q = _dbSet.AsNoTracking()
                          .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(p.SearchTerm) && searchFields.Length > 0)
            {
                q = q.WhereIlikeAny(p.SearchTerm, searchFields);
            }

            q = q.OrderByDescending(x => x.CreatedDate);

            return await PagedList<T>.CreateAsync(q, p.PageIndex, p.PageSize);
        }
    }

}
