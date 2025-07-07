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
            // Sửa tên bảng từ "Dm_HangHoaThiTruongs" thành "Dm_HangHoaThiTruong"
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

        public async Task<List<Dm_HangHoaThiTruong>> SearchMatHangAsync(Guid nhomHangHoaId, string searchTerm, int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Dm_HangHoaThiTruong>();
            
            searchTerm = searchTerm.Trim().ToLower();
            string searchPattern = "%" + searchTerm + "%";
            
            // Bước 1: Tìm tất cả mặt hàng khớp với từ khóa
            var matchedItems = await _context.Dm_HangHoaThiTruongs
                .FromSqlRaw(@"
                    WITH RECURSIVE HangHoaCon AS (
                        -- Lấy node gốc
                        SELECT *, 0 AS ""Level"" FROM ""Dm_HangHoaThiTruong"" 
                        WHERE ""Id"" = {0} AND ""IsDelete"" = false
                        
                        UNION ALL
                        
                        -- Lấy tất cả con cháu
                        SELECT c.*, hc.""Level"" + 1 FROM ""Dm_HangHoaThiTruong"" c
                        JOIN HangHoaCon hc ON c.""MatHangChaId"" = hc.""Id""
                        WHERE c.""IsDelete"" = false
                    )
                    SELECT * FROM HangHoaCon
                    WHERE LOWER(""Ma"") LIKE {1} 
                       OR LOWER(""Ten"") LIKE {1}
                       OR (""DacTinh"" IS NOT NULL AND LOWER(""DacTinh"") LIKE {1})
                    LIMIT {2}",
                    nhomHangHoaId, searchPattern, maxResults)
                .AsNoTracking()
                .ToListAsync();
    
            if (matchedItems.Count == 0)
                return new List<Dm_HangHoaThiTruong>();
            
            // Bước 2: Thu thập tất cả MatHangChaId từ các mặt hàng đã tìm thấy
            var parentIds = matchedItems
                .Where(x => x.MatHangChaId.HasValue)
                .Select(x => x.MatHangChaId.Value)
                .Distinct()
                .ToList();
            
            // Bước 3: Lấy tất cả các nhóm cha
            var allParents = new List<Dm_HangHoaThiTruong>();
            foreach (var parentId in parentIds)
            {
                var parentChain = await GetParentChainAsync(parentId, nhomHangHoaId);
                allParents.AddRange(parentChain);
            }
            
            // Bước 4: Kết hợp kết quả và loại bỏ trùng lặp
            var result = matchedItems.Concat(allParents)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();
            
            // Bước 5: Thêm thông tin đơn vị tính
            var donViTinhIds = result
                .Where(x => x.DonViTinhId.HasValue)
                .Select(x => x.DonViTinhId.Value)
                .Distinct();
            
            var donViTinhs = await _context.DonViTinhs
                .Where(d => donViTinhIds.Contains(d.Id))
                .ToDictionaryAsync(k => k.Id, v => v);
            
            foreach (var item in result)
            {
                if (item.DonViTinhId.HasValue && donViTinhs.ContainsKey(item.DonViTinhId.Value))
                {
                    item.DonViTinh = donViTinhs[item.DonViTinhId.Value];
                }
            }
            
            return result;
        }

        // Phương thức mới để lấy chuỗi các nhóm cha
        private async Task<List<Dm_HangHoaThiTruong>> GetParentChainAsync(Guid startId, Guid stopId)
        {
            var result = new List<Dm_HangHoaThiTruong>();
            var currentId = startId;
            
            while (currentId != stopId)
            {
                var parent = await _context.Dm_HangHoaThiTruongs
                    .Where(x => x.Id == currentId && !x.IsDelete)
                    .FirstOrDefaultAsync();
                
                if (parent == null || !parent.MatHangChaId.HasValue)
                    break;
                
                result.Add(parent);
                currentId = parent.MatHangChaId.Value;
            }
            
            return result;
        }
    }
}
