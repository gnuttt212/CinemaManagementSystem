using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class NhanVienController : Controller
    {
        private readonly INhanVienBUS _nhanVienBus;

        public NhanVienController(INhanVienBUS nhanVienBus)
        {
            _nhanVienBus = nhanVienBus;
        }

        public IActionResult Index()
        {
            var dsNhanVien = _nhanVienBus.LayDanhSach();
            return View(dsNhanVien);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhanVienDTO dto)
        {
            if (ModelState.IsValid)
            {
                bool result = _nhanVienBus.Them(dto);
                if (result) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Tài khoản đã tồn tại hoặc có lỗi xảy ra.");
            }
            return View(dto);
        }

        public IActionResult Edit(int id)
        {
            var nv = _nhanVienBus.LayTheoMa(id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NhanVienDTO dto)
        {
            if (ModelState.IsValid)
            {
                bool result = _nhanVienBus.Sua(dto);
                if (result) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật.");
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _nhanVienBus.Xoa(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
