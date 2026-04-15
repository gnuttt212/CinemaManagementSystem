using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class SuatChieu
{
    public int MaSuat { get; set; }

    public int? MaPhim { get; set; }

    public DateOnly? NgayChieu { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public decimal? GiaVe { get; set; }

    public int? MaPhong { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public string? TrangThai { get; set; }

    public virtual Phim? MaPhimNavigation { get; set; }

    public virtual Phong? MaPhongNavigation { get; set; }

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
