using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Cinema.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly INguoiDungBUS _nguoiDungBus;

        public AccountController(INguoiDungBUS nguoiDungBus)
        {
            _nguoiDungBus = nguoiDungBus;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserAccount") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginRequest req)
        {
            if (!ModelState.IsValid) return View();

            var user = _nguoiDungBus.LayNguoiDungSauDangNhap(req);

            if (user != null)
            {
                HttpContext.Session.SetString("UserAccount", user.TaiKhoan);
                if (user.IsAdmin)
                {
                    HttpContext.Session.SetString("Role", "Admin");
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    HttpContext.Session.SetString("Role", "Customer");
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest req)
        {
            if (ModelState.IsValid)
            {
                bool result = _nguoiDungBus.DangKy(req);
                if (result)
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Tài khoản đã tồn tại hoặc có lỗi xảy ra!");
            }
            return View(req);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}