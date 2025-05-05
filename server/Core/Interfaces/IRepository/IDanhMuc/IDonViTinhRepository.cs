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
        Task<Dm_DonViTinh> GetByMaAsync(string ma);
        Task<bool> IsMaUniqueAsync(string ma, Guid? id = null);
        Task<PagedList<Dm_DonViTinh>> SearchByNameAsync(SearchParams searchParams);
    }
}
