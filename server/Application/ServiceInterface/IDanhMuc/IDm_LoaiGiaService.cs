using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IDm_LoaiGiaService
    {
        // Create
        Task<LoaiGiaDto> CreateAsync(LoaiGiaCreateDto createDto);
        
        // Read
        Task<LoaiGiaDto> GetByIdAsync(Guid id);
        Task<PagedList<LoaiGiaDto>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<LoaiGiaDto>> SearchAsync(SearchParams searchParams);
        
        // Update
        Task<bool> UpdateAsync(Guid id, LoaiGiaUpdateDto updateDto);
        
        // Delete
        Task<bool> DeleteAsync(Guid id);
        
        // Check if exists
        Task<bool> ExistsAsync(Guid id);
    }
}
