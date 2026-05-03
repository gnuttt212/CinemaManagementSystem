using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class KhuyenMaiController : Controller
    {
        private readonly IKhuyenMaiBUS _khuyenMaiBUS;

        public KhuyenMaiController(IKhuyenMaiBUS khuyenMaiBUS)
        {
            _khuyenMaiBUS = khuyenMaiBUS;
        }

        public IActionResult Index()
        {
            var ds = _khuyenMaiBUS.LayDanhSachKhuyenMai();
            return View(ds);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhuyenMaiDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.TenKM))
            {
                ModelState.AddModelError("TenKM", "Tên khuyến mãi không được để trống.");
            }

            if (model.PhanTramGiam <= 0 || model.PhanTramGiam > 100)
            {
                ModelState.AddModelError("PhanTramGiam", "Phần trăm giảm phải từ 1 đến 100.");
            }

            if (model.NgayBatDau.HasValue && model.NgayKetThuc.HasValue
                && model.NgayKetThuc < model.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _khuyenMaiBUS.ThemKhuyenMai(model);
            if (result > 0)
            {
                TempData["Success"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Có lỗi xảy ra khi thêm khuyến mãi.";
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var km = _khuyenMaiBUS.LayTheoId(id);
            if (km == null) return NotFound();
            return View(km);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KhuyenMaiDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.TenKM))
            {
                ModelState.AddModelError("TenKM", "Tên khuyến mãi không được để trống.");
            }

            if (model.PhanTramGiam <= 0 || model.PhanTramGiam > 100)
            {
                ModelState.AddModelError("PhanTramGiam", "Phần trăm giảm phải từ 1 đến 100.");
            }

            if (model.NgayBatDau.HasValue && model.NgayKetThuc.HasValue
                && model.NgayKetThuc < model.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _khuyenMaiBUS.SuaKhuyenMai(model);
            if (result)
            {
                TempData["Success"] = "Cập nhật khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Có lỗi xảy ra khi cập nhật khuyến mãi.";
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _khuyenMaiBUS.XoaKhuyenMai(id);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy khuyến mãi hoặc không thể xóa!" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
