using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cinema.BUS
{
    public class PhimBUS : IPhimBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public PhimBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public List<PhimDTO> LayDanhSachPhim() 
        {
            return _db.Phims
                .Include(p => p.MaLoaiPhimNavigation)
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    ThoiLuong = p.ThoiLuong,
                    GioiHanTuoi = p.GioiHanTuoi,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    Hinh = p.Hinh,
                    TheLoai = p.MaLoaiPhimNavigation != null ? p.MaLoaiPhimNavigation.TenLoai : "Chưa phân loại"
                })
                .ToList();
        }

        public List<PhimDTO> LayDanhSachPhimDangChieu(DateTime? selectedDate = null)
        {
            var targetDate = selectedDate.HasValue ? DateOnly.FromDateTime(selectedDate.Value) : DateOnly.FromDateTime(DateTime.Today);

            return _db.Phims
                .Include(p => p.SuatChieus)
                .Include(p => p.MaLoaiPhimNavigation)
                .Where(p => p.ThoiLuong > 0 && p.SuatChieus.Any(s => s.NgayChieu == targetDate))
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    ThoiLuong = p.ThoiLuong,
                    GioiHanTuoi = p.GioiHanTuoi,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    Hinh = p.Hinh,
                    TheLoai = p.MaLoaiPhimNavigation != null ? p.MaLoaiPhimNavigation.TenLoai : "Chưa phân loại",
                    DanhSachSuatChieu = p.SuatChieus
                        .Where(s => s.NgayChieu == targetDate)
                        .OrderBy(s => s.GioBatDau)
                        .Select(s => new SuatChieuHienThiDTO
                        {
                            MaSuat = s.MaSuat,
                            GioBatDau = s.GioBatDau.HasValue ? s.GioBatDau.Value.ToString(@"hh\:mm") : "00:00"
                        }).ToList()
                })
                .ToList();
        }

        public List<PhimDTO> LayDanhSachPhimDangChieu_SP()
        {
            var phims = _db.Phims.FromSqlRaw("EXEC sp_LayDanhSachPhimDangChieu").Include(p => p.MaLoaiPhimNavigation).ToList();
            var maPhims = phims.Select(p => p.MaPhim).ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var suatChieusToday = _db.SuatChieus
                .Where(s => s.MaPhim.HasValue && maPhims.Contains(s.MaPhim.Value) && s.NgayChieu == today)
                .ToList();
            
            return phims.Select(p => new PhimDTO
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                ThoiLuong = p.ThoiLuong,
                GioiHanTuoi = p.GioiHanTuoi,
                NgayKhoiChieu = p.NgayKhoiChieu,
                Hinh = p.Hinh,
                TheLoai = p.MaLoaiPhimNavigation != null ? p.MaLoaiPhimNavigation.TenLoai : "Chưa phân loại",
                DanhSachSuatChieu = suatChieusToday
                    .Where(s => s.MaPhim == p.MaPhim)
                    .OrderBy(s => s.GioBatDau)
                    .Select(s => new SuatChieuHienThiDTO
                    {
                        MaSuat = s.MaSuat,
                        GioBatDau = s.GioBatDau.HasValue ? s.GioBatDau.Value.ToString(@"hh\:mm") : "00:00"
                    }).ToList()
            }).ToList();
        }
        public List<PhimDTO> TimKiemPhim(string query)
        {
            if (string.IsNullOrEmpty(query)) return LayDanhSachPhimDangChieu();

            var today = DateTime.Today;
            var q = query.ToLower();

            return _db.Phims
                .Include(p => p.SuatChieus)
                .Include(p => p.MaLoaiPhimNavigation)
                .Where(p => p.TenPhim.ToLower().Contains(q))
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    ThoiLuong = p.ThoiLuong,
                    GioiHanTuoi = p.GioiHanTuoi,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    Hinh = p.Hinh,
                    TheLoai = p.MaLoaiPhimNavigation != null ? p.MaLoaiPhimNavigation.TenLoai : "Chưa phân loại",
                    DanhSachSuatChieu = p.SuatChieus
                        .Where(s => s.NgayChieu >= DateOnly.FromDateTime(DateTime.Today))
                        .OrderBy(s => s.GioBatDau)
                        .Select(s => new SuatChieuHienThiDTO
                        {
                            MaSuat = s.MaSuat,
                            GioBatDau = s.GioBatDau.HasValue ? s.GioBatDau.Value.ToString(@"hh\:mm") : "00:00"
                        }).ToList()
                })
                .ToList();
        }

        public PhimDTO LayChiTietPhim(int maPhim)
        {
            var phim = _db.Phims.FirstOrDefault(p => p.MaPhim == maPhim);
            if (phim == null) return null;

            return new PhimDTO
            {
                MaPhim = phim.MaPhim,
                TenPhim = phim.TenPhim,
                ThoiLuong = phim.ThoiLuong,
                NgayKhoiChieu = phim.NgayKhoiChieu,
                GioiHanTuoi = phim.GioiHanTuoi ?? 0,
                Hinh = phim.Hinh,
                MaLoaiPhim = phim.MaLoaiPhim,
                SuatChieus = _db.SuatChieus.Where(s => s.MaPhim == maPhim)
                    .Select(s => new SuatChieuDTO
                    {
                        MaSuat = s.MaSuat,
                        GioBatDau = s.GioBatDau.HasValue ? TimeSpan.FromTicks(s.GioBatDau.Value.Ticks) : null
                    }).ToList()
            };
        }

        public int ThemPhim(PhimDTO dto)
        {
            try
            {
                var phim = new Phim
                {
                    TenPhim = dto.TenPhim,
                    ThoiLuong = dto.ThoiLuong,
                    GioiHanTuoi = dto.GioiHanTuoi,
                    NgayKhoiChieu = dto.NgayKhoiChieu,
                    Hinh = dto.Hinh,
                    MaLoaiPhim = dto.MaLoaiPhim ?? 1
                };
                _db.Phims.Add(phim);
                _db.SaveChanges();
                return phim.MaPhim;
            }
            catch { return 0; }
        }

        public bool SuaPhim(PhimDTO dto)
        {
            try
            {
                var phim = _db.Phims.Find(dto.MaPhim);
                if (phim == null) return false;

                phim.TenPhim = dto.TenPhim;
                phim.ThoiLuong = dto.ThoiLuong;
                phim.GioiHanTuoi = dto.GioiHanTuoi;
                phim.NgayKhoiChieu = dto.NgayKhoiChieu;
                phim.Hinh = dto.Hinh;
                phim.MaLoaiPhim = dto.MaLoaiPhim;

                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool XoaPhim(int maPhim)
        {
            try
            {
                var phim = _db.Phims.Find(maPhim);
                if (phim == null) return false;
                _db.Phims.Remove(phim);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public List<GheDTO> LayDanhSachGheTheoSuat(int maSuat)
        {
            var suatChieu = _db.SuatChieus.Find(maSuat);
            if (suatChieu == null) return new List<GheDTO>();

            return _db.Ghes
                .Where(g => g.MaPhong == suatChieu.MaPhong)
                .Select(g => new GheDTO
                {
                    MaGhe = g.MaGhe,
                    TenGhe = g.TenGhe,
                    LoaiGhe = g.LoaiGhe,
                    DaDat = _db.Ves.Any(v => v.MaGhe == g.MaGhe && v.MaSuat == maSuat)
                }).ToList();
        }

        // === Các method hỗ trợ luồng chọn ghế (tránh Controller gọi _db trực tiếp) ===

        public SuatChieu? LaySuatChieu(int maSuat)
        {
            return _db.SuatChieus.Find(maSuat);
        }

        public SuatChieu? LaySuatChieuChiTiet(int maSuat)
        {
            return _db.SuatChieus
                .Include(s => s.MaPhimNavigation)
                .FirstOrDefault(s => s.MaSuat == maSuat);
        }

        public List<Ghe> LayDanhSachGheTheoPhong(int maPhong)
        {
            return _db.Ghes
                .Where(g => g.MaPhong == maPhong)
                .OrderBy(g => g.TenGhe)
                .ToList();
        }

        public Ghe? LayGheTheoTenVaPhong(string tenGhe, int maPhong)
        {
            return _db.Ghes.FirstOrDefault(g => g.TenGhe == tenGhe && g.MaPhong == maPhong);
        }

        public DichVu? LayDichVu(int maDV)
        {
            return _db.DichVus.Find(maDV);
        }
    }
}