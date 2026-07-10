namespace Cinema.DTO
{
    public class TicketPurchasedEvent
    {
        public int MaHD { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }
}
