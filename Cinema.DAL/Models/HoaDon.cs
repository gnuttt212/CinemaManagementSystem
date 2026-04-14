using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime? NgayLap { get; set; }

    public decimal? TongTien { get; set; }

    public string? TaiKhoan { get; set; }

    public int? MaNd { get; set; }

    public virtual ICollection<ChiTietDv> ChiTietDvs { get; set; } = new List<ChiTietDv>();

    public virtual NguoiDung? MaNdNavigation { get; set; }

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
