using Cinema.DAL.Models;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cinema.BUS
{
    public class HoaDonBUS : IHoaDonBUS
    {
        private readonly QuanLyRapPhimContext _context;

        public HoaDonBUS(QuanLyRapPhimContext context)
        {
            _context = context;
        }
        public List<HoaDonDTO> LayDanhSachHoaDon()
        {
            return _context.HoaDons
                .Select(h => new HoaDonDTO
                {
                    MaHD = h.MaHd,
                    NgayLap = h.NgayLap,
                    TongTien = h.TongTien
                }).ToList();
        }
        public HoaDonDTO LayChiTietHoaDon(int maHD)
        {
            var h = _context.HoaDons.FirstOrDefault(x => x.MaHd == maHD);
            if (h == null) return null;
            return new HoaDonDTO
            {
                MaHD = h.MaHd,
                NgayLap = h.NgayLap,
                TongTien = h.TongTien
            };
        }
        public HoaDonDTO LayChiTietHoaDonFull(int maHD)
        {
            var hd = _context.HoaDons
                .Include(h => h.Ves).ThenInclude(v => v.MaGheNavigation)
                .Include(h => h.Ves).ThenInclude(v => v.MaSuatNavigation).ThenInclude(s => s.MaPhimNavigation)
                .Include(h => h.ChiTietDvs).ThenInclude(ct => ct.MaDvNavigation)
                .FirstOrDefault(h => h.MaHd == maHD);

            if (hd == null) return null;

            var firstVe = hd.Ves.FirstOrDefault();

            return new HoaDonDTO
            {
                MaHD = hd.MaHd,
                NgayLap = hd.NgayLap,
                TongTien = hd.TongTien,
                TenPhim = firstVe?.MaSuatNavigation?.MaPhimNavigation?.TenPhim?.Trim() ?? "N/A",
                DanhSachGhe = hd.Ves.Select(v => v.MaGheNavigation?.TenGhe?.Trim() ?? "??").ToList(),
                SuatChieu = firstVe?.MaSuatNavigation?.GioBatDau?.ToString("HH:mm") ?? "N/A",
                DanhSachDichVu = hd.ChiTietDvs.Select(ct => new ChiTietDV_DTO
                {
                    TenDV = ct.MaDvNavigation?.TenDv ?? "Dịch vụ",
                    SoLuong = ct.SoLuong ?? 0,
                    DonGiaBan = ct.DonGiaBan ?? 0
                }).ToList()
            };
        }
        public List<int> LayDanhSachMaGheDaDat(int maSuat)
        {
            return _context.Ves
                .Where(v => v.MaSuat == maSuat && v.MaGhe.HasValue)
                .Select(v => v.MaGhe.Value) 
                .ToList();
        }
        public bool KiemTraGheDaDat(int maSuat, int maGhe)
        {
            return _context.Ves.Any(v => v.MaSuat == maSuat && v.MaGhe == maGhe);
        }
        public int LuuVaThanhToan(CartItemDTO cart, string username)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = _context.NguoiDungs.FirstOrDefault(u => u.TaiKhoan == username);
                    if (user == null) return 0;
                    var suatChieu = _context.SuatChieus.Find(cart.MaSuat);
                    if (suatChieu == null) return 0;
                    var dsGheTrongPhong = _context.Ghes
                        .Where(g => cart.DanhSachGhe.Contains(g.TenGhe) && g.MaPhong == suatChieu.MaPhong)
                        .ToList();

                    if (dsGheTrongPhong.Count != cart.DanhSachGhe.Count) return 0;
                    var dsMaGhe = dsGheTrongPhong.Select(g => g.MaGhe).ToList();
                    bool biTrungGhe = _context.Ves.Any(v => v.MaSuat == cart.MaSuat && v.MaGhe.HasValue && dsMaGhe.Contains(v.MaGhe.Value));

                    if (biTrungGhe) return -1; 
                    var hoaDon = new HoaDon
                    {
                        MaNd = user.MaNd,
                        NgayLap = DateTime.Now,
                        TongTien = cart.TongTien
                    };
                    _context.HoaDons.Add(hoaDon);
                    _context.SaveChanges();
                    foreach (var ghe in dsGheTrongPhong)
                    {
                        _context.Ves.Add(new Ve
                        {
                            MaHd = hoaDon.MaHd,
                            MaSuat = cart.MaSuat,
                            MaGhe = ghe.MaGhe,
                            GiaVe = suatChieu.GiaVe
                        });
                    }
                    if (cart.DichVus != null)
                    {
                        foreach (var dv in cart.DichVus)
                        {
                            _context.ChiTietDvs.Add(new ChiTietDv
                            {
                                MaHd = hoaDon.MaHd,
                                MaDv = dv.MaDV,
                                SoLuong = dv.SoLuong,
                                DonGiaBan = dv.DonGia
                            });
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit(); 

                    return hoaDon.MaHd;
                }
                catch (Exception)
                {
                    transaction.Rollback(); 
                    return 0;
                }
            }
        }
    }
}