using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public PhimController(IPhimBUS phimBus, IDichVuBUS dichVuBus, IHoaDonBUS hoaDonBus)
        {
            _phimBus = phimBus;
            _dichVuBus = dichVuBus;
            _hoaDonBus = hoaDonBus;
        }

        public IActionResult Index(string date)
        {
            DateTime? selectedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                selectedDate = parsedDate;
            }
            
            var dsPhim = _phimBus.LayDanhSachPhimDangChieu(selectedDate);
            ViewBag.SelectedDate = selectedDate;
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
            var suatChieu = _phimBus.LaySuatChieu(maSuat);
            if (suatChieu == null) return NotFound();

            var danhSachGhe = _phimBus.LayDanhSachGheTheoPhong(suatChieu.MaPhong ?? 0);
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
                var suatChieu = _phimBus.LaySuatChieuChiTiet(maSuat);

                if (suatChieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy suất chiếu hợp lệ!" });
                }

                foreach (var tenGhe in ghes)
                {
                    var gheInfo = _phimBus.LayGheTheoTenVaPhong(tenGhe, suatChieu.MaPhong ?? 0);

                    if (gheInfo != null)
                    {
                        if (_hoaDonBus.KiemTraGheDaDat(maSuat, gheInfo.MaGhe))
                        {
                            return Json(new { success = false, message = $"Ghế {tenGhe} vừa có người khác đặt. Vui lòng chọn ghế khác!" });
                        }
                    }
                }

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
                        var dvInfo = _phimBus.LayDichVu(d.MaDV);
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