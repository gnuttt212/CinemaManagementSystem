using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Booking.Entities;

public partial class ChiTietDoAn
{
    public int MaHd { get; set; }

    public int MaDoAn { get; set; }

    public int? SoLuong { get; set; }

    public decimal? Gia { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}

