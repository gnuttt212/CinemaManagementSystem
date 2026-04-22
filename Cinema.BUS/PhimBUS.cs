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
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    TheLoai = p.TheLoai,
                    DaoDien = p.DaoDien,
                    ThoiLuong = p.ThoiLuong,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    MoTa = p.MoTa,
                    Poster = p.Poster
                })
                .ToList();
        }

        public List<PhimDTO> LayDanhSachPhimDangChieu(DateTime? selectedDate = null)
        {
            var targetDate = selectedDate.HasValue ? DateOnly.FromDateTime(selectedDate.Value) : DateOnly.FromDateTime(DateTime.Today);

            return _db.Phims
                .Include(p => p.LichChieus)
                .Where(p => p.ThoiLuong > 0 && p.LichChieus.Any(lc => lc.NgayChieu == targetDate))
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    TheLoai = p.TheLoai,
                    DaoDien = p.DaoDien,
                    ThoiLuong = p.ThoiLuong,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    MoTa = p.MoTa,
                    Poster = p.Poster,
                    DanhSachLichChieu = p.LichChieus
                        .Where(lc => lc.NgayChieu == targetDate)
                        .OrderBy(lc => lc.GioChieu)
                        .Select(lc => new LichChieuHienThiDTO
                        {
                            MaLich = lc.MaLich,
                            GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "00:00"
                        }).ToList()
                })
                .ToList();
        }

        public List<PhimDTO> LayDanhSachPhimDangChieu_SP()
        {
            var phims = _db.Phims.FromSqlRaw("EXEC sp_LayDanhSachPhimDangChieu").ToList();
            var maPhims = phims.Select(p => p.MaPhim).ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var lichChieusToday = _db.LichChieus
                .Where(lc => lc.MaPhim.HasValue && maPhims.Contains(lc.MaPhim.Value) && lc.NgayChieu == today)
                .ToList();
            
            return phims.Select(p => new PhimDTO
            {
                MaPhim = p.MaPhim,
                TenPhim = p.TenPhim,
                TheLoai = p.TheLoai,
                DaoDien = p.DaoDien,
                ThoiLuong = p.ThoiLuong,
                NgayKhoiChieu = p.NgayKhoiChieu,
                MoTa = p.MoTa,
                Poster = p.Poster,
                DanhSachLichChieu = lichChieusToday
                    .Where(lc => lc.MaPhim == p.MaPhim)
                    .OrderBy(lc => lc.GioChieu)
                    .Select(lc => new LichChieuHienThiDTO
                    {
                        MaLich = lc.MaLich,
                        GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "00:00"
                    }).ToList()
            }).ToList();
        }

        public List<PhimDTO> TimKiemPhim(string query)
        {
            if (string.IsNullOrEmpty(query)) return LayDanhSachPhimDangChieu();

            var q = query.ToLower();

            return _db.Phims
                .Include(p => p.LichChieus)
                .Where(p => p.TenPhim.ToLower().Contains(q))
                .Select(p => new PhimDTO
                {
                    MaPhim = p.MaPhim,
                    TenPhim = p.TenPhim,
                    TheLoai = p.TheLoai,
                    DaoDien = p.DaoDien,
                    ThoiLuong = p.ThoiLuong,
                    NgayKhoiChieu = p.NgayKhoiChieu,
                    MoTa = p.MoTa,
                    Poster = p.Poster,
                    DanhSachLichChieu = p.LichChieus
                        .Where(lc => lc.NgayChieu >= DateOnly.FromDateTime(DateTime.Today))
                        .OrderBy(lc => lc.GioChieu)
                        .Select(lc => new LichChieuHienThiDTO
                        {
                            MaLich = lc.MaLich,
                            GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "00:00"
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
                TheLoai = phim.TheLoai,
                DaoDien = phim.DaoDien,
                ThoiLuong = phim.ThoiLuong,
                NgayKhoiChieu = phim.NgayKhoiChieu,
                MoTa = phim.MoTa,
                Poster = phim.Poster,
                LichChieus = _db.LichChieus.Where(lc => lc.MaPhim == maPhim)
                    .Select(lc => new LichChieuDTO
                    {
                        MaLich = lc.MaLich,
                        GioChieu = lc.GioChieu.HasValue ? TimeSpan.FromTicks(lc.GioChieu.Value.Ticks) : null,
                        GiaVe = lc.GiaVe
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
                    TheLoai = dto.TheLoai,
                    DaoDien = dto.DaoDien,
                    ThoiLuong = dto.ThoiLuong,
                    NgayKhoiChieu = dto.NgayKhoiChieu,
                    MoTa = dto.MoTa,
                    Poster = dto.Poster
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
                phim.TheLoai = dto.TheLoai;
                phim.DaoDien = dto.DaoDien;
                phim.ThoiLuong = dto.ThoiLuong;
                phim.NgayKhoiChieu = dto.NgayKhoiChieu;
                phim.MoTa = dto.MoTa;
                phim.Poster = dto.Poster;

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

        public List<GheDTO> LayDanhSachGheTheoLich(int maLich)
        {
            var lichChieu = _db.LichChieus.Find(maLich);
            if (lichChieu == null) return new List<GheDTO>();

            return _db.Ghes
                .Where(g => g.MaPhong == lichChieu.MaPhong)
                .Select(g => new GheDTO
                {
                    MaGhe = g.MaGhe,
                    Hang = g.Hang,
                    SoGhe = g.SoGhe,
                    LoaiGhe = g.LoaiGhe,
                    DaDat = _db.ChiTietHoaDons.Any(ct => ct.MaGhe == g.MaGhe && ct.MaLich == maLich)
                }).ToList();
        }

        // === Các method hỗ trợ luồng chọn ghế ===

        public LichChieu? LayLichChieu(int maLich)
        {
            return _db.LichChieus.Find(maLich);
        }

        public LichChieu? LayLichChieuChiTiet(int maLich)
        {
            return _db.LichChieus
                .Include(lc => lc.MaPhimNavigation)
                .FirstOrDefault(lc => lc.MaLich == maLich);
        }

        public List<Ghe> LayDanhSachGheTheoPhong(int maPhong)
        {
            return _db.Ghes
                .Where(g => g.MaPhong == maPhong)
                .OrderBy(g => g.Hang).ThenBy(g => g.SoGhe)
                .ToList();
        }

        public Ghe? LayGheTheoHangSoVaPhong(string hang, int soGhe, int maPhong)
        {
            return _db.Ghes.FirstOrDefault(g => g.Hang == hang && g.SoGhe == soGhe && g.MaPhong == maPhong);
        }

        public DoAn? LayDoAn(int maDoAn)
        {
            return _db.DoAns.Find(maDoAn);
        }
    }
}