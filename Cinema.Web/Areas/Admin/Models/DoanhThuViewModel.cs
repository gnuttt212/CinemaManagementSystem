namespace Cinema.Web.Areas.Admin.Models
{
    public class DoanhThuViewModel
    {
        public decimal TongDoanhThu { get; set; }
        public int TongVeDaBan { get; set; }
        public int TongHoaDon { get; set; }
        public List<DoanhThuTheoPhim> ThongKePhim { get; set; } = new List<DoanhThuTheoPhim>();
    }

    public class DoanhThuTheoPhim
    {
        public string? TenPhim { get; set; }
        public decimal DoanhThu { get; set; }
    }
}