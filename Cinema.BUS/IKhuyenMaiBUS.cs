using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IKhuyenMaiBUS
    {
        List<KhuyenMaiDTO> LayDanhSachKhuyenMai();
        KhuyenMaiDTO LayTheoId(int id);
        int ThemKhuyenMai(KhuyenMaiDTO dto);
        bool SuaKhuyenMai(KhuyenMaiDTO dto);
        bool XoaKhuyenMai(int id);
    }
}
