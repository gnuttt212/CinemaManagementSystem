using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IPhimBUS
    {       
        List<PhimDTO> LayDanhSachPhim();
        List<PhimDTO> LayDanhSachPhimDangChieu(DateTime? selectedDate = null);
        List<PhimDTO> LayDanhSachPhimDangChieu_SP();
        PhimDTO LayChiTietPhim(int maPhim);
        List<GheDTO> LayDanhSachGheTheoSuat(int maSuat);
        int ThemPhim(PhimDTO phim);
        bool SuaPhim(PhimDTO phim);
        bool XoaPhim(int maPhim);
        List<PhimDTO> TimKiemPhim(string query);

        // Hỗ trợ luồng chọn ghế (ChonGhe, LuuGheVaoSession)
        SuatChieu? LaySuatChieu(int maSuat);
        SuatChieu? LaySuatChieuChiTiet(int maSuat);
        List<Ghe> LayDanhSachGheTheoPhong(int maPhong);
        Ghe? LayGheTheoTenVaPhong(string tenGhe, int maPhong);
        DichVu? LayDichVu(int maDV);
    }
}