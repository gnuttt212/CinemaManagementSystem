using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class ChiTietDoAn
{
    public int MaHd { get; set; }

    public int MaDoAn { get; set; }

    public int? SoLuong { get; set; }

    public decimal? Gia { get; set; }

    public virtual DoAn MaDoAnNavigation { get; set; } = null!;

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
