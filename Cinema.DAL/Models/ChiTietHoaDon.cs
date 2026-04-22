using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class ChiTietHoaDon
{
    public int MaHd { get; set; }

    public int MaGhe { get; set; }

    public int? MaLich { get; set; }

    public decimal? GiaVe { get; set; }

    public virtual Ghe MaGheNavigation { get; set; } = null!;

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual LichChieu? MaLichNavigation { get; set; }
}
