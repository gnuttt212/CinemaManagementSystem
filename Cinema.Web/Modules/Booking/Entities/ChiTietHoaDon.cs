using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Booking.Entities;

public partial class ChiTietHoaDon
{
    public int MaHd { get; set; }

    public int MaGhe { get; set; }

    public int? MaLich { get; set; }

    public decimal? GiaVe { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}

