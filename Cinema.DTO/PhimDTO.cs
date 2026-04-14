using System;
using System.Collections.Generic;

namespace Cinema.DTO
{
    public class PhimDTO
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; } = string.Empty;
        public int? ThoiLuong { get; set; }
        public DateTime? NgayKhoiChieu { get; set; }
        public int? GioiHanTuoi { get; set; }
        public string? MoTa { get; set; }
        public string Hinh { get; set; } = string.Empty;
        public string TheLoai { get; set; } = string.Empty;
        public string? LoaiGhe { get; set; }
        public List<SuatChieuDTO> SuatChieus { get; set; } = new List<SuatChieuDTO>();
        public List<SuatChieuHienThiDTO> DanhSachSuatChieu { get; set; } = new List<SuatChieuHienThiDTO>();

        public string ThongTinHienThi => $"{TenPhim} ({ThoiLuong} phút)";
    }

    public class SuatChieuHienThiDTO
    {
        public int MaSuat { get; set; }
        public string? GioBatDau { get; set; } 
    }

    public class SuatChieuDTO
    {
        public int MaSuat { get; set; }
        public TimeSpan? GioBatDau { get; set; } 
        public decimal? GiaVe { get; set; }
    }
}