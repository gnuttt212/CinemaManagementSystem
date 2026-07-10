namespace Cinema.DTO.Events
{
    public interface InvoiceCreatedEvent
    {
        int MaHD { get; }
        string TaiKhoan { get; }
        decimal TongTien { get; }
        string TrangThai { get; }
    }
}
