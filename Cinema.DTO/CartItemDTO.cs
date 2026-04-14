using System.Collections.Generic;
using System.Linq;

namespace Cinema.DTO
{
    public class CartItemDTO
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }

       
        public int MaSuat { get; set; }

        public List<string> DanhSachGhe { get; set; } = new List<string>();
        public decimal TongTienPhim { get; set; }

        public List<DichVuDTO> DichVus { get; set; } = new List<DichVuDTO>();

        
        public decimal TongTien => TongTienPhim + (DichVus?.Sum(d => (d.DonGia) * (d.SoLuong)) ?? 0m);
    }
}