﻿using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Core.Specifications;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository
{
    public class NhomHangHoaRepository : GenericRepository<Dm_NhomHangHoa>, INhomHangHoaRepository
    {
        public NhomHangHoaRepository(StoreContext context) : base(context)
        {

        }

        // Only implementing methods not in the base class
        public async Task<Dm_NhomHangHoa?> GetByMaNhomAsync(string maNhom)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.MaNhom == maNhom && !x.IsDelete);
        }

        public async Task<bool> ExistsByMaNhomAsync(string maNhom)
        {
            return await _dbSet.AnyAsync(x => x.MaNhom == maNhom && !x.IsDelete);
        }

        public async Task<IReadOnlyList<Dm_NhomHangHoa>> GetRootGroupsAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.NhomChaId == null)
                .OrderBy(x => x.TenNhom)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Dm_NhomHangHoa>> GetChildGroupsAsync(Guid parentId)
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.NhomChaId == parentId)
                .OrderBy(x => x.TenNhom)
                .ToListAsync();
        }

        public async Task<PagedList<Dm_NhomHangHoa>> GetFilteredAsync(SpecificationParams specParams)
        {
            var query = _dbSet
                .Where(x => !x.IsDelete)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(specParams.SearchTerm))
            {
                var searchTerm = specParams.SearchTerm.ToLower();
                query = query.Where(x => 
                    x.TenNhom.ToLower().Contains(searchTerm) || 
                    x.MaNhom.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(specParams.SortBy))
            {
                query = ApplySorting(query, specParams.SortBy, specParams.IsDescending);
            }
            else
            {
                // Default sort by name
                query = query.OrderBy(x => x.TenNhom);
            }

            return await PagedList<Dm_NhomHangHoa>.CreateAsync(
                query, 
                specParams.PageIndex, 
                specParams.PageSize);
        }

        public async Task<Dm_NhomHangHoa> GetWithChildrenAsync(Guid id, int levels = 1)
        {
            var query = _dbSet
                .Where(x => x.Id == id && !x.IsDelete)
                .AsQueryable();

            // Include children recursively based on levels
            for (int i = 0; i < levels; i++)
            {
                query = IncludeChildrenLevel(query, i);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Dm_NhomHangHoa> GetWithProductsAsync(Guid id)
        {
            return await _dbSet
                .Where(x => x.Id == id && !x.IsDelete)
                .Include(x => x.HangHoas.Where(h => !h.IsDelete))
                .FirstOrDefaultAsync();
        }

        public async Task<Dm_NhomHangHoa> GetSingleBySpecAsync(ISpecification<Dm_NhomHangHoa> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Dm_NhomHangHoa>> GetListBySpecAsync(ISpecification<Dm_NhomHangHoa> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<PagedList<Dm_NhomHangHoa>> GetPagedBySpecAsync(ISpecification<Dm_NhomHangHoa> spec, PaginationParams paginationParams)
        {
            var query = ApplySpecification(spec);
            return await PagedList<Dm_NhomHangHoa>.CreateAsync(query, paginationParams.PageIndex, paginationParams.PageSize);
        }

        public async Task<int> CountAsync(ISpecification<Dm_NhomHangHoa> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public override async Task<PagedList<Dm_NhomHangHoa>> GetAllAsync(PaginationParams paginationParams)
        {
            // Apply default ordering before pagination
            var query = _dbSet
                .Where(x => !x.IsDelete)
                .OrderBy(x => x.TenNhom); 

            return await PagedList<Dm_NhomHangHoa>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        // Helper methods
        private IQueryable<Dm_NhomHangHoa> ApplySpecification(ISpecification<Dm_NhomHangHoa> spec)
        {
            var query = _dbSet.Where(x => !x.IsDelete).AsQueryable();
            
            // Apply criteria
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            
            // Apply includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            
            // Apply ordering
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }
            
            // Apply paging
            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            
            return query;
        }

        private IQueryable<Dm_NhomHangHoa> IncludeChildrenLevel(IQueryable<Dm_NhomHangHoa> query, int level)
        {
            if (level == 0)
            {
                return query.Include(x => x.NhomCon.Where(c => !c.IsDelete));
            }
            
            string includePath = "NhomCon";
            for (int i = 0; i < level; i++)
            {
                includePath += ".NhomCon";
            }
            
            return query.Include(includePath);
        }

        private IQueryable<Dm_NhomHangHoa> ApplySorting(IQueryable<Dm_NhomHangHoa> query, string sortBy, bool isDescending)
        {
            return sortBy.ToLower() switch
            {
                "maNhom" => isDescending ? query.OrderByDescending(x => x.MaNhom) : query.OrderBy(x => x.MaNhom),
                "tenNhom" => isDescending ? query.OrderByDescending(x => x.TenNhom) : query.OrderBy(x => x.TenNhom),
                "createdDate" => isDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate),
                _ => query.OrderBy(x => x.TenNhom)
            };
        }
    }
}
