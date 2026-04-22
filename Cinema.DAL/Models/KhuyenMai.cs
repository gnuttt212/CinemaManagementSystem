using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string TenKm { get; set; } = null!;

    public decimal? PhanTramGiam { get; set; }

    public string? DieuKien { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }
}
