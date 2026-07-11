using Cinema.Web.Modules.Booking.Data;
using Cinema.Web.Modules.Booking.Entities;
using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Booking.Services
{
    public interface IHoaDonBUS
    {
        List<HoaDonDTO> LayDanhSachHoaDon();
        bool KiemTraGheDaDat(int maLich, int maGhe);
        List<int> LayDanhSachMaGheDaDat(int maLich);
        int LuuVaThanhToan(CartItemDTO cart, string taiKhoan);
        int LuuDonChuaThanhToan(CartItemDTO cart, string taiKhoan);
        bool CapNhatTrangThaiHoaDon(int maHD, string trangThai);
        HoaDonDTO LayChiTietHoaDon(int maHD);
        HoaDonDTO LayChiTietHoaDonFull(int maHD);
    }
}


