using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Cinema.BUS
{
    public class KhachHangBUS : IKhachHangBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public KhachHangBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public bool DangKy(KhachHangRegisterRequest req)
        {
            if (_db.KhachHangs.Any(kh => kh.TaiKhoan == req.TaiKhoan)) return false;

            var newKH = new KhachHang
            {
                TaiKhoan = req.TaiKhoan,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(req.MatKhau),
                HoTen = req.HoTen,
                Email = req.Email,
                Sdt = req.SDT,
                DiemTichLuy = 0
            };
            _db.KhachHangs.Add(newKH);
            return _db.SaveChanges() > 0;
        }

        public bool DangNhap(KhachHangLoginRequest req)
        {
            var kh = _db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == req.TaiKhoan);
            if (kh == null) return false;
            return BCrypt.Net.BCrypt.Verify(req.MatKhau, kh.MatKhau);
        }

        public KhachHangDTO? LayKhachHangSauDangNhap(KhachHangLoginRequest req)
        {
            var kh = _db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == req.TaiKhoan);
            if (kh == null || !BCrypt.Net.BCrypt.Verify(req.MatKhau, kh.MatKhau))
                return null;

            return new KhachHangDTO
            {
                MaKH = kh.MaKh,
                TaiKhoan = kh.TaiKhoan,
                HoTen = kh.HoTen ?? "",
                Email = kh.Email ?? "",
                SDT = kh.Sdt,
                DiemTichLuy = kh.DiemTichLuy ?? 0
            };
        }

        public KhachHangDTO LayThongTinProfile(string taiKhoan)
        {
            var kh = _db.KhachHangs
                .Include(k => k.HoaDons)
                .FirstOrDefault(k => k.TaiKhoan == taiKhoan);

            if (kh == null) return null;

            return new KhachHangDTO
            {
                MaKH = kh.MaKh,
                TaiKhoan = kh.TaiKhoan,
                HoTen = kh.HoTen,
                Email = kh.Email,
                SDT = kh.Sdt,
                NgaySinh = kh.NgaySinh,
                DiemTichLuy = kh.DiemTichLuy ?? 0,
                LichSuHoaDon = kh.HoaDons.Select(h => new HoaDonDTO
                {
                    MaHD = h.MaHd,
                    NgayDat = h.NgayDat,
                    TongTien = h.TongTien,
                    TrangThai = h.TrangThai
                }).ToList()
            };
        }

        public bool CapNhatProfile(KhachHangDTO model)
        {
            try
            {
                var kh = _db.KhachHangs.Find(model.MaKH);
                if (kh == null) return false;
                kh.HoTen = model.HoTen;
                kh.Email = model.Email;
                kh.Sdt = model.SDT;

                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi)
        {
            try
            {
                var kh = _db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == taiKhoan);
                if (kh == null) return false;

                if (!BCrypt.Net.BCrypt.Verify(matKhauCu, kh.MatKhau)) return false;

                kh.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public KhachHangDTO? DangNhapGoogle(string email, string hoTen)
        {
            try
            {
                var kh = _db.KhachHangs.FirstOrDefault(k => k.Email == email);

                if (kh == null)
                    kh = new KhachHang
                    {
                        TaiKhoan = email, 
                        MatKhau = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Mật khẩu ngẫu nhiên
                        HoTen = hoTen ?? email,
                        Email = email,
                        DiemTichLuy = 0
                    };
                    _db.KhachHangs.Add(kh);
                    _db.SaveChanges();
                }

                return new KhachHangDTO
                {
                    MaKH = kh.MaKh,
                    TaiKhoan = kh.TaiKhoan,
                    HoTen = kh.HoTen ?? "",
                    Email = kh.Email ?? "",
                    SDT = kh.Sdt,
                    DiemTichLuy = kh.DiemTichLuy ?? 0
                };
            }
            catch { return null; }
        }

        public List<KhachHangDTO> LayDanhSach()
        {
            return _db.KhachHangs.Select(k => new KhachHangDTO
            {
                MaKH = k.MaKh,
                TaiKhoan = k.TaiKhoan,
                HoTen = k.HoTen ?? "",
                Email = k.Email ?? "",
                SDT = k.Sdt,
                NgaySinh = k.NgaySinh,
                DiemTichLuy = k.DiemTichLuy ?? 0
            }).ToList();
        }

        public KhachHangDTO? LayTheoMa(int maKh)
        {
            var k = _db.KhachHangs.Find(maKh);
            if (k == null) return null;
            return new KhachHangDTO
            {
                MaKH = k.MaKh,
                TaiKhoan = k.TaiKhoan,
                HoTen = k.HoTen ?? "",
                Email = k.Email ?? "",
                SDT = k.Sdt,
                NgaySinh = k.NgaySinh,
                DiemTichLuy = k.DiemTichLuy ?? 0
            };
        }

        public bool Sua(KhachHangDTO model)
        {
            try
            {
                var kh = _db.KhachHangs.Find(model.MaKH);
                if (kh == null) return false;
                kh.HoTen = model.HoTen;
                kh.Email = model.Email;
                kh.Sdt = model.SDT;
                kh.NgaySinh = model.NgaySinh;
                kh.DiemTichLuy = model.DiemTichLuy;

                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool Xoa(int maKh)
        {
            try
            {
                var kh = _db.KhachHangs.Find(maKh);
                if (kh == null) return false;
                _db.KhachHangs.Remove(kh);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}
