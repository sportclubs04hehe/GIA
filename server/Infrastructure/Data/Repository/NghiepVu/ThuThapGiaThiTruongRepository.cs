using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IRepository.INghiepVu;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repository.NghiepVu
{
    public class ThuThapGiaThiTruongRepository : GenericRepository<ThuThapGiaThiTruong>, IThuThapGiaThiTruongRepository
    {
        public ThuThapGiaThiTruongRepository(StoreContext context) : base(context)
        {
        }

        protected override IQueryable<ThuThapGiaThiTruong> IncludeRelations(IQueryable<ThuThapGiaThiTruong> query)
        {
            return query.Include(x => x.HangHoa)
                       .Include(x => x.LoaiGia);
        }
        
        // Ghi đè phương thức SearchAsync để tìm kiếm theo thuộc tính của entity quan hệ
        public async Task<PagedList<ThuThapGiaThiTruong>> SearchAsync(SearchParams searchParams)
        {
            var query = BaseQuery.AsNoTracking();
            query = IncludeRelations(query);
            
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.ToLower();
                query = query.Where(e => 
                    (e.HangHoa != null && e.HangHoa.Ten.ToLower().Contains(searchTerm)) ||
                    (e.LoaiGia != null && e.LoaiGia.Ten.ToLower().Contains(searchTerm))
                );
            }
            
            query = query.OrderByDescending(x => x.CreatedDate);
            
            return await PagedList<ThuThapGiaThiTruong>.CreateAsync(
                query, 
                searchParams.PageIndex, 
                searchParams.PageSize
            );
        }

        // Thêm method GetPreviousPricesAsync vào repository
        public async Task<Dictionary<Guid, decimal?>> GetPreviousPricesAsync(List<Guid> hangHoaIds, DateTime ngayThuThap, Guid loaiGiaId)
        {
            return await _context.ThuThapGiaThiTruongs
                .Where(x => !x.IsDelete &&
                           hangHoaIds.Contains(x.HangHoaId) &&
                           x.LoaiGiaId == loaiGiaId &&
                           x.NgayThuThap < ngayThuThap)
                .GroupBy(x => x.HangHoaId)
                .Select(g => new
                {
                    HangHoaId = g.Key,
                    GiaBinhQuanKyTruoc = g.OrderByDescending(x => x.NgayThuThap)
                                          .Select(x => x.GiaBinhQuanKyNay)
                                          .FirstOrDefault()
                })
                .ToDictionaryAsync(x => x.HangHoaId, x => x.GiaBinhQuanKyTruoc);
        }

        public async Task<List<Dm_HangHoaThiTruong>> GetHierarchicalDataWithPreviousPricesAsync(
            Guid parentId,
            DateTime ngayThuThap,
            Guid loaiGiaId)
        {
            // Kiểm tra sự tồn tại của hàng hóa gốc
            var parentExists = await _context.Dm_HangHoaThiTruongs.AnyAsync(x => x.Id == parentId && !x.IsDelete);
            if (!parentExists)
                throw new KeyNotFoundException($"Không tìm thấy mặt hàng với ID {parentId}");

            // Lấy tên bảng từ entity configuration
            var hangHoaEntityType = _context.Model.FindEntityType(typeof(Dm_HangHoaThiTruong));
            var donViTinhEntityType = _context.Model.FindEntityType(typeof(Dm_DonViTinh));
            
            string hangHoaTableName = hangHoaEntityType.GetTableName();
            string donViTinhTableName = donViTinhEntityType.GetTableName(); // Sẽ là "DonViTinh"

            // SQL để lấy cấu trúc phân cấp - sửa tên bảng
            string hierarchySql = $@"
            WITH RECURSIVE TreeCTE AS (
                SELECT 
                    hh.""Id"",
                    hh.""Ma"",
                    hh.""Ten"",
                    hh.""GhiChu"",
                    hh.""NgayHieuLuc"",
                    hh.""NgayHetHieuLuc"",
                    hh.""LoaiMatHang"",
                    hh.""MatHangChaId"",
                    hh.""DacTinh"",
                    hh.""DonViTinhId"",
                    hh.""ThuocTinhId"",
                    hh.""CreatedDate"",
                    hh.""ModifiedDate"",
                    hh.""IsDelete"",
                    0 AS level_hierarchy
                FROM ""{hangHoaTableName}"" hh
                WHERE hh.""Id"" = @parentId AND hh.""IsDelete"" = false
                
                UNION ALL
                
                SELECT 
                    child.""Id"",
                    child.""Ma"",
                    child.""Ten"",
                    child.""GhiChu"",
                    child.""NgayHieuLuc"",
                    child.""NgayHetHieuLuc"",
                    child.""LoaiMatHang"",
                    child.""MatHangChaId"",
                    child.""DacTinh"",
                    child.""DonViTinhId"",
                    child.""ThuocTinhId"",
                    child.""CreatedDate"",
                    child.""ModifiedDate"",
                    child.""IsDelete"",
                    parent.level_hierarchy + 1
                FROM ""{hangHoaTableName}"" child
                INNER JOIN TreeCTE parent ON child.""MatHangChaId"" = parent.""Id""
                WHERE child.""IsDelete"" = false
            )
            SELECT 
                t.*,
                dvt.""Ten"" AS ""TenDonViTinh"",
                t.level_hierarchy
            FROM TreeCTE t
            LEFT JOIN ""{donViTinhTableName}"" dvt ON t.""DonViTinhId"" = dvt.""Id"" AND dvt.""IsDelete"" = false
            ORDER BY 
                t.""LoaiMatHang"", 
                CASE WHEN t.""Ma"" ~ E'^\\d+$' THEN CAST(t.""Ma"" AS INTEGER) ELSE 999999 END";

            // Thực hiện query SQL thô và map thủ công
            var hierarchyResults = new List<Dm_HangHoaThiTruong>();
            
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = hierarchySql;
                command.Parameters.Add(new NpgsqlParameter("@parentId", NpgsqlDbType.Uuid) { Value = parentId });

                await _context.Database.OpenConnectionAsync();
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new Dm_HangHoaThiTruong
                        {
                            Id = reader.GetGuid("Id"),
                            Ma = reader.GetString("Ma"),
                            Ten = reader.GetString("Ten"),
                            GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                            NgayHieuLuc = reader.GetDateTime("NgayHieuLuc"),
                            NgayHetHieuLuc = reader.GetDateTime("NgayHetHieuLuc"),
                            LoaiMatHang = (Core.Entities.Domain.DanhMuc.Enum.Loai)reader.GetInt32("LoaiMatHang"),
                            MatHangChaId = reader.IsDBNull("MatHangChaId") ? null : reader.GetGuid("MatHangChaId"),
                            DacTinh = reader.IsDBNull("DacTinh") ? null : reader.GetString("DacTinh"),
                            DonViTinhId = reader.IsDBNull("DonViTinhId") ? null : reader.GetGuid("DonViTinhId"),
                            ThuocTinhId = reader.IsDBNull("ThuocTinhId") ? null : reader.GetGuid("ThuocTinhId"),
                            CreatedDate = reader.GetDateTime("CreatedDate"),
                            ModifiedDate = reader.IsDBNull("ModifiedDate") ? null : reader.GetDateTime("ModifiedDate"),
                            IsDelete = reader.GetBoolean("IsDelete"),
                            Level = reader.GetInt32("level_hierarchy"),
                            MatHangCon = new List<Dm_HangHoaThiTruong>()
                        };

                        // Thêm tên đơn vị tính nếu có
                        if (!reader.IsDBNull("TenDonViTinh"))
                        {
                            item.DonViTinh = new Dm_DonViTinh
                            {
                                Ma = "", // Required member
                                Ten = reader.GetString("TenDonViTinh")
                            };
                        }

                        hierarchyResults.Add(item);
                    }
                }
            }

            // Xây dựng cấu trúc cây - giữ nguyên logic cũ
            var nodeMap = hierarchyResults.ToDictionary(x => x.Id);
            var rootNodes = new List<Dm_HangHoaThiTruong>();

            // Tìm node gốc
            var root = hierarchyResults.FirstOrDefault(x => x.Id == parentId);
            if (root != null)
            {
                rootNodes.Add(root);
            }

            // Nhóm con theo ID cha
            var childrenByParent = hierarchyResults
                .Where(x => x.Id != parentId && x.MatHangChaId.HasValue)
                .GroupBy(x => x.MatHangChaId.Value)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => {
                    if (int.TryParse(x.Ma, out int numericValue))
                        return numericValue;
                    return int.MaxValue;
                }).ToList());

            // Xây dựng quan hệ cha-con
            foreach (var parentNodeId in childrenByParent.Keys)
            {
                if (nodeMap.TryGetValue(parentNodeId, out var parentNode))
                {
                    parentNode.MatHangCon = childrenByParent[parentNodeId];
                }
            }

            return rootNodes;
        }

        public async Task<bool> ExistsForDateAndPriceTypeAsync(Guid hangHoaId, DateTime ngayThuThap, Guid loaiGiaId)
        {
            return await _context.ThuThapGiaThiTruongs
                .AnyAsync(x => !x.IsDelete &&
                              x.HangHoaId == hangHoaId &&
                              x.NgayThuThap.Date == ngayThuThap.Date &&
                              x.LoaiGiaId == loaiGiaId);
        }

        public async Task<List<ThuThapGiaThiTruong>> BulkAddAsync(List<ThuThapGiaThiTruong> entities)
        {
            await _context.ThuThapGiaThiTruongs.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<Dictionary<Guid, bool>> CheckExistenceForBulkAsync(List<Guid> hangHoaIds, DateTime ngayThuThap, Guid loaiGiaId)
        {
            var existingRecords = await _context.ThuThapGiaThiTruongs
                .Where(x => !x.IsDelete &&
                           hangHoaIds.Contains(x.HangHoaId) &&
                           x.NgayThuThap.Date == ngayThuThap.Date &&
                           x.LoaiGiaId == loaiGiaId)
                .Select(x => x.HangHoaId)
                .ToListAsync();

            return hangHoaIds.ToDictionary(id => id, id => existingRecords.Contains(id));
        }

    }
}
