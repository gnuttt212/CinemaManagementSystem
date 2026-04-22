using Cinema.DAL.Models;
using Cinema.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.BUS
{
    public class KhuyenMaiBUS : IKhuyenMaiBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public KhuyenMaiBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public List<KhuyenMaiDTO> LayDanhSachKhuyenMai()
        {
            return _db.KhuyenMais
                .Select(km => new KhuyenMaiDTO
                {
                    MaKM = km.MaKm,
                    TenKM = km.TenKm,
                    PhanTramGiam = km.PhanTramGiam ?? 0,
                    DieuKien = km.DieuKien,
                    NgayBatDau = km.NgayBatDau,
                    NgayKetThuc = km.NgayKetThuc
                }).ToList();
        }

        public KhuyenMaiDTO LayTheoId(int id)
        {
            return _db.KhuyenMais
                .Where(km => km.MaKm == id)
                .Select(km => new KhuyenMaiDTO
                {
                    MaKM = km.MaKm,
                    TenKM = km.TenKm,
                    PhanTramGiam = km.PhanTramGiam ?? 0,
                    DieuKien = km.DieuKien,
                    NgayBatDau = km.NgayBatDau,
                    NgayKetThuc = km.NgayKetThuc
                }).FirstOrDefault();
        }

        public int ThemKhuyenMai(KhuyenMaiDTO dto)
        {
            try
            {
                var km = new KhuyenMai
                {
                    TenKm = dto.TenKM,
                    PhanTramGiam = dto.PhanTramGiam,
                    DieuKien = dto.DieuKien,
                    NgayBatDau = dto.NgayBatDau,
                    NgayKetThuc = dto.NgayKetThuc
                };
                _db.KhuyenMais.Add(km);
                _db.SaveChanges();
                return km.MaKm;
            }
            catch { return 0; }
        }

        public bool SuaKhuyenMai(KhuyenMaiDTO dto)
        {
            try
            {
                var km = _db.KhuyenMais.Find(dto.MaKM);
                if (km == null) return false;

                km.TenKm = dto.TenKM;
                km.PhanTramGiam = dto.PhanTramGiam;
                km.DieuKien = dto.DieuKien;
                km.NgayBatDau = dto.NgayBatDau;
                km.NgayKetThuc = dto.NgayKetThuc;

                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool XoaKhuyenMai(int id)
        {
            try
            {
                var km = _db.KhuyenMais.Find(id);
                if (km == null) return false;
                _db.KhuyenMais.Remove(km);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}
