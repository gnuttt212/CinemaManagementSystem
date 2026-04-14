using Cinema.DAL.Models;
using Cinema.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.BUS
{
    public class DichVuBUS : IDichVuBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public DichVuBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public List<DichVuDTO> LayDanhSachDichVu()
        {
            return _db.DichVus
                .Select(d => new DichVuDTO
                {
                    MaDV = d.MaDv,
                    TenDV = d.TenDv ?? "", 
                    DonGia = d.DonGia ?? 0,
                    SoLuongTon = d.SoLuongTon ?? 0
                }).ToList();
        }

        public DichVuDTO LayTheoId(int id)
        {
            var result = _db.DichVus
                .Where(d => d.MaDv == id)
                .Select(d => new DichVuDTO
                {
                    MaDV = d.MaDv,
                    TenDV = d.TenDv ?? "",
                    DonGia = d.DonGia ?? 0,
                    SoLuongTon = d.SoLuongTon ?? 0
                }).FirstOrDefault();
            return result; 
        }
    }
}