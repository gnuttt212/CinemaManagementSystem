using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class VwDoanhThuTheoPhim
{
    public int MaPhim { get; set; }

    public string TenPhim { get; set; } = null!;

    public int? SoLuongHoaDon { get; set; }

    public decimal TongDoanhThu { get; set; }
}
