using System.Collections.Generic;
using System.Data;

namespace Cinema.DAL.AdoNet
{
    public interface ICinemaAdoNetDAL
    {
        DataTable LayDanhSachPhimAdoNet();
        Dictionary<string, decimal> GetDoanhThuTheoPhimChart();
        DataSet LayPhimVaLichChieu_DataSet();
    }
}
