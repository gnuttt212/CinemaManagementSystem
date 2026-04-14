using System.Collections.Generic;
using System.Data;

namespace Cinema.DAL.AdoNet
{
    public interface ICinemaAdoNetDAL
    {
        DataTable LayDanhSachPhimAdoNet();
        Dictionary<string, decimal> GetDoanhThuTheoPhimChart();

        /// <summary>
        /// Lấy dữ liệu Phim + Suất chiếu bằng DataSet (mô hình phi kết nối đầy đủ)
        /// DataSet chứa nhiều DataTable cùng lúc
        /// </summary>
        DataSet LayPhimVaSuatChieu_DataSet();
    }
}
