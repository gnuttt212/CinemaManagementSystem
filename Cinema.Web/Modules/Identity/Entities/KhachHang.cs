using System;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Identity.Entities;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string? HoTen { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public int? DiemTichLuy { get; set; }

    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

}

