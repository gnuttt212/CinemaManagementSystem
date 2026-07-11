using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Catalog.Entities;
using System.Collections.Generic;
using System.Data;

namespace Cinema.Web.Modules.Catalog.Services
{
    public interface ICinemaAdoNetDAL
    {
        DataTable LayDanhSachPhimAdoNet();
        Dictionary<string, decimal> GetDoanhThuTheoPhimChart();
        DataSet LayPhimVaLichChieu_DataSet();
    }
}


