using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IDoAnBUS
    {
        List<DoAnDTO> LayDanhSachDoAn();
        DoAnDTO LayTheoId(int id);
    }
}
