using Core.Entities.IdentityBase;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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


        public virtual async Task<T> GetByIdNoTrackingAsync(Guid id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }

        public virtual async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id && !x.IsDelete);
        }

        public virtual async Task<List<Guid>> ExistsManyAsync(IEnumerable<Guid> ids)
        {
            return await _dbSet
                .Where(x => !x.IsDelete && ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
        }

        protected virtual IQueryable<T> IncludeRelations(IQueryable<T> query)
        => query;

        public virtual async Task<PagedList<T>> GetAllAsync(PaginationParams paginationParams)
        {
            var q = _dbSet
                .Where(x => !x.IsDelete)
                .AsNoTracking();

            q = IncludeRelations(q);

            q = string.IsNullOrEmpty(paginationParams.OrderBy)
                ? q.OrderBy(x => x.CreatedDate)
                : q.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<T>.CreateAsync(q, paginationParams.PageIndex, paginationParams.PageSize);
        }

        public async Task<PagedList<T>> SearchAsync(
            SearchParams p,
            params Expression<Func<T, string>>[] searchFields)
        {
            var query = _dbSet.AsNoTracking()
                             .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(p.SearchTerm) && searchFields.Length > 0)
            {
                var searchTerm = p.SearchTerm.ToLower();

                Expression<Func<T, bool>> combinedExpression = null;

                foreach (var selector in searchFields)
                {
                    var memberExpression = selector.Body as MemberExpression;
                    if (memberExpression == null)
                        continue;

                    string propertyName = memberExpression.Member.Name;

                    var parameter = Expression.Parameter(typeof(T), "x");

                    var property = Expression.Property(parameter, propertyName);
                    var toLower = Expression.Call(property,
                        typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    var contains = Expression.Call(toLower,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                        Expression.Constant(searchTerm));

                    var expression = Expression.Lambda<Func<T, bool>>(contains, parameter);

                    combinedExpression = combinedExpression == null
                        ? expression
                        : CombineOr(combinedExpression, expression);
                }

                if (combinedExpression != null)
                    query = query.Where(combinedExpression);
            }

            var orderedQuery = query.OrderByDescending(x => x.CreatedDate);

            return await PagedList<T>.CreateAsync(orderedQuery, p.PageIndex, p.PageSize);
        }

        private Expression<Func<T, bool>> CombineOr(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var visitor = new ParameterReplacer(expr2.Parameters[0], parameter);
            var body2Modified = visitor.Visit(expr2.Body);
            visitor = new ParameterReplacer(expr1.Parameters[0], parameter);
            var body1Modified = visitor.Visit(expr1.Body);
            var combined = Expression.OrElse(body1Modified, body2Modified);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}
