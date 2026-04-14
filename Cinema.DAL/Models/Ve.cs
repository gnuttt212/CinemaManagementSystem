using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class Ve
{
    public int MaVe { get; set; }

    public int? MaSuat { get; set; }

    public int? MaGhe { get; set; }

    public int? MaHd { get; set; }

    public decimal? GiaVe { get; set; }

    public virtual Ghe? MaGheNavigation { get; set; }

    public virtual HoaDon? MaHdNavigation { get; set; }

    public virtual SuatChieu? MaSuatNavigation { get; set; }
}
