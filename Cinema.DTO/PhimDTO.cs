using System;
using System.Collections.Generic;

namespace Cinema.DTO
{
    public class PhimDTO
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; } = string.Empty;
        public string? TheLoai { get; set; }
        public string? DaoDien { get; set; }
        public int? ThoiLuong { get; set; }
        public DateTime? NgayKhoiChieu { get; set; }
        public string? MoTa { get; set; }
        public string? Poster { get; set; }
        public List<LichChieuDTO> LichChieus { get; set; } = new List<LichChieuDTO>();
        public List<LichChieuHienThiDTO> DanhSachLichChieu { get; set; } = new List<LichChieuHienThiDTO>();

        public string ThongTinHienThi => $"{TenPhim} ({ThoiLuong} phút)";
    }

    public class LichChieuHienThiDTO
    {
        public int MaLich { get; set; }
        public string? GioChieu { get; set; } 
    }

    public class LichChieuDTO
    {
        public int MaLich { get; set; }
        public TimeSpan? GioChieu { get; set; } 
        public decimal? GiaVe { get; set; }
    }
}