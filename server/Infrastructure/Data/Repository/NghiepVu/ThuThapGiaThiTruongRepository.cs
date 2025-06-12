using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Interfaces.IRepository.INghiepVu;
using Infrastructure.Data.Generic;

namespace Infrastructure.Data.Repository.NghiepVu
{
    public class ThuThapGiaThiTruongRepository : GenericRepository<ThuThapGiaThiTruong>, IThuThapGiaThiTruongRepository
    {
        public ThuThapGiaThiTruongRepository(StoreContext context) : base(context)
        {
        }
    }
}
