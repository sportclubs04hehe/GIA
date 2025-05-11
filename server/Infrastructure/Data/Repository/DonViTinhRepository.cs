using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository
{
    public class DonViTinhRepository : GenericRepository<Dm_DonViTinh>, IDonViTinhRepository
    {
        public DonViTinhRepository(StoreContext context) : base(context)
        {
        }

        public IQueryable<Dm_DonViTinh> GetActive()
        {
            return _context.Set<Dm_DonViTinh>()
                .Where(d => !d.IsDelete)
                .AsNoTracking();
        }

        public async Task<Dm_DonViTinh> GetByMaAsync(string ma)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Ma == ma && !x.IsDelete);
        }

        public async Task<bool> IsMaUniqueAsync(string ma, Guid? id = null)
        {
            if (id.HasValue)
                return !await _dbSet.AnyAsync(x => x.Ma == ma && x.Id != id.Value && !x.IsDelete);

            return !await _dbSet.AnyAsync(x => x.Ma == ma && !x.IsDelete);
        }

        public Task<PagedList<Dm_DonViTinh>> SearchByNameAsync(SearchParams p)
        {
            return base.SearchAsync(p, x => x.Ten, x => x.Ma);
        }

        public Task<bool> ExistsByMaAsync(
        string ma,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ma))
                throw new ArgumentException("Mã không được để trống", nameof(ma));

            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.Ma == ma && !x.IsDelete);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return query.AnyAsync(cancellationToken);
        }

        public async Task<Dm_DonViTinh?> GetByTenAsync(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                throw new ArgumentException("Tên không được để trống", nameof(ten));

            // Use ToLower() on both sides for case-insensitive comparison that translates to SQL
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Ten.ToLower() == ten.ToLower() && !x.IsDelete);
        }

        public async Task<Dm_DonViTinh> AddIfNotExistsAsync(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                throw new ArgumentException("Tên không được để trống", nameof(ten));

            // Tìm đơn vị tính theo tên
            var entity = await _context.DonViTinhs
                .FirstOrDefaultAsync(x => x.Ten.ToLower() == ten.ToLower() && !x.IsDelete);

            if (entity != null)
                return entity;

            // Nếu không tìm thấy, tạo mới
            entity = new Dm_DonViTinh
            {
                Id = Guid.NewGuid(),
                // Fix: Call GenerateUniqueCodeAsync instead of GenerateCode
                Ma = await GenerateUniqueCodeAsync(ten),
                Ten = ten,
                NgayHieuLuc = DateTime.UtcNow, // Sử dụng UTC
                NgayHetHieuLuc = DateTime.UtcNow.AddYears(10), // Sử dụng UTC
                CreatedDate = DateTime.UtcNow, // Sử dụng UTC
                IsDelete = false
            };

            await _context.DonViTinhs.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        private async Task<string> GenerateUniqueCodeAsync(string ten)
        {
            // Generate a code based on the first characters of each word in the name
            var words = ten.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var code = string.Join("", words.Select(w => w.Length > 0 ? w[0].ToString().ToUpper() : ""));
            
            // Ensure the code has at least one character
            if (string.IsNullOrEmpty(code))
                code = "DVT";
                
            // Ensure the code is unique by appending a number if necessary
            var baseCode = code;
            var counter = 1;
            
            while (await _dbSet.AnyAsync(x => x.Ma == code && !x.IsDelete))
            {
                code = $"{baseCode}{counter++}";
            }
            
            return code;
        }

        public async Task<IEnumerable<Dm_DonViTinh>> BulkAddAsync(IEnumerable<Dm_DonViTinh> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var currentUtcTime = DateTime.UtcNow;
            
            // Ensure all DateTime properties are in UTC
            foreach (var entity in entities)
            {
                // Fix: Use nullable DateTime correctly
                entity.CreatedDate = entity.CreatedDate.HasValue && entity.CreatedDate.Value != DateTime.MinValue 
                    ? DateTime.SpecifyKind(entity.CreatedDate.Value, DateTimeKind.Utc)
                    : currentUtcTime;
                    
                if (entity.ModifiedDate.HasValue && entity.ModifiedDate.Value != DateTime.MinValue)
                    entity.ModifiedDate = DateTime.SpecifyKind(entity.ModifiedDate.Value, DateTimeKind.Utc);
                    
                entity.NgayHieuLuc = DateTime.SpecifyKind(entity.NgayHieuLuc, DateTimeKind.Utc);
                entity.NgayHetHieuLuc = DateTime.SpecifyKind(entity.NgayHetHieuLuc, DateTimeKind.Utc);
            }

            await _context.DonViTinhs.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return entities;
        }
    }
}
