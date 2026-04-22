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
                string query = "SELECT MaPhim, TenPhim, TheLoai, DaoDien, ThoiLuong, NgayKhoiChieu, Poster FROM Phim";
                
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
                            if (result.ContainsKey(tenPhim))
                                result[tenPhim] += doanhThu;
                            else
                                result[tenPhim] = doanhThu;
                        }
                    }
                }
            }
            return result;
        }

        public DataSet LayPhimVaLichChieu_DataSet()
        {
            DataSet ds = new DataSet("CinemaDataSet");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string queryPhim = "SELECT MaPhim, TenPhim, TheLoai, DaoDien, ThoiLuong, NgayKhoiChieu, Poster FROM Phim";
                using (SqlDataAdapter adapterPhim = new SqlDataAdapter(queryPhim, conn))
                {
                    adapterPhim.Fill(ds, "Phim");
                }

                string queryLichChieu = @"SELECT lc.MaLich, lc.MaPhim, lc.MaPhong, lc.NgayChieu, lc.GioChieu, lc.GiaVe, p.TenPhim 
                                          FROM LichChieu lc 
                                          LEFT JOIN Phim p ON lc.MaPhim = p.MaPhim
                                          ORDER BY lc.NgayChieu DESC";
                using (SqlDataAdapter adapterLC = new SqlDataAdapter(queryLichChieu, conn))
                {
                    adapterLC.Fill(ds, "LichChieu");
                }

                if (ds.Tables["Phim"]!.Columns.Contains("MaPhim") && ds.Tables["LichChieu"]!.Columns.Contains("MaPhim"))
                {
                    DataRelation relation = new DataRelation(
                        "FK_Phim_LichChieu",
                        ds.Tables["Phim"]!.Columns["MaPhim"]!,
                        ds.Tables["LichChieu"]!.Columns["MaPhim"]!,
                        false
                    );
                    ds.Relations.Add(relation);
                }
            }

            return ds;
        }
    }
}
