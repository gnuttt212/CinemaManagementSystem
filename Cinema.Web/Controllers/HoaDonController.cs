using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cinema.Web.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonBUS _hoaDonBus;

        public HoaDonController(IHoaDonBUS hoaDonBus)
        {
            _hoaDonBus = hoaDonBus;
        }
        [HttpGet]
        public IActionResult GioHang()
        {
            var cart = HttpContext.Session.Get<CartItemDTO>("GioHang");

            if (cart == null)
            {
                return RedirectToAction("Index", "Phim");
            }
            return View(cart);
        }
        [HttpPost]
        public IActionResult XacNhanThanhToan()
        {
            var cart = HttpContext.Session.Get<CartItemDTO>("GioHang");
            var userAccount = HttpContext.Session.GetString("UserAccount");

            if (string.IsNullOrEmpty(userAccount))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập trước khi thanh toán!" });
            }

            if (cart == null || cart.DanhSachGhe == null || !cart.DanhSachGhe.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống!" });
            }
            int maHDMoi = _hoaDonBus.LuuVaThanhToan(cart, userAccount);

            if (maHDMoi > 0)
            {
                HttpContext.Session.Remove("GioHang");

                return Json(new
                {
                    success = true,
                    message = "Thanh toán thành công!",
                    maHD = maHDMoi
                });
            }

            if (maHDMoi == -1)
            {
                return Json(new { success = false, message = "Ghế đã bị người khác đặt trước. Vui lòng chọn ghế khác!" });
            }

            return Json(new { success = false, message = "Lưu hóa đơn thất bại. Vui lòng thử lại sau!" });
        }
        public IActionResult ThanhToanThanhCong(int id)
        {
            var model = _hoaDonBus.LayChiTietHoaDonFull(id);

            if (model == null) return NotFound();

            return View(model);
        }
    }
}