using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IDonViTinhRepository : IGenericRepository<Dm_DonViTinh>
    {
        IQueryable<Dm_DonViTinh> GetActive();
        Task<bool> ExistsByMaAsync(
        string ma,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
        Task<Dm_DonViTinh> GetByMaAsync(string ma);
        Task<bool> IsMaUniqueAsync(string ma, Guid? id = null);
        Task<PagedList<Dm_DonViTinh>> SearchByNameAsync(SearchParams searchParams);
        Task<Dm_DonViTinh?> GetByTenAsync(string ten);
        Task<Dm_DonViTinh> AddIfNotExistsAsync(string ten);
        Task<IEnumerable<Dm_DonViTinh>> BulkAddAsync(IEnumerable<Dm_DonViTinh> entities);
    }
}
