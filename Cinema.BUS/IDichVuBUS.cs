using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cinema.BUS
{
    public interface IDichVuBUS
    {
        List<DichVuDTO> LayDanhSachDichVu();
        DichVuDTO LayTheoId(int id);
    }
}
