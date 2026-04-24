namespace Cinema.DTO
{
    public class PhongChieuDTO
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; } = null!;
        public string? LoaiPhong { get; set; }
        public int? SucChua { get; set; }
        public int SoGheDaTao { get; set; }
    }
}
