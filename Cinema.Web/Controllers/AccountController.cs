using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult ResetData([FromServices] Cinema.DAL.Models.QuanLyRapPhimContext _db)
        {
            try
            {
                
                _db.Database.ExecuteSqlRaw("DELETE FROM ChiTietHoaDon");
                _db.Database.ExecuteSqlRaw("DELETE FROM ChiTietDoAn");
                _db.Database.ExecuteSqlRaw("DELETE FROM HoaDon");
                _db.Database.ExecuteSqlRaw("DELETE FROM KhachHang");
                _db.Database.ExecuteSqlRaw("DELETE FROM NhanVien");

                string defaultPassword = BCrypt.Net.BCrypt.HashPassword("123456");

                
                _db.NhanViens.Add(new Cinema.DAL.Models.NhanVien
                {
                    TaiKhoan = "admin",
                    MatKhau = defaultPassword,
                    HoTen = "Quản Trị Hệ Thống",
                    ChucVu = "Giám Đốc",
                    PhanQuyen = "Admin"
                });

                
                _db.NhanViens.Add(new Cinema.DAL.Models.NhanVien
                {
                    TaiKhoan = "staff",
                    MatKhau = defaultPassword,
                    HoTen = "Nhân Viên Bán Vé",
                    ChucVu = "Nhân Viên",
                    PhanQuyen = "NhanVien"
                });

                
                _db.KhachHangs.Add(new Cinema.DAL.Models.KhachHang
                {
                    TaiKhoan = "user",
                    MatKhau = defaultPassword,
                    HoTen = "Khách Hàng Mẫu",
                    Email = "user@example.com",
                    Sdt = "0987654321",
                    DiemTichLuy = 100
                });

                _db.SaveChanges();
                return Content("Đã reset và tạo lại dữ liệu tài khoản thành công! Mật khẩu mặc định: 123456");
            }
            catch (Exception ex)
            {
                return Content("Có lỗi xảy ra (có thể do ràng buộc khóa ngoại): " + ex.Message);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}