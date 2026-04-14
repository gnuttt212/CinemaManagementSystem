namespace Cinema.DTO
{
    public class DichVuDTO
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; } = string.Empty; 
        public decimal DonGia { get; set; } 
        public int? SoLuongTon { get; set; }
        public string? HinhAnh { get; set; } 

        public int SoLuong { get; set; } = 0;
        public bool ConHang => SoLuongTon > 0;
        public decimal ThanhTien => DonGia * SoLuong;
    }
}
