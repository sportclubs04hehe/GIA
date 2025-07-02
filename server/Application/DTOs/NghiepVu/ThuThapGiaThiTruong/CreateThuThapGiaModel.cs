using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class CreateThuThapGiaModel
    {
        public ThuThapGiaThiTruongCreateDto ThuThapGia { get; set; }
        public IEnumerable<ThuThapGiaChiTietCreateDto> ChiTietGia { get; set; } = new List<ThuThapGiaChiTietCreateDto>();
    }

    public class UpdateThuThapGiaModel
    {
        public ThuThapGiaThiTruongUpdateDto ThuThapGia { get; set; }
        public IEnumerable<ThuThapGiaChiTietUpdateDto> ChiTietGia { get; set; } = new List<ThuThapGiaChiTietUpdateDto>();
    }
}
