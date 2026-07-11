using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Catalog.Services
{
    public interface IPhongChieuBUS
    {
        List<PhongChieuDTO> LayDanhSachPhong();
        PhongChieuDTO LayChiTietPhong(int maPhong);
        bool ThemPhong(PhongChieuDTO dto);
        string SuaPhong(PhongChieuDTO dto); 
        string XoaPhong(int maPhong);
    }
}


