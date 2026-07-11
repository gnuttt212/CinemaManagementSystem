using Cinema.Web.Modules.Booking.Data;
using Cinema.Web.Modules.Booking.Entities;
using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Data;
using Cinema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Cinema.DTO.Events;

namespace Cinema.Web.Modules.Booking.Services
{
    public class HoaDonBUS : IHoaDonBUS
    {
        private readonly BookingDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly CatalogDbContext _catalogContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public HoaDonBUS(BookingDbContext context, IdentityDbContext identityContext, CatalogDbContext catalogContext, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _identityContext = identityContext;
            _catalogContext = catalogContext;
            _publishEndpoint = publishEndpoint;
        }

        public List<HoaDonDTO> LayDanhSachHoaDon()
        {
            var hoaDons = _context.HoaDons.ToList();
            var maKhs = hoaDons.Select(h => h.MaKh).Distinct().ToList();
            var khachHangs = _identityContext.KhachHangs.Where(k => maKhs.Contains(k.MaKh)).ToDictionary(k => k.MaKh, k => k.HoTen);

            return hoaDons.Select(h => new HoaDonDTO
            {
                MaHD = h.MaHd,
                NgayDat = h.NgayDat,
                TongTien = h.TongTien,
                TrangThai = h.TrangThai,
                TenKhachHang = h.MaKh != null && khachHangs.ContainsKey(h.MaKh) ? khachHangs[h.MaKh] : null
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
                .Include(h => h.ChiTietHoaDons)
                .Include(h => h.ChiTietDoAns)
                .FirstOrDefault(h => h.MaHd == maHD);

            if (hd == null) return null;

            var kh = hd.MaKh != null ? _identityContext.KhachHangs.FirstOrDefault(k => k.MaKh == hd.MaKh) : null;
            var nv = hd.MaNv != null ? _identityContext.NhanViens.FirstOrDefault(n => n.MaNv == hd.MaNv) : null;

            var firstCT = hd.ChiTietHoaDons.FirstOrDefault();
            string tenPhim = "N/A";
            string lichChieuStr = "N/A";
            if (firstCT != null)
            {
                var lc = _catalogContext.LichChieus.FirstOrDefault(l => l.MaLich == firstCT.MaLich);
                if (lc != null)
                {
                    lichChieuStr = lc.GioChieu?.ToString("HH:mm") ?? "N/A";
                    var phim = _catalogContext.Phims.FirstOrDefault(p => p.MaPhim == lc.MaPhim);
                    if (phim != null) tenPhim = phim.TenPhim?.Trim() ?? "N/A";
                }
            }

            var ghes = new List<string>();
            foreach(var ct in hd.ChiTietHoaDons)
            {
                var ghe = _catalogContext.Ghes.FirstOrDefault(g => g.MaGhe == ct.MaGhe);
                if (ghe != null) ghes.Add($"{ghe.Hang}{ghe.SoGhe}");
                else ghes.Add("??");
            }

            var doAns = new List<ChiTietDoAnDTO>();
            foreach(var ct in hd.ChiTietDoAns)
            {
                var da = _catalogContext.DoAns.FirstOrDefault(d => d.MaDoAn == ct.MaDoAn);
                doAns.Add(new ChiTietDoAnDTO
                {
                    TenDoAn = da?.TenDoAn ?? "Đồ ăn",
                    SoLuong = ct.SoLuong ?? 0,
                    Gia = ct.Gia ?? 0
                });
            }

            return new HoaDonDTO
            {
                MaHD = hd.MaHd,
                NgayDat = hd.NgayDat,
                TongTien = hd.TongTien,
                TrangThai = hd.TrangThai,
                TenKhachHang = kh?.HoTen,
                TenNhanVien = nv?.HoTen,
                TenPhim = tenPhim,
                DanhSachGhe = ghes,
                LichChieu = lichChieuStr,
                DanhSachDoAn = doAns
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
            return LuuHoaDonInternal(cart, taiKhoan, "Đã thanh toán");
        }

        public int LuuDonChuaThanhToan(CartItemDTO cart, string taiKhoan)
        {
            return LuuHoaDonInternal(cart, taiKhoan, "Chờ thanh toán");
        }

        private int LuuHoaDonInternal(CartItemDTO cart, string taiKhoan, string trangThai)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var khachHang = _identityContext.KhachHangs.FirstOrDefault(kh => kh.TaiKhoan == taiKhoan);
                    if (khachHang == null) return 0;

                    var lichChieu = _catalogContext.LichChieus.Find(cart.MaLich);
                    if (lichChieu == null) return 0;

                    var dsGheTrongPhong = new List<Cinema.Web.Modules.Catalog.Entities.Ghe>();
                    foreach (var tenGhe in cart.DanhSachGhe)
                    {
                        if (tenGhe.Length >= 2)
                        {
                            string hang = tenGhe.Substring(0, 1);
                            if (int.TryParse(tenGhe.Substring(1), out int soGhe))
                            {
                                var ghe = _catalogContext.Ghes.FirstOrDefault(g => g.Hang == hang && g.SoGhe == soGhe && g.MaPhong == lichChieu.MaPhong);
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
                        TrangThai = trangThai
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

                    _publishEndpoint.Publish<InvoiceCreatedEvent>(new
                    {
                        MaHD = hoaDon.MaHd,
                        TaiKhoan = taiKhoan,
                        TongTien = cart.TongTien,
                        TrangThai = trangThai
                    }).GetAwaiter().GetResult();

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

        public bool CapNhatTrangThaiHoaDon(int maHD, string trangThai)
        {
            try
            {
                var hd = _context.HoaDons.Find(maHD);
                if (hd == null) return false;
                hd.TrangThai = trangThai;
                _context.SaveChanges();
                return true;
            }
            catch { return false; }
        }
    }
}
