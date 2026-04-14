using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Cinema.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyRapPhimContext _db;

        public AccountController(QuanLyRapPhimContext db)
        {
            _db = db;
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
        public IActionResult Login(string TaiKhoan, string MatKhau)
        {
            var user = _db.NguoiDungs.FirstOrDefault(u => u.TaiKhoan == TaiKhoan && u.MatKhau == MatKhau);

            if (user != null)
            {
                HttpContext.Session.SetString("UserAccount", user.TaiKhoan);
                if (user.IsAdmin == true)
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
        public IActionResult Register(NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                _db.NguoiDungs.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}