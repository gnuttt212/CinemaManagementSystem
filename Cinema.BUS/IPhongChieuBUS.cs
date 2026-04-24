using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IPhongChieuBUS
    {
        List<PhongChieuDTO> LayDanhSachPhong();
        PhongChieuDTO LayChiTietPhong(int maPhong);
        bool ThemPhong(PhongChieuDTO dto);
        string SuaPhong(PhongChieuDTO dto); // return error message or empty string on success
        string XoaPhong(int maPhong);
    }
}
