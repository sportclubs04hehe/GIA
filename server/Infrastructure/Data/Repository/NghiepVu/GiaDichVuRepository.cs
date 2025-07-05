using Core.Entities.Domain.DanhMuc;
using Core.Interfaces.IRepository.INghiepVu;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository.NghiepVu
{
    public class GiaDichVuRepository : IGiaDichVuRepository
    {
        private readonly StoreContext _context;

        public GiaDichVuRepository(StoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách hàng hóa theo cấu trúc cây và giá kỳ trước trong một truy vấn
        /// </summary>
        public async Task<(List<Dm_HangHoaThiTruong> HangHoa, Dictionary<Guid, decimal> GiaKyTruoc)>
            GetHangHoaVaGiaKyTruocAsync(Guid nhomHangHoaId, DateTime? ngayNhap)
        {
            // 1. Lấy danh sách hàng hóa
            var allChildren = await GetAllChildrenRecursiveAsync(nhomHangHoaId);

            // 2. Lấy giá kỳ trước trong một truy vấn nếu có ngày nhập
            Dictionary<Guid, decimal> previousPrices = new Dictionary<Guid, decimal>();

            if (ngayNhap.HasValue && allChildren.Any())
            {
                var utcNgayNhap = DateTime.SpecifyKind(ngayNhap.Value, DateTimeKind.Utc);
                var hangHoaIds = allChildren.Select(x => x.Id).ToList();

                // Truy vấn kết hợp để lấy phiếu và giá gần nhất trong một lệnh SQL
                var latestPrices = await (
                    from gia in _context.ThuThapGiaChiTiets
                    join phieu in _context.ThuThapGiaThiTruongs
                        on gia.ThuThapGiaThiTruongId equals phieu.Id
                    where phieu.NhomHangHoaId == nhomHangHoaId
                        && phieu.NgayNhap < utcNgayNhap
                        && !phieu.IsDelete
                        && !gia.IsDelete
                        && hangHoaIds.Contains(gia.HangHoaThiTruongId)
                        && gia.GiaBinhQuanKyNay.HasValue
                    orderby phieu.NgayNhap descending
                    select new
                    {
                        gia.HangHoaThiTruongId,
                        gia.GiaBinhQuanKyNay,
                        phieu.NgayNhap
                    }
                ).ToListAsync();

                // Lọc để chỉ lấy giá gần nhất cho mỗi hàng hóa
                previousPrices = latestPrices
                    .GroupBy(x => x.HangHoaThiTruongId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(p => p.NgayNhap)
                            .First()
                            .GiaBinhQuanKyNay.Value
                    );
            }

            return (allChildren, previousPrices);
        }

        /// <summary>
        /// Lấy tất cả mặt hàng con theo cấu trúc cây từ một nhóm hàng hóa
        /// </summary>
        private async Task<List<Dm_HangHoaThiTruong>> GetAllChildrenRecursiveAsync(Guid parentId)
        {
            // Sử dụng CTE tối ưu để lấy tất cả mặt hàng con và tính level
            return await _context.Dm_HangHoaThiTruongs
                .FromSqlRaw(@"
                    WITH RECURSIVE RecursiveChildren AS (
                        -- Anchor: Lấy các mặt hàng con trực tiếp
                        SELECT *, 1 as Level FROM ""Dm_HangHoaThiTruong"" 
                        WHERE ""MatHangChaId"" = {0} AND ""IsDelete"" = false
                        
                        UNION ALL
                        
                        -- Recursive: Lấy các cháu/chắt
                        SELECT c.*, rc.Level + 1 as Level FROM ""Dm_HangHoaThiTruong"" c
                        INNER JOIN RecursiveChildren rc ON c.""MatHangChaId"" = rc.""Id""
                        WHERE c.""IsDelete"" = false
                    )
                    SELECT * FROM RecursiveChildren 
                    ORDER BY Level, ""Ma""", parentId)
                .Include(x => x.DonViTinh)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
