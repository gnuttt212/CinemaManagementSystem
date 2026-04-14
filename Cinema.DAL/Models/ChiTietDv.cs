using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class ChiTietDv
{
    public int MaHd { get; set; }

    public int MaDv { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGiaBan { get; set; }

    public virtual DichVu MaDvNavigation { get; set; } = null!;

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
