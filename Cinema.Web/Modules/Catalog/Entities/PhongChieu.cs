using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Catalog.Entities;

public partial class PhongChieu
{
    public int MaPhong { get; set; }

    public string TenPhong { get; set; } = null!;

    public string? LoaiPhong { get; set; }

    public int? SucChua { get; set; }

    public virtual ICollection<Ghe> Ghes { get; set; } = new List<Ghe>();

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();
}

