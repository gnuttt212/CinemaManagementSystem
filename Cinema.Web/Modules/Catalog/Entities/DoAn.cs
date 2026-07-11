using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Catalog.Entities;

public partial class DoAn
{
    public int MaDoAn { get; set; }

    public string TenDoAn { get; set; } = null!;

    public decimal? Gia { get; set; }

    public string? Loai { get; set; }

}

