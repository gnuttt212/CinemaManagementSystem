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
        private readonly IDoAnBUS _doAnBus;
        private readonly IHoaDonBUS _hoaDonBus;

        public PhimController(IPhimBUS phimBus, IDoAnBUS doAnBus, IHoaDonBUS hoaDonBus)
        {
            _phimBus = phimBus;
            _doAnBus = doAnBus;
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

        public IActionResult ChonGhe(int maLich)
        {
            var lichChieu = _phimBus.LayLichChieu(maLich);
            if (lichChieu == null) return NotFound();

            var danhSachGhe = _phimBus.LayDanhSachGheTheoPhong(lichChieu.MaPhong ?? 0);
            var gheDaDat = _hoaDonBus.LayDanhSachMaGheDaDat(maLich);

            ViewBag.DoAns = _doAnBus.LayDanhSachDoAn();
            ViewBag.MaLich = maLich;
            ViewBag.GiaVeGoc = lichChieu.GiaVe ?? 0;
            ViewBag.GheDaDat = gheDaDat;

            return View(danhSachGhe);
        }

        [HttpPost]
        public IActionResult LuuGheVaoSession(List<string> ghes, int maLich, List<DoAnChonDTO> doAns)
        {
            try
            {
                var lichChieu = _phimBus.LayLichChieuChiTiet(maLich);

                if (lichChieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch chiếu hợp lệ!" });
                }

                foreach (var tenGhe in ghes)
                {
                    if (tenGhe.Length >= 2)
                    {
                        string hang = tenGhe.Substring(0, 1);
                        if (int.TryParse(tenGhe.Substring(1), out int soGhe))
                        {
                            var gheInfo = _phimBus.LayGheTheoHangSoVaPhong(hang, soGhe, lichChieu.MaPhong ?? 0);
                            if (gheInfo != null)
                            {
                                if (_hoaDonBus.KiemTraGheDaDat(maLich, gheInfo.MaGhe))
                                {
                                    return Json(new { success = false, message = $"Ghế {tenGhe} vừa có người khác đặt. Vui lòng chọn ghế khác!" });
                                }
                            }
                        }
                    }
                }

                var cart = HttpContext.Session.Get<CartItemDTO>("GioHang") ?? new CartItemDTO();

                cart.MaPhim = lichChieu.MaPhim ?? 0;
                cart.TenPhim = lichChieu.MaPhimNavigation?.TenPhim ?? "Phim không xác định";
                cart.Poster = lichChieu.MaPhimNavigation?.Poster;
                cart.MaLich = maLich;
                cart.DanhSachGhe = ghes;
                cart.TongTienPhim = ghes.Count * (lichChieu.GiaVe ?? 0);

                if (doAns != null && doAns.Any())
                {
                    cart.DoAns = doAns.Select(d => {
                        var doAnInfo = _phimBus.LayDoAn(d.MaDoAn);
                        return new DoAnDTO
                        {
                            MaDoAn = d.MaDoAn,
                            TenDoAn = doAnInfo?.TenDoAn ?? "Đồ ăn",
                            Gia = doAnInfo?.Gia ?? 0,
                            SoLuong = d.SoLuong
                        };
                    }).ToList();
                }
                else
                {
                    cart.DoAns = new List<DoAnDTO>();
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

    /// <summary>
    /// DTO đơn giản cho việc chọn đồ ăn từ form
    /// </summary>
    public class DoAnChonDTO
    {
        public int MaDoAn { get; set; }
        public int SoLuong { get; set; }
    }
}