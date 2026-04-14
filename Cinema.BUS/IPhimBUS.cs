using Cinema.DTO;
using System;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IPhimBUS
    {       
        List<PhimDTO> LayDanhSachPhim();
        List<PhimDTO> LayDanhSachPhimDangChieu();
        List<PhimDTO> LayDanhSachPhimDangChieu_SP();
        PhimDTO LayChiTietPhim(int maPhim);
        List<GheDTO> LayDanhSachGheTheoSuat(int maSuat);
        bool ThemPhim(PhimDTO phim);
        bool SuaPhim(PhimDTO phim);
        bool XoaPhim(int maPhim);
        List<PhimDTO> TimKiemPhim(string query);
    }
}