using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IRepository.INghiepVu;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;
using System;
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
    }
}
