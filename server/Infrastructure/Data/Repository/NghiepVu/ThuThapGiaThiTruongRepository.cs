using Core.Entities.Domain.NghiepVu;
using Core.Interfaces.IRepository.INghiepVu;
using Infrastructure.Data.Generic;
using Infrastructure.Data.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.NghiepVu
{
    public class ThuThapGiaThiTruongRepository : GenericRepository<ThuThapGiaThiTruong>, IThuThapGiaThiTruongRepository
    {
        public ThuThapGiaThiTruongRepository(StoreContext context) : base(context)
        {
        }

        // Override phương thức từ base class để include các quan hệ
        protected override IQueryable<ThuThapGiaThiTruong> IncludeRelations(IQueryable<ThuThapGiaThiTruong> query)
        {
            return query
                .Include(t => t.LoaiGia)
                .Include(t => t.NhomHangHoa);
        }

        // Lấy thông tin chi tiết của một phiếu thu thập giá bao gồm danh sách chi tiết giá
        public async Task<ThuThapGiaThiTruong> GetThuThapGiaThiTruongWithDetailsAsync(Guid id)
        {
            return await _context.ThuThapGiaThiTruongs
                .Where(t => t.Id == id && !t.IsDelete)
                .Include(t => t.LoaiGia)
                .Include(t => t.NhomHangHoa)
                .Include(t => t.ChiTietGia)
                    .ThenInclude(c => c.HangHoaThiTruong)
                        .ThenInclude(h => h.DonViTinh)
                .FirstOrDefaultAsync();
        }

        // Tạo mới phiếu thu thập giá và danh sách chi tiết giá
        public async Task<ThuThapGiaThiTruong> CreateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruong thuThapGia,
            IEnumerable<ThuThapGiaChiTiet> chiTietGia)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Thêm phiếu thu thập giá - giữ nguyên sử dụng _metadataHandler
                _metadataHandler.SetCreateMetadata(thuThapGia);
                await _context.ThuThapGiaThiTruongs.AddAsync(thuThapGia);
                await _context.SaveChangesAsync();

                // Thêm chi tiết giá - sửa để sử dụng lớp tiện ích mới
                foreach (var chiTiet in chiTietGia)
                {
                    chiTiet.ThuThapGiaThiTruongId = thuThapGia.Id;
                    EntityMetadataUtility.SetCreateMetadata(chiTiet);  // Sử dụng lớp tiện ích static
                }

                await _context.ThuThapGiaChiTiets.AddRangeAsync(chiTietGia);
                await _context.SaveChangesAsync();

                // Tính toán tỷ lệ tăng giảm
                await TinhToanTyLeTangGiamAsync(thuThapGia.Id);

                await transaction.CommitAsync();
                return thuThapGia;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Cập nhật phiếu thu thập giá và danh sách chi tiết giá
        public async Task<bool> UpdateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruong thuThapGia,
            IEnumerable<ThuThapGiaChiTiet> chiTietGia)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Cập nhật phiếu thu thập giá - giữ nguyên
                var existingRecord = await _context.ThuThapGiaThiTruongs
                    .FindAsync(thuThapGia.Id);

                if (existingRecord == null)
                    return false;

                _context.Entry(existingRecord).CurrentValues.SetValues(thuThapGia);
                _metadataHandler.SetUpdateMetadata(existingRecord);

                // Lấy danh sách chi tiết giá hiện có
                var existingDetails = await _context.ThuThapGiaChiTiets
                    .Where(c => c.ThuThapGiaThiTruongId == thuThapGia.Id)
                    .ToListAsync();

                // Xác định chi tiết giá cần thêm, cập nhật hoặc xóa
                var chiTietGiaList = chiTietGia.ToList();
                var chiTietIds = chiTietGiaList.Where(c => c.Id != Guid.Empty).Select(c => c.Id);

                // Chi tiết cần xóa
                var detailsToDelete = existingDetails.Where(c => !chiTietIds.Contains(c.Id)).ToList();
                _context.ThuThapGiaChiTiets.RemoveRange(detailsToDelete);

                foreach (var chiTiet in chiTietGiaList)
                {
                    // Nếu là chi tiết mới
                    if (chiTiet.Id == Guid.Empty)
                    {
                        chiTiet.ThuThapGiaThiTruongId = thuThapGia.Id;
                        EntityMetadataUtility.SetCreateMetadata(chiTiet);  // Sử dụng lớp tiện ích static
                        await _context.ThuThapGiaChiTiets.AddAsync(chiTiet);
                    }
                    // Nếu là chi tiết đã tồn tại
                    else
                    {
                        var existingDetail = existingDetails.FirstOrDefault(c => c.Id == chiTiet.Id);
                        if (existingDetail != null)
                        {
                            _context.Entry(existingDetail).CurrentValues.SetValues(chiTiet);
                            EntityMetadataUtility.SetUpdateMetadata(existingDetail);  // Sử dụng lớp tiện ích static
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Tính toán tỷ lệ tăng giảm
                await TinhToanTyLeTangGiamAsync(thuThapGia.Id);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Tính toán tỷ lệ tăng giảm giá cho tất cả chi tiết giá
        public async Task<bool> TinhToanTyLeTangGiamAsync(Guid thuThapGiaThiTruongId)
        {
            try
            {
                var chiTietGia = await _context.ThuThapGiaChiTiets
                    .Where(c => c.ThuThapGiaThiTruongId == thuThapGiaThiTruongId)
                    .ToListAsync();

                foreach (var chiTiet in chiTietGia)
                {
                    if (chiTiet.GiaBinhQuanKyTruoc.HasValue && chiTiet.GiaBinhQuanKyNay.HasValue && chiTiet.GiaBinhQuanKyTruoc > 0)
                    {
                        // Tính mức tăng/giảm
                        chiTiet.MucTangGiamGiaBinhQuan = chiTiet.GiaBinhQuanKyNay - chiTiet.GiaBinhQuanKyTruoc;
                        
                        // Tính tỷ lệ tăng/giảm (%)
                        chiTiet.TyLeTangGiamGiaBinhQuan = Math.Round(
                            (decimal)(chiTiet.MucTangGiamGiaBinhQuan / chiTiet.GiaBinhQuanKyTruoc * 100), 2);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
