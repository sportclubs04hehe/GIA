using Core.Entities.Domain.NghiepVu;
using Core.Interfaces.IRepository.INghiepVu;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.NghiepVu
{
    public class ThuThapGiaChiTietRepository : GenericRepository<ThuThapGiaChiTiet>, IThuThapGiaChiTietRepository
    {
        public ThuThapGiaChiTietRepository(StoreContext context) : base(context)
        {
        }

        protected override IQueryable<ThuThapGiaChiTiet> IncludeRelations(IQueryable<ThuThapGiaChiTiet> query)
        {
            return query
                .Include(c => c.ThuThapGiaThiTruong)
                .Include(c => c.HangHoaThiTruong)
                    .ThenInclude(h => h.DonViTinh);
        }

        public async Task<List<ThuThapGiaChiTiet>> GetByThuThapGiaIdAsync(Guid thuThapGiaId)
        {
            return await _context.ThuThapGiaChiTiets
                .Where(c => c.ThuThapGiaThiTruongId == thuThapGiaId && !c.IsDelete)
                .Include(c => c.HangHoaThiTruong)
                    .ThenInclude(h => h.DonViTinh)
                .OrderBy(c => c.HangHoaThiTruong.Ten)
                .ToListAsync();
        }

        public async Task<List<ThuThapGiaChiTiet>> GetLichSuGiaByHangHoaAsync(Guid hangHoaId)
        {
            return await _context.ThuThapGiaChiTiets
                .Where(c => c.HangHoaThiTruongId == hangHoaId && !c.IsDelete)
                .Include(c => c.ThuThapGiaThiTruong)
                .OrderByDescending(c => c.ThuThapGiaThiTruong.NgayNhap)
                .ToListAsync();
        }
    }
}
