using Core.Entities.IdentityBase;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Infrastructure.Data.Utilities;

namespace Infrastructure.Data.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseIdentity
    {
        protected readonly StoreContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly EntityMetadataHandler<T> _metadataHandler;
        protected readonly ExpressionBuilder<T> _expressionBuilder;

        public GenericRepository(StoreContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _metadataHandler = new EntityMetadataHandler<T>();
            _expressionBuilder = new ExpressionBuilder<T>();
        }

        // Base query for active entities
        protected virtual IQueryable<T> BaseQuery => _dbSet.Where(x => !x.IsDelete);

        // Include relationships in queries - override in derived repositories
        protected virtual IQueryable<T> IncludeRelations(IQueryable<T> query) => query;

        public virtual async Task<T> AddAsync(T entity)
        {
            _metadataHandler.SetCreateMetadata(entity);
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            var entitiesList = entities.ToList();
            foreach (var entity in entitiesList)
            {
                _metadataHandler.SetCreateMetadata(entity);
            }

            await _dbSet.AddRangeAsync(entitiesList);
            await _context.SaveChangesAsync();
            return entitiesList;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            var trackedEntity = await _dbSet.FindAsync(entity.Id);
            if (trackedEntity == null) return false;

            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            _metadataHandler.SetUpdateMetadata(trackedEntity);

            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _metadataHandler.SetDeleteMetadata(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await BaseQuery.FirstOrDefaultAsync(x => x.Id == id);
        }

        // Tạo một phương thức mới để xử lý tracking option
        public virtual async Task<T> GetByIdWithTrackingOptionAsync(Guid id, bool tracking = true)
        {
            var query = tracking ? BaseQuery : BaseQuery.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<T> GetByIdNoTrackingAsync(Guid id)
        {
            return await GetByIdWithTrackingOptionAsync(id, tracking: false);
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await BaseQuery.AnyAsync(x => x.Id == id);
        }

        public virtual async Task<List<Guid>> ExistsManyAsync(IEnumerable<Guid> ids)
        {
            return await BaseQuery
                .Where(x => ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task<PagedList<T>> GetAllAsync(PaginationParams paginationParams)
        {
            var query = BaseQuery.AsNoTracking();
            query = IncludeRelations(query);

            query = string.IsNullOrEmpty(paginationParams.OrderBy)
                ? query.OrderBy(x => x.CreatedDate)
                : query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<T>.CreateAsync(query, paginationParams.PageIndex, paginationParams.PageSize);
        }

        public async Task<PagedList<T>> SearchAsync(
            SearchParams p,
            params Expression<Func<T, string>>[] searchFields)
        {
            var query = BaseQuery.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(p.SearchTerm) && searchFields.Length > 0)
            {
                query = _expressionBuilder.ApplySearch(query, p.SearchTerm, searchFields);
            }

            var orderedQuery = query.OrderByDescending(x => x.CreatedDate);
            return await PagedList<T>.CreateAsync(orderedQuery, p.PageIndex, p.PageSize);
        }

        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await BaseQuery
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}