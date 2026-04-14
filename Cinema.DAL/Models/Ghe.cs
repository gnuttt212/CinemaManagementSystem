using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class Ghe
{
    public int MaGhe { get; set; }

    public string? TenGhe { get; set; }

    public int? MaSuat { get; set; }

    public bool? DaDat { get; set; }

    public string? LoaiGhe { get; set; }

    public int? MaPhong { get; set; }

    public virtual SuatChieu? MaSuatNavigation { get; set; }

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
