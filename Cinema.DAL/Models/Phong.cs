using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class Phong
{
    public int MaPhong { get; set; }

    public string TenPhong { get; set; } = null!;

    public int? SoLuongGhe { get; set; }

    public virtual ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
}
