using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly IKhachHangBUS _khachHangBus;
        private readonly INhanVienBUS _nhanVienBus;

        public NguoiDungController(IKhachHangBUS khachHangBus, INhanVienBUS nhanVienBus)
        {
            _khachHangBus = khachHangBus;
            _nhanVienBus = nhanVienBus;
        }

        public IActionResult StaffProfile()
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(taiKhoan) || (role != "Admin" && role != "NhanVien"))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = _nhanVienBus.LayThongTinProfileNhanVien(taiKhoan);
            if (model == null) return NotFound();

            return View(model);
        }

        public IActionResult Profile()
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(taiKhoan))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = _khachHangBus.LayThongTinProfile(taiKhoan);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var user = _khachHangBus.LayThongTinProfile(username);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(KhachHangDTO model)
        {
            if (ModelState.IsValid)
            {
                bool result = _khachHangBus.CapNhatProfile(model);

                if (result)
                {
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại, vui lòng thử lại!");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(taiKhoan)) return RedirectToAction("Login", "Account");
            return View(new Cinema.DTO.DoiMatKhauRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhau(Cinema.DTO.DoiMatKhauRequest model)
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(taiKhoan)) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                bool result = _khachHangBus.DoiMatKhau(taiKhoan, model.MatKhauHienTai, model.MatKhauMoi);
                if (result)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu hiện tại không chính xác hoặc có lỗi xảy ra.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DoiMatKhauNhanVien()
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(taiKhoan) || (role != "Admin" && role != "NhanVien"))
                return RedirectToAction("Login", "Account");
            return View(new Cinema.DTO.DoiMatKhauRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhauNhanVien(Cinema.DTO.DoiMatKhauRequest model)
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(taiKhoan) || (role != "Admin" && role != "NhanVien"))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                bool result = _nhanVienBus.DoiMatKhau(taiKhoan, model.MatKhauHienTai, model.MatKhauMoi);
                if (result)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("StaffProfile");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu hiện tại không chính xác hoặc có lỗi xảy ra.");
                }
            }
            return View(model);
        }
    }
}
