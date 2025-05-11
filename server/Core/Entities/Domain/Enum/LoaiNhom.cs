using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Domain.Enum
{
    public enum LoaiNhom
    {
        NhomPhanLoai = 1,   // Nhóm chỉ để phân loại, không phải hàng hóa thực sự
        HangHoa = 2         // Nhóm này đại diện cho một hàng hóa thực sự
    }
}
