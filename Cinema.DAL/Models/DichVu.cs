using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class DichVu
{
    public int MaDv { get; set; }

    public string? TenDv { get; set; }

    public decimal? DonGia { get; set; }

    public int? SoLuongTon { get; set; }

    public virtual ICollection<ChiTietDv> ChiTietDvs { get; set; } = new List<ChiTietDv>();
}
