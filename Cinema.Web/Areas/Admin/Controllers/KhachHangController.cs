using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class KhachHangController : Controller
    {
        private readonly IKhachHangBUS _khachHangBus;

        public KhachHangController(IKhachHangBUS khachHangBus)
        {
            _khachHangBus = khachHangBus;
        }

        public IActionResult Index()
        {
            var dsKhachHang = _khachHangBus.LayDanhSach();
            return View(dsKhachHang);
        }

        public IActionResult Edit(int id)
        {
            var kh = _khachHangBus.LayTheoMa(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KhachHangDTO dto)
        {
            // We just update info, not password
            // ModelState might fail on some lists, but we can clear them if needed
            ModelState.Remove("LichSuHoaDon");
            
            if (ModelState.IsValid)
            {
                bool result = _khachHangBus.Sua(dto);
                if (result) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật.");
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _khachHangBus.Xoa(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
