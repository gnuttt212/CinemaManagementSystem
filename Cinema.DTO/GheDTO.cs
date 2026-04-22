namespace Cinema.DTO
{
    public class GheDTO
    {
        public int MaGhe { get; set; }
        public string? Hang { get; set; }
        public int? SoGhe { get; set; }
        public string? LoaiGhe { get; set; }
        public bool DaDat { get; set; }

        public string TenGhe => $"{Hang}{SoGhe}";
    }
}
