using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime? NgayLap { get; set; }
        public decimal? TongTien { get; set; }
        public string? TenPhim { get; set; }
        public string? SuatChieu { get; set; }

        public List<string> DanhSachGhe { get; set; } = new List<string>();
        public List<ChiTietDV_DTO> DanhSachDichVu { get; set; } = new List<ChiTietDV_DTO>();
    }

    public class ChiTietDV_DTO
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaBan;
    }
}
