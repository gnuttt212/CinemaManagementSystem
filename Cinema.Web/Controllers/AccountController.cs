using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace Cinema.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IKhachHangBUS _khachHangBus;
        private readonly INhanVienBUS _nhanVienBus;

        public AccountController(IKhachHangBUS khachHangBus, INhanVienBUS nhanVienBus)
        {
            _khachHangBus = khachHangBus;
            _nhanVienBus = nhanVienBus;
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
        public IActionResult Login(KhachHangLoginRequest req)
        {
            if (!ModelState.IsValid) return View();

            
            var khachHang = _khachHangBus.LayKhachHangSauDangNhap(req);
            if (khachHang != null)
            {
                HttpContext.Session.SetString("UserAccount", khachHang.TaiKhoan);
                HttpContext.Session.SetString("Role", "Customer");
                return RedirectToAction("Index", "Home");
            }

            
            var nvReq = new NhanVienLoginRequest { TaiKhoan = req.TaiKhoan, MatKhau = req.MatKhau };
            var nhanVien = _nhanVienBus.LayNhanVienSauDangNhap(nvReq);
            if (nhanVien != null)
            {
                string role = nhanVien.PhanQuyen == "Admin" ? "Admin" : "NhanVien";
                HttpContext.Session.SetString("UserAccount", nhanVien.TaiKhoan);
                HttpContext.Session.SetString("Role", role);
                
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "NhanVien" });
                }
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    var khachHang = _khachHangBus.DangNhapGoogle(email, name ?? email);

                    if (khachHang != null)
                    {
                        HttpContext.Session.SetString("UserAccount", khachHang.TaiKhoan);
                        HttpContext.Session.SetString("Role", "Customer");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(KhachHangRegisterRequest req)
        {
            if (ModelState.IsValid)
            {
                bool result = _khachHangBus.DangKy(req);
                if (result)
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Tài khoản đã tồn tại hoặc có lỗi xảy ra!");
            }
            return View(req);
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}