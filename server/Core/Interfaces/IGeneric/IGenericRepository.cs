using Core.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IGeneric
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task<T> GetByIdNoTrackingAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Guid>> ExistsManyAsync(IEnumerable<Guid> ids);
        Task<PagedList<T>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<T>> SearchAsync(
            SearchParams searchParams,
            params Expression<Func<T, string>>[] searchFields);
        
        // Thêm phương thức mới để lấy danh sách theo điều kiện
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
    }
}
