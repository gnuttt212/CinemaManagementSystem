using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class LichChieu
{
    public int MaLich { get; set; }

    public int? MaPhim { get; set; }

    public int? MaPhong { get; set; }

    public DateOnly? NgayChieu { get; set; }

    public TimeOnly? GioChieu { get; set; }

    public decimal? GiaVe { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual Phim? MaPhimNavigation { get; set; }

    public virtual PhongChieu? MaPhongNavigation { get; set; }
}
