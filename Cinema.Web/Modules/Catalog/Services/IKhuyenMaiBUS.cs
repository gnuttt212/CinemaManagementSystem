using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Catalog.Services
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


