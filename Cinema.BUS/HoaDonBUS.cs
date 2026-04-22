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
                .Include(h => h.MaKhNavigation)
                .Select(h => new HoaDonDTO
                {
                    MaHD = h.MaHd,
                    NgayDat = h.NgayDat,
                    TongTien = h.TongTien,
                    TrangThai = h.TrangThai,
                    TenKhachHang = h.MaKhNavigation != null ? h.MaKhNavigation.HoTen : null
                }).ToList();
        }

        public HoaDonDTO LayChiTietHoaDon(int maHD)
        {
            var h = _context.HoaDons.FirstOrDefault(x => x.MaHd == maHD);
            if (h == null) return null;
            return new HoaDonDTO
            {
                MaHD = h.MaHd,
                NgayDat = h.NgayDat,
                TongTien = h.TongTien,
                TrangThai = h.TrangThai
            };
        }

        public HoaDonDTO LayChiTietHoaDonFull(int maHD)
        {
            var hd = _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MaGheNavigation)
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MaLichNavigation).ThenInclude(lc => lc.MaPhimNavigation)
                .Include(h => h.ChiTietDoAns).ThenInclude(ct => ct.MaDoAnNavigation)
                .FirstOrDefault(h => h.MaHd == maHD);

            if (hd == null) return null;

            var firstCT = hd.ChiTietHoaDons.FirstOrDefault();

            return new HoaDonDTO
            {
                MaHD = hd.MaHd,
                NgayDat = hd.NgayDat,
                TongTien = hd.TongTien,
                TrangThai = hd.TrangThai,
                TenKhachHang = hd.MaKhNavigation?.HoTen,
                TenNhanVien = hd.MaNvNavigation?.HoTen,
                TenPhim = firstCT?.MaLichNavigation?.MaPhimNavigation?.TenPhim?.Trim() ?? "N/A",
                DanhSachGhe = hd.ChiTietHoaDons.Select(ct => 
                {
                    var ghe = ct.MaGheNavigation;
                    return ghe != null ? $"{ghe.Hang}{ghe.SoGhe}" : "??";
                }).ToList(),
                LichChieu = firstCT?.MaLichNavigation?.GioChieu?.ToString("HH:mm") ?? "N/A",
                DanhSachDoAn = hd.ChiTietDoAns.Select(ct => new ChiTietDoAnDTO
                {
                    TenDoAn = ct.MaDoAnNavigation?.TenDoAn ?? "Đồ ăn",
                    SoLuong = ct.SoLuong ?? 0,
                    Gia = ct.Gia ?? 0
                }).ToList()
            };
        }

        public List<int> LayDanhSachMaGheDaDat(int maLich)
        {
            return _context.ChiTietHoaDons
                .Where(ct => ct.MaLich == maLich)
                .Select(ct => ct.MaGhe)
                .ToList();
        }

        public bool KiemTraGheDaDat(int maLich, int maGhe)
        {
            return _context.ChiTietHoaDons.Any(ct => ct.MaLich == maLich && ct.MaGhe == maGhe);
        }

        public int LuuVaThanhToan(CartItemDTO cart, string taiKhoan)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.TaiKhoan == taiKhoan);
                    if (khachHang == null) return 0;

                    var lichChieu = _context.LichChieus.Find(cart.MaLich);
                    if (lichChieu == null) return 0;

                    // Tìm ghế trong phòng dựa trên tên ghế (HàngSố)
                    var dsGheTrongPhong = new List<Ghe>();
                    foreach (var tenGhe in cart.DanhSachGhe)
                    {
                        if (tenGhe.Length >= 2)
                        {
                            string hang = tenGhe.Substring(0, 1);
                            if (int.TryParse(tenGhe.Substring(1), out int soGhe))
                            {
                                var ghe = _context.Ghes.FirstOrDefault(g => g.Hang == hang && g.SoGhe == soGhe && g.MaPhong == lichChieu.MaPhong);
                                if (ghe != null) dsGheTrongPhong.Add(ghe);
                            }
                        }
                    }

                    if (dsGheTrongPhong.Count != cart.DanhSachGhe.Count) return 0;

                    var dsMaGhe = dsGheTrongPhong.Select(g => g.MaGhe).ToList();
                    bool biTrungGhe = _context.ChiTietHoaDons.Any(ct => ct.MaLich == cart.MaLich && dsMaGhe.Contains(ct.MaGhe));

                    if (biTrungGhe) return -1;

                    var hoaDon = new HoaDon
                    {
                        MaKh = khachHang.MaKh,
                        NgayDat = DateTime.Now,
                        TongTien = cart.TongTien,
                        TrangThai = "Đã thanh toán"
                    };
                    _context.HoaDons.Add(hoaDon);
                    _context.SaveChanges();

                    foreach (var ghe in dsGheTrongPhong)
                    {
                        _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                        {
                            MaHd = hoaDon.MaHd,
                            MaGhe = ghe.MaGhe,
                            MaLich = cart.MaLich,
                            GiaVe = lichChieu.GiaVe
                        });
                    }

                    if (cart.DoAns != null)
                    {
                        foreach (var doAn in cart.DoAns)
                        {
                            _context.ChiTietDoAns.Add(new ChiTietDoAn
                            {
                                MaHd = hoaDon.MaHd,
                                MaDoAn = doAn.MaDoAn,
                                SoLuong = doAn.SoLuong,
                                Gia = doAn.Gia
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