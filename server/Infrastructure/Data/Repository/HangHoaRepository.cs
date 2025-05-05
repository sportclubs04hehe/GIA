using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository
{
    public class HangHoaRepository : GenericRepository<Dm_HangHoa>, IHangHoaRepository
    {
        public HangHoaRepository(StoreContext context) : base(context)
        {
        }

        public async Task<Dm_HangHoa> GetByMaMatHangAsync(string maMatHang)
        {
            return await _dbSet
                .Include(h => h.NhomHangHoa)
                .FirstOrDefaultAsync(h => h.MaMatHang == maMatHang && !h.IsDelete);
        }

        public async Task<PagedList<Dm_HangHoa>> GetActiveHangHoaAsync(PaginationParams paginationParams)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(h => !h.IsDelete &&
                       h.NgayHieuLuc <= DateTime.UtcNow &&
                       h.NgayHetHieuLuc >= DateTime.UtcNow)
                .Include(h => h.NhomHangHoa)
                .OrderBy(h => h.TenMatHang);

            return await PagedList<Dm_HangHoa>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        public async Task<PagedList<Dm_HangHoa>> GetAllAsync(PaginationParams paginationParams)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(h => !h.IsDelete)
                .Include(h => h.NhomHangHoa)
                .OrderBy(h => h.TenMatHang);

            return await PagedList<Dm_HangHoa>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        public async Task<PagedList<Dm_HangHoa>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(h => h.NhomHangHoaId == nhomHangHoaId && !h.IsDelete)
                .Include(h => h.NhomHangHoa)
                .OrderBy(h => h.TenMatHang);

            return await PagedList<Dm_HangHoa>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        public async Task<PagedList<Dm_HangHoa>> GetWithFilterAsync(SpecificationParams specParams)
        {
            var query = _dbSet
                .Where(h => !h.IsDelete)
                .Include(h => h.NhomHangHoa)
                .AsQueryable();

            // Apply search if provided
            if (!string.IsNullOrEmpty(specParams.SearchTerm))
            {
                query = query.Where(h =>
                    h.TenMatHang.Contains(specParams.SearchTerm) ||
                    h.MaMatHang.Contains(specParams.SearchTerm) ||
                    (h.GhiChu != null && h.GhiChu.Contains(specParams.SearchTerm)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(specParams.SortBy))
            {
                query = specParams.SortBy.ToLower() switch
                {
                    "tenmathang" => specParams.IsDescending
                        ? query.OrderByDescending(h => h.TenMatHang)
                        : query.OrderBy(h => h.TenMatHang),
                    "mamathang" => specParams.IsDescending
                        ? query.OrderByDescending(h => h.MaMatHang)
                        : query.OrderBy(h => h.MaMatHang),
                    "ngayhieuluc" => specParams.IsDescending
                        ? query.OrderByDescending(h => h.NgayHieuLuc)
                        : query.OrderBy(h => h.NgayHieuLuc),
                    _ => query.OrderBy(h => h.TenMatHang)
                };
            }
            else
            {
                query = query.OrderBy(h => h.TenMatHang);
            }

            return await PagedList<Dm_HangHoa>.CreateAsync(
                query,
                specParams.PageIndex,
                specParams.PageSize);
        }

        public IQueryable<Dm_HangHoa> SearchQuery(SearchParams p)
        {
            var searchTerm = p.SearchTerm?.Trim().ToLower();
            // Tạo pattern LIKE
            var formattedTerm = $"%{searchTerm}%";

            return _dbSet
                .AsNoTracking()
                .Where(h => !h.IsDelete
                    && (string.IsNullOrEmpty(searchTerm)
                        || EF.Functions.Like(h.TenMatHang.ToLower(), formattedTerm)
                        || EF.Functions.Like(h.MaMatHang.ToLower(), formattedTerm)
                        || (h.GhiChu != null && EF.Functions.Like(h.GhiChu.ToLower(), formattedTerm))))
                .OrderBy(h => h.TenMatHang);
        }

        public Task<int> CountAsync()
        => _dbSet
           .AsNoTracking()                     
           .CountAsync(h => !h.IsDelete);      


        public Task<bool> ExistsByMaMatHangAsync(
            string maMatHang,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.MaMatHang == maMatHang && !x.IsDelete);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return query.AnyAsync(cancellationToken);
        }
    }
}
