using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DanhMuc.Repository
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

        protected override IQueryable<Dm_HangHoa> IncludeRelations(IQueryable<Dm_HangHoa> query)
        {
            return query
                .Include(x => x.DonViTinh);
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

        public async Task<PagedList<Dm_HangHoa>> SearchQuery(SearchParams p)
        {
            // Start with base query that filters deleted items
            var query = _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete);
                
            // Apply search term if provided
            if (!string.IsNullOrWhiteSpace(p.SearchTerm))
            {
                var searchTerm = p.SearchTerm.ToLower();
                query = query.Where(x => 
                    x.TenMatHang.ToLower().Contains(searchTerm) || 
                    x.MaMatHang.ToLower().Contains(searchTerm));
            }
            
            // Include related entity
            query = query.Include(x => x.DonViTinh);
            
            // Apply ordering
            var orderedQuery = query.OrderByDescending(x => x.CreatedDate);
            
            return await PagedList<Dm_HangHoa>.CreateAsync(
                orderedQuery, 
                p.PageIndex, 
                p.PageSize);
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
