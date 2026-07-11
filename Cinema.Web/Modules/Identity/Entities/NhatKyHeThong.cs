using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Identity.Entities;

public partial class NhatKyHeThong
{
    public int MaNhatKy { get; set; }

    public string HanhDong { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? TaiKhoan { get; set; }

    public DateTime ThoiGian { get; set; }

    public string? DiaChiIp { get; set; }
}

