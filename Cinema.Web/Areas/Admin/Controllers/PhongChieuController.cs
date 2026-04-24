using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhongChieuController : Controller
    {
        private readonly IPhongChieuBUS _phongChieuBus;

        public PhongChieuController(IPhongChieuBUS phongChieuBus)
        {
            _phongChieuBus = phongChieuBus;
        }

        public IActionResult Index()
        {
            var data = _phongChieuBus.LayDanhSachPhong();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhongChieuDTO dto)
        {
            if (ModelState.IsValid)
            {
                var success = _phongChieuBus.ThemPhong(dto);
                if (success)
                {
                    TempData["Success"] = "Thêm phòng chiếu và sinh ghế tự động thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Lỗi khi thêm phòng chiếu. Hãy kiểm tra lại.");
            }
            return View(dto);
        }

        public IActionResult Edit(int id)
        {
            var p = _phongChieuBus.LayChiTietPhong(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PhongChieuDTO dto)
        {
            if (ModelState.IsValid)
            {
                var error = _phongChieuBus.SuaPhong(dto);
                if (string.IsNullOrEmpty(error))
                {
                    TempData["Success"] = "Cập nhật phòng chiếu thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", error);
            }
            return View(dto);
        }

        public IActionResult Delete(int id)
        {
            var p = _phongChieuBus.LayChiTietPhong(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var error = _phongChieuBus.XoaPhong(id);
            if (string.IsNullOrEmpty(error))
            {
                TempData["Success"] = "Xóa phòng chiếu thành công!";
            }
            else
            {
                TempData["Error"] = error;
            }
            return RedirectToAction("Index");
        }
    }
}
