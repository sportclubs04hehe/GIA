using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.Danhmuc
{
    public class Dm_ThuocTinhRepository : GenericRepository<Dm_ThuocTinh>, IDm_ThuocTinhRepository
    {
        public Dm_ThuocTinhRepository(StoreContext context) : base(context)
        {

        }

        // Lấy tất cả các danh mục cha (chỉ các nhóm không có cha)
        public async Task<List<Dm_ThuocTinh>> GetAllParentCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.Loai == Loai.Cha && x.ThuocTinhChaId == null)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
