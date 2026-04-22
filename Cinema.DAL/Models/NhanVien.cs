using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string? HoTen { get; set; }

    public string? ChucVu { get; set; }

    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? PhanQuyen { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
