using System;

namespace Cinema.DTO
{
    public class KhuyenMaiDTO
    {
        public int MaKM { get; set; }
        public string TenKM { get; set; } = string.Empty;
        public decimal PhanTramGiam { get; set; }
        public string? DieuKien { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }

        public bool ConHieuLuc => NgayKetThuc.HasValue && NgayKetThuc.Value >= DateOnly.FromDateTime(DateTime.Today);
    }
}
