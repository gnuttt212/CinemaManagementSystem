using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class Phim
{
    public int MaPhim { get; set; }

    public string TenPhim { get; set; } = null!;

    public string? TheLoai { get; set; }

    public string? DaoDien { get; set; }

    public int? ThoiLuong { get; set; }

    public DateTime? NgayKhoiChieu { get; set; }

    public string? MoTa { get; set; }

    public string? Poster { get; set; }

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();
}
