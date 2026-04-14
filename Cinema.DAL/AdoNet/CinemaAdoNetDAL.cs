using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cinema.DAL.AdoNet
{
    public class CinemaAdoNetDAL : ICinemaAdoNetDAL
    {
        private readonly string _connectionString;
        public CinemaAdoNetDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        public DataTable LayDanhSachPhimAdoNet()
        {
            DataTable dt = new DataTable();
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT MaPhim, TenPhim, ThoiLuong, GioiHanTuoi, NgayKhoiChieu FROM Phim";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            
            return dt;
        }

        public Dictionary<string, decimal> GetDoanhThuTheoPhimChart()
        {
            var result = new Dictionary<string, decimal>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TenPhim, TongDoanhThu FROM vw_DoanhThuTheoPhim", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tenPhim = reader["TenPhim"].ToString() ?? "Unknown";
                            decimal doanhThu = Convert.ToDecimal(reader["TongDoanhThu"]);
                            result.Add(tenPhim, doanhThu);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Minh họa ADO.NET mô hình phi kết nối đầy đủ:
        /// DataSet chứa nhiều DataTable (Phim + SuatChieu) cùng lúc
        /// Sử dụng SqlDataAdapter để Fill dữ liệu vào bộ nhớ
        /// </summary>
        public DataSet LayPhimVaSuatChieu_DataSet()
        {
            DataSet ds = new DataSet("CinemaDataSet");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // DataTable 1: Danh sách Phim
                string queryPhim = "SELECT MaPhim, TenPhim, ThoiLuong, GioiHanTuoi, NgayKhoiChieu FROM Phim";
                using (SqlDataAdapter adapterPhim = new SqlDataAdapter(queryPhim, conn))
                {
                    adapterPhim.Fill(ds, "Phim");
                }

                // DataTable 2: Danh sách Suất Chiếu
                string querySuatChieu = @"SELECT sc.MaSuat, sc.MaPhim, sc.MaPhong, sc.NgayChieu, sc.GioBatDau, sc.GiaVe, p.TenPhim 
                                          FROM SuatChieu sc 
                                          LEFT JOIN Phim p ON sc.MaPhim = p.MaPhim
                                          ORDER BY sc.NgayChieu DESC";
                using (SqlDataAdapter adapterSC = new SqlDataAdapter(querySuatChieu, conn))
                {
                    adapterSC.Fill(ds, "SuatChieu");
                }

                // Thiết lập DataRelation giữa 2 bảng trong DataSet (minh họa mô hình phi kết nối)
                if (ds.Tables["Phim"]!.Columns.Contains("MaPhim") && ds.Tables["SuatChieu"]!.Columns.Contains("MaPhim"))
                {
                    DataRelation relation = new DataRelation(
                        "FK_Phim_SuatChieu",
                        ds.Tables["Phim"]!.Columns["MaPhim"]!,
                        ds.Tables["SuatChieu"]!.Columns["MaPhim"]!,
                        false
                    );
                    ds.Relations.Add(relation);
                }
            }

            return ds;
        }
    }
}

