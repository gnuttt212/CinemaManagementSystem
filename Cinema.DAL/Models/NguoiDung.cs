using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class NguoiDung
{
    public int MaNd { get; set; }

    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? Email { get; set; }

    public bool? IsAdmin { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
