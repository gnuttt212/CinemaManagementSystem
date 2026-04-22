using System.Collections.Generic;
using System.Linq;

namespace Cinema.DTO
{
    public class CartItemDTO
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; } = string.Empty;
        public string? Poster { get; set; }

        public int MaLich { get; set; }

        public List<string> DanhSachGhe { get; set; } = new List<string>();
        public decimal TongTienPhim { get; set; }

        public List<DoAnDTO> DoAns { get; set; } = new List<DoAnDTO>();

        public decimal TongTien => TongTienPhim + (DoAns?.Sum(d => d.Gia * d.SoLuong) ?? 0m);
    }
}