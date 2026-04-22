using System;
using System.Collections.Generic;

namespace Cinema.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public int? MaKH { get; set; }
        public int? MaNV { get; set; }
        public DateTime? NgayDat { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThai { get; set; }
        
        public string? TenKhachHang { get; set; }
        public string? TenNhanVien { get; set; }
        public string? TenPhim { get; set; }
        public string? LichChieu { get; set; }

        public List<string> DanhSachGhe { get; set; } = new List<string>();
        public List<ChiTietDoAnDTO> DanhSachDoAn { get; set; } = new List<ChiTietDoAnDTO>();
    }

    public class ChiTietDoAnDTO
    {
        public int MaDoAn { get; set; }
        public string TenDoAn { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien => SoLuong * Gia;
    }
}
