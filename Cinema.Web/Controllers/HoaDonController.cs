using Cinema.BUS;
using Cinema.DTO;
using Cinema.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cinema.Web.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonBUS _hoaDonBus;
        private readonly IConfiguration _config;

        public HoaDonController(IHoaDonBUS hoaDonBus, IConfiguration config)
        {
            _hoaDonBus = hoaDonBus;
            _config = config;
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

        [HttpPost]
        public IActionResult ThanhToanVnPay()
        {
            var cart = HttpContext.Session.Get<CartItemDTO>("GioHang");
            var userAccount = HttpContext.Session.GetString("UserAccount");

            if (string.IsNullOrEmpty(userAccount))
                return Json(new { success = false, message = "Vui lòng đăng nhập trước khi thanh toán!" });

            if (cart == null || cart.DanhSachGhe == null || !cart.DanhSachGhe.Any())
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống!" });

            // Lưu hóa đơn với trạng thái "Chờ thanh toán"
            int maHD = _hoaDonBus.LuuDonChuaThanhToan(cart, userAccount);

            if (maHD <= 0)
            {
                if (maHD == -1)
                    return Json(new { success = false, message = "Ghế đã bị người khác đặt trước!" });
                return Json(new { success = false, message = "Lưu hóa đơn thất bại!" });
            }

            // Xóa giỏ hàng
            HttpContext.Session.Remove("GioHang");

            // Tạo URL thanh toán VNPay
            string vnpUrl = TaoUrlVnPay(maHD, cart.TongTien, cart.TenPhim);

            return Json(new { success = true, redirectUrl = vnpUrl });
        }

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            var vnPay = new VnPayService();
            foreach (var (key, value) in Request.Query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value!);
                }
            }

            string vnpHashSecret = _config["VnPay:HashSecret"] ?? "";
            string vnpSecureHash = Request.Query["vnp_SecureHash"]!;

            bool isValid = vnPay.ValidateSignature(vnpSecureHash, vnpHashSecret);

            string vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            string vnpTxnRef = vnPay.GetResponseData("vnp_TxnRef");

            if (int.TryParse(vnpTxnRef, out int maHD))
            {
                if (isValid && vnpResponseCode == "00")
                {
                    // Thanh toán thành công
                    _hoaDonBus.CapNhatTrangThaiHoaDon(maHD, "Đã thanh toán");
                    return RedirectToAction("ThanhToanThanhCong", new { id = maHD });
                }
                else
                {
                    // Thanh toán thất bại - hủy hóa đơn
                    _hoaDonBus.CapNhatTrangThaiHoaDon(maHD, "Thanh toán thất bại");
                    ViewBag.Message = "Thanh toán VNPay không thành công. Mã lỗi: " + vnpResponseCode;
                    return View("VnPayFail");
                }
            }

            ViewBag.Message = "Dữ liệu thanh toán không hợp lệ.";
            return View("VnPayFail");
        }

        public IActionResult ThanhToanThanhCong(int id)
        {
            var model = _hoaDonBus.LayChiTietHoaDonFull(id);

            if (model == null) return NotFound();

            return View(model);
        }

        private string TaoUrlVnPay(int maHD, decimal tongTien, string moTa)
        {
            string vnpTmnCode = _config["VnPay:TmnCode"] ?? "";
            string vnpHashSecret = _config["VnPay:HashSecret"] ?? "";
            string vnpUrl = _config["VnPay:BaseUrl"] ?? "";
            string vnpReturnUrl = _config["VnPay:ReturnUrl"] ?? "";

            var vnPay = new VnPayService();

            vnPay.AddRequestData("vnp_Version", "2.1.0");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", vnpTmnCode);
            vnPay.AddRequestData("vnp_Amount", ((long)(tongTien * 100)).ToString()); // VNPay tính theo đơn vị nhỏ nhất (x100)
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1");
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toan ve xem phim: " + moTa);
            vnPay.AddRequestData("vnp_OrderType", "billpayment");
            vnPay.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);
            vnPay.AddRequestData("vnp_TxnRef", maHD.ToString());

            return vnPay.CreateRequestUrl(vnpUrl, vnpHashSecret);
        }
    }
}