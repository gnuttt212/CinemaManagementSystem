using System;
using System.Collections.Generic;
using Cinema.Web.Modules.Booking.Entities;

namespace Cinema.Web.Modules.Catalog.Entities;

public partial class Ghe
{
    public int MaGhe { get; set; }

    public int? MaPhong { get; set; }

    public string? Hang { get; set; }

    public int? SoGhe { get; set; }

    public string? LoaiGhe { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual PhongChieu? MaPhongNavigation { get; set; }
}

