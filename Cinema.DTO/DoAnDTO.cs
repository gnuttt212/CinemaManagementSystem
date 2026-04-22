namespace Cinema.DTO
{
    public class DoAnDTO
    {
        public int MaDoAn { get; set; }
        public string TenDoAn { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public string? Loai { get; set; }

        public int SoLuong { get; set; } = 0;
        public decimal ThanhTien => Gia * SoLuong;
    }
}
