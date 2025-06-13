using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IThuThapGiaThiTruongRepository : IGenericRepository<ThuThapGiaThiTruong>
    {
        Task<PagedList<ThuThapGiaThiTruong>> SearchAsync(SearchParams searchParams);
    }
}
