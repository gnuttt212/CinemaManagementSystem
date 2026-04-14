using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class Phim
{
    public int MaPhim { get; set; }

    public string TenPhim { get; set; } = null!;

    public int? ThoiLuong { get; set; }

    public int? GioiHanTuoi { get; set; }

    public int? MaLoaiPhim { get; set; }

    public string? Hinh { get; set; }

    public DateTime? NgayKhoiChieu { get; set; }

    public virtual LoaiPhim? MaLoaiPhimNavigation { get; set; }

    public virtual ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
}
