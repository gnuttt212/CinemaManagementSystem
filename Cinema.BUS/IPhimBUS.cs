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
        List<GheDTO> LayDanhSachGheTheoLich(int maLich);
        int ThemPhim(PhimDTO phim);
        bool SuaPhim(PhimDTO phim);
        bool XoaPhim(int maPhim);
        List<PhimDTO> TimKiemPhim(string query);
        LichChieu? LayLichChieu(int maLich);
        LichChieu? LayLichChieuChiTiet(int maLich);
        List<Ghe> LayDanhSachGheTheoPhong(int maPhong);
        Ghe? LayGheTheoHangSoVaPhong(string hang, int soGhe, int maPhong);
        DoAn? LayDoAn(int maDoAn);
    }
}