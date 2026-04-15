using Cinema.BUS;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly INguoiDungBUS _nguoiDungBus;

        public NguoiDungController(INguoiDungBUS nguoiDungBus)
        {
            _nguoiDungBus = nguoiDungBus;
        }

        public IActionResult Profile()
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(taiKhoan))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = _nguoiDungBus.LayThongTinProfile(taiKhoan);
            if (model == null) return NotFound();

            return View(model);
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var user = _nguoiDungBus.LayThongTinProfile(username); ;
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        public IActionResult EditProfile(NguoiDungDTO model)
        {
            if (ModelState.IsValid)
            {
                bool result = _nguoiDungBus.CapNhatProfile(model);

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
        public IActionResult DoiMatKhau(Cinema.DTO.DoiMatKhauRequest model)
        {
            var taiKhoan = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(taiKhoan)) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                bool result = _nguoiDungBus.DoiMatKhau(taiKhoan, model.MatKhauHienTai, model.MatKhauMoi);
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
    }
}
