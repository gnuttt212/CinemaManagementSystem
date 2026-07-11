using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.Web.Modules.Catalog.Services
{
    public interface IDoAnBUS
    {
        List<DoAnDTO> LayDanhSachDoAn();
        DoAnDTO LayTheoId(int id);
    }
}


