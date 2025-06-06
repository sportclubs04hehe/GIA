using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.Danhmuc
{
    public class Dm_ThuocTinhRepository : GenericRepository<Dm_ThuocTinh>, IDm_ThuocTinhRepository
    {
        private readonly StoreContext _storeContext;

        public Dm_ThuocTinhRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        // Lấy tất cả các danh mục cha (chỉ các nhóm không có cha)
        public async Task<List<Dm_ThuocTinh>> GetAllParentCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.LoaiThuocTinh == LoaiThuocTinhEnum.NhomThuocTinh && x.ThuocTinhChaId == null)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
