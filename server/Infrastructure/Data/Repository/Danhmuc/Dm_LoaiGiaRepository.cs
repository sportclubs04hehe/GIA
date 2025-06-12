using Core.Entities.Domain.DanhMuc;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;

namespace Infrastructure.Data.DanhMuc.Repository
{
    public class Dm_LoaiGiaRepository : GenericRepository<Dm_LoaiGia>, IDm_LoaiGiaRepository
    {
        public Dm_LoaiGiaRepository(StoreContext context) : base(context)
        { 
        }
    }
}
