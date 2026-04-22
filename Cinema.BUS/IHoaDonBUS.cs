using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IHoaDonBUS
    {
        List<HoaDonDTO> LayDanhSachHoaDon();
        bool KiemTraGheDaDat(int maLich, int maGhe);
        List<int> LayDanhSachMaGheDaDat(int maLich);
        int LuuVaThanhToan(CartItemDTO cart, string taiKhoan);
        HoaDonDTO LayChiTietHoaDon(int maHD);
        HoaDonDTO LayChiTietHoaDonFull(int maHD);
    }
}