using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.ServiceInterface.INghiepVu;
using AutoMapper;
using Core.Entities.Domain.NghiepVu;
using Core.Interfaces.IRepository.INghiepVu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceImplement.NghiepVu
{
    public class ThuThapGiaChiTietService : IThuThapGiaChiTietService
    {
        private readonly IThuThapGiaChiTietRepository _chiTietGiaRepository;
        private readonly IThuThapGiaThiTruongRepository _thuThapGiaRepository;
        private readonly IMapper _mapper;

        public ThuThapGiaChiTietService(
            IThuThapGiaChiTietRepository chiTietGiaRepository,
            IThuThapGiaThiTruongRepository thuThapGiaRepository,
            IMapper mapper)
        {
            _chiTietGiaRepository = chiTietGiaRepository;
            _thuThapGiaRepository = thuThapGiaRepository;
            _mapper = mapper;
        }

        // Lấy danh sách chi tiết giá của một phiếu thu thập giá
        public async Task<IEnumerable<ThuThapGiaChiTietDto>> GetByThuThapGiaIdAsync(Guid thuThapGiaId)
        {
            var chiTietGia = await _chiTietGiaRepository.GetByThuThapGiaIdAsync(thuThapGiaId);
            return _mapper.Map<IEnumerable<ThuThapGiaChiTietDto>>(chiTietGia);
        }

        // Lấy thông tin chi tiết giá của một mặt hàng
        public async Task<ThuThapGiaChiTietDto> GetByIdAsync(Guid id)
        {
            var chiTietGia = await _chiTietGiaRepository.GetByIdAsync(id);
            return _mapper.Map<ThuThapGiaChiTietDto>(chiTietGia);
        }

        // Lấy lịch sử giá của một mặt hàng
        public async Task<IEnumerable<ThuThapGiaChiTietDto>> GetLichSuGiaByHangHoaAsync(Guid hangHoaId)
        {
            var lichSuGia = await _chiTietGiaRepository.GetLichSuGiaByHangHoaAsync(hangHoaId);
            return _mapper.Map<IEnumerable<ThuThapGiaChiTietDto>>(lichSuGia);
        }

        // Tạo mới một chi tiết giá
        public async Task<ThuThapGiaChiTietDto> CreateAsync(ThuThapGiaChiTietCreateDto chiTietGiaDto)
        {
            var chiTietGia = _mapper.Map<ThuThapGiaChiTiet>(chiTietGiaDto);
            
            // Tự động tính toán mức tăng/giảm và tỷ lệ % nếu có giá trước và giá sau
            if (chiTietGia.GiaBinhQuanKyTruoc.HasValue && chiTietGia.GiaBinhQuanKyNay.HasValue 
                && chiTietGia.GiaBinhQuanKyTruoc > 0)
            {
                chiTietGia.MucTangGiamGiaBinhQuan = chiTietGia.GiaBinhQuanKyNay - chiTietGia.GiaBinhQuanKyTruoc;
                chiTietGia.TyLeTangGiamGiaBinhQuan = Math.Round(
                    (decimal)(chiTietGia.MucTangGiamGiaBinhQuan / chiTietGia.GiaBinhQuanKyTruoc * 100), 2);
            }
            
            var result = await _chiTietGiaRepository.AddAsync(chiTietGia);
            return _mapper.Map<ThuThapGiaChiTietDto>(result);
        }

        // Cập nhật một chi tiết giá
        public async Task<bool> UpdateAsync(ThuThapGiaChiTietUpdateDto chiTietGiaDto)
        {
            var chiTietGia = _mapper.Map<ThuThapGiaChiTiet>(chiTietGiaDto);
            
            // Tự động tính toán mức tăng/giảm và tỷ lệ % nếu có giá trước và giá sau
            if (chiTietGia.GiaBinhQuanKyTruoc.HasValue && chiTietGia.GiaBinhQuanKyNay.HasValue 
                && chiTietGia.GiaBinhQuanKyTruoc > 0)
            {
                chiTietGia.MucTangGiamGiaBinhQuan = chiTietGia.GiaBinhQuanKyNay - chiTietGia.GiaBinhQuanKyTruoc;
                chiTietGia.TyLeTangGiamGiaBinhQuan = Math.Round(
                    (decimal)(chiTietGia.MucTangGiamGiaBinhQuan / chiTietGia.GiaBinhQuanKyTruoc * 100), 2);
            }
            
            return await _chiTietGiaRepository.UpdateAsync(chiTietGia);
        }

        // Xóa một chi tiết giá
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _chiTietGiaRepository.DeleteAsync(id);
        }

        // Lưu nhiều chi tiết giá cùng lúc
        public async Task<bool> SaveManyAsync(IEnumerable<ThuThapGiaChiTietCreateDto> chiTietGiaDto)
        {
            var chiTietGiaList = _mapper.Map<IEnumerable<ThuThapGiaChiTiet>>(chiTietGiaDto);
            
            // Tính toán mức tăng/giảm và tỷ lệ % cho tất cả chi tiết
            foreach (var item in chiTietGiaList)
            {
                if (item.GiaBinhQuanKyTruoc.HasValue && item.GiaBinhQuanKyNay.HasValue 
                    && item.GiaBinhQuanKyTruoc > 0)
                {
                    item.MucTangGiamGiaBinhQuan = item.GiaBinhQuanKyNay - item.GiaBinhQuanKyTruoc;
                    item.TyLeTangGiamGiaBinhQuan = Math.Round(
                        (decimal)(item.MucTangGiamGiaBinhQuan / item.GiaBinhQuanKyTruoc * 100), 2);
                }
            }
            
            await _chiTietGiaRepository.AddRangeAsync(chiTietGiaList);
            return true;
        }

        // Tính toán tỷ lệ tăng giảm cho các chi tiết giá
        public async Task<bool> TinhToanTyLeTangGiamAsync(Guid thuThapGiaId)
        {
            return await _thuThapGiaRepository.TinhToanTyLeTangGiamAsync(thuThapGiaId);
        }
    }
}
