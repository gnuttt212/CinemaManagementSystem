using System;
using System.Collections.Generic;
using Cinema.Web.Modules.Booking.Entities;

namespace Cinema.Web.Modules.Catalog.Entities;

public partial class DoAn
{
    public int MaDoAn { get; set; }

    public string TenDoAn { get; set; } = null!;

    public decimal? Gia { get; set; }

    public string? Loai { get; set; }

    public virtual ICollection<ChiTietDoAn> ChiTietDoAns { get; set; } = new List<ChiTietDoAn>();
}

