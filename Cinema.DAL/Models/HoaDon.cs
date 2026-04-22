using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public int? MaKh { get; set; }

    public int? MaNv { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<ChiTietDoAn> ChiTietDoAns { get; set; } = new List<ChiTietDoAn>();

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
