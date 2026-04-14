using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models;

public partial class LoaiPhim
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Phim> Phims { get; set; } = new List<Phim>();
}
