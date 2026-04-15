using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Cinema.BUS
{
    public class NguoiDungBUS : INguoiDungBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public NguoiDungBUS(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public bool DangKy(RegisterRequest req)
        {
            
            if (_db.NguoiDungs.Any(u => u.TaiKhoan == req.TaiKhoan)) return false;

            var newUser = new NguoiDung
            {
                TaiKhoan = req.TaiKhoan,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(req.MatKhau),
                HoTen = req.HoTen,
                Email = req.Email
            };
            _db.NguoiDungs.Add(newUser);
            return _db.SaveChanges() > 0;
        }

        public bool DangNhap(LoginRequest req)
        {
            var user = _db.NguoiDungs.FirstOrDefault(u => u.TaiKhoan == req.TaiKhoan);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(req.MatKhau, user.MatKhau);
        }

        public NguoiDungDTO? LayNguoiDungSauDangNhap(LoginRequest req)
        {
            var user = _db.NguoiDungs.FirstOrDefault(u => u.TaiKhoan == req.TaiKhoan);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.MatKhau, user.MatKhau))
                return null;

            return new NguoiDungDTO
            {
                MaND = user.MaNd,
                TaiKhoan = user.TaiKhoan ?? "",
                HoTen = user.HoTen ?? "",
                Email = user.Email ?? "",
                IsAdmin = user.IsAdmin ?? false
            };
        }

        public NguoiDungDTO LayThongTinProfile(string taiKhoan)
        {
            var user = _db.NguoiDungs
                
                .Include(u => u.HoaDons)
                .FirstOrDefault(u => u.TaiKhoan == taiKhoan);

            if (user == null) return null;

            return new NguoiDungDTO
            {
                MaND = user.MaNd,
                TaiKhoan = user.TaiKhoan,
                HoTen = user.HoTen,
                Email = user.Email,
                
                LichSuHoaDon = user.HoaDons.Select(h => new HoaDonDTO
                {
                    MaHD = h.MaHd,
                    NgayLap = h.NgayLap,
                    TongTien = h.TongTien
                }).ToList()
            };
        }
        public bool CapNhatProfile(NguoiDungDTO model)
        {
            try
            {
                var user = _db.NguoiDungs.Find(model.MaND);
                if (user == null) return false;
                user.HoTen = model.HoTen;
                user.Email = model.Email;

                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi)
        {
            try
            {
                var user = _db.NguoiDungs.FirstOrDefault(u => u.TaiKhoan == taiKhoan);
                if (user == null) return false;

                if (!BCrypt.Net.BCrypt.Verify(matKhauCu, user.MatKhau)) return false;

                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}
