using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repository
{
    public class DonViTinhRepository : GenericRepository<Dm_DonViTinh>, IDonViTinhRepository
    {
        public DonViTinhRepository(StoreContext context) : base(context)
        {
        }

        public async Task<Dm_DonViTinh> GetByMaAsync(string ma)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Ma == ma && !x.IsDelete);
        }

        public async Task<bool> IsMaUniqueAsync(string ma, Guid? id = null)
        {
            if (id.HasValue)
                return !await _dbSet.AnyAsync(x => x.Ma == ma && x.Id != id.Value && !x.IsDelete);

            return !await _dbSet.AnyAsync(x => x.Ma == ma && !x.IsDelete);
        }

        public async Task<PagedList<Dm_DonViTinh>> SearchByNameAsync(SearchParams searchParams)
        {
            var query = _dbSet.Where(x => !x.IsDelete).AsQueryable();

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                var searchTermLower = searchParams.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Ten.ToLower().Contains(searchTermLower) ||
                    x.Ma.ToLower().Contains(searchTermLower));
            }

            query = query.OrderByDescending(x => x.CreatedDate);
            return await PagedList<Dm_DonViTinh>.CreateAsync(query, searchParams.PageIndex, searchParams.PageSize);
        }

        public Task<PagedList<Dm_DonViTinh>> SearchAsync(SearchParams p)
        {
            return base.SearchAsync(p, x => x.Ten, x => x.Ma);
        }

        public Task<bool> ExistsByMaAsync(
        string ma,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ma))
                throw new ArgumentException("Mã không được để trống", nameof(ma));

            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.Ma == ma && !x.IsDelete);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return query.AnyAsync(cancellationToken);
        }

    }
}
