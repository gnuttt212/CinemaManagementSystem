using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Web.Controllers
{
    public class PhimController : Controller
    {
        private readonly IPhimBUS _phimBus;
        private readonly IDichVuBUS _dichVuBus;
        private readonly IHoaDonBUS _hoaDonBus; 
        private readonly QuanLyRapPhimContext _db;

        public PhimController(IPhimBUS phimBus, IDichVuBUS dichVuBus, IHoaDonBUS hoaDonBus, QuanLyRapPhimContext db)
        {
            _phimBus = phimBus;
            _dichVuBus = dichVuBus;
            _hoaDonBus = hoaDonBus; 
            _db = db;
        }

        public IActionResult Index()
        {
            var dsPhim = _phimBus.LayDanhSachPhimDangChieu();
            return View(dsPhim);
        }

        public IActionResult Search(string query)
        {
            var dsPhim = _phimBus.LayDanhSachPhimDangChieu();
            if (!string.IsNullOrEmpty(query))
            {
                dsPhim = dsPhim.Where(p => p.TenPhim.ToLower().Contains(query.ToLower())).ToList();
            }
            ViewBag.Query = query;
            return View("Index", dsPhim);
        }

        public ActionResult Details(int id)
        {
            var phim = _phimBus.LayChiTietPhim(id);
            if (phim == null) return NotFound();
            return View(phim);
        }
        public IActionResult ChonGhe(int maSuat)
        {
            var suatChieu = _db.SuatChieus.FirstOrDefault(s => s.MaSuat == maSuat);
            if (suatChieu == null) return NotFound();

            var danhSachGhe = _db.Ghes
                .Where(g => g.MaPhong == suatChieu.MaPhong)
                .OrderBy(g => g.TenGhe)
                .ToList();
            var gheDaDat = _hoaDonBus.LayDanhSachMaGheDaDat(maSuat);

            ViewBag.DichVus = _dichVuBus.LayDanhSachDichVu();
            ViewBag.MaSuat = maSuat;
            ViewBag.GiaVeGoc = suatChieu.GiaVe ?? 0;
            ViewBag.GheDaDat = gheDaDat; 

            return View(danhSachGhe);
        }
        [HttpPost]
        public IActionResult LuuGheVaoSession(List<string> ghes, int maSuat, List<DichVuChonDTO> dichVus)
        {
            try
            {
                var suatChieu = _db.SuatChieus
                    .Include(s => s.MaPhimNavigation)
                    .FirstOrDefault(s => s.MaSuat == maSuat);

                if (suatChieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy suất chiếu hợp lệ!" });
                }

                foreach (var tenGhe in ghes)
                {
                    var gheInfo = _db.Ghes.FirstOrDefault(g => g.TenGhe == tenGhe &&
                                  g.MaPhong == suatChieu.MaPhong);

                    if (gheInfo != null)
                    {
                        if (_hoaDonBus.KiemTraGheDaDat(maSuat, gheInfo.MaGhe))
                        {
                            return Json(new { success = false, message = $"Ghế {tenGhe} vừa có người khác đặt. Vui lòng chọn ghế khác!" });
                        }
                    }
                }

                if (suatChieu == null)
                    return Json(new { success = false, message = "Suất chiếu không tồn tại!" });

                var cart = HttpContext.Session.Get<CartItemDTO>("GioHang") ?? new CartItemDTO();

                cart.MaPhim = suatChieu.MaPhim ?? 0;
                cart.TenPhim = suatChieu.MaPhimNavigation?.TenPhim ?? "Phim không xác định";
                cart.HinhAnh = suatChieu.MaPhimNavigation?.Hinh;
                cart.MaSuat = maSuat;
                cart.DanhSachGhe = ghes;
                cart.TongTienPhim = ghes.Count * (suatChieu.GiaVe ?? 0);

                if (dichVus != null && dichVus.Any())
                {
                    cart.DichVus = dichVus.Select(d => {
                        var dvInfo = _db.DichVus.Find(d.MaDV);
                        return new DichVuDTO
                        {
                            MaDV = d.MaDV,
                            TenDV = dvInfo?.TenDv ?? "Dịch vụ",
                            DonGia = dvInfo?.DonGia ?? 0,
                            SoLuong = d.SoLuong
                        };
                    }).ToList();
                }
                else
                {
                    cart.DichVus = new List<DichVuDTO>();
                }

                HttpContext.Session.Set("GioHang", cart);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}