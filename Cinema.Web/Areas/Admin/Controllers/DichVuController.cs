using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DichVuController : Controller
    {
        private readonly QuanLyRapPhimContext _db;

        public DichVuController(QuanLyRapPhimContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var ds = _db.DichVus.OrderByDescending(d => d.MaDv).ToList();
            return View(ds);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DichVu model)
        {
            if (ModelState.IsValid)
            {
                _db.DichVus.Add(model);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var dichVu = _db.DichVus.Find(id);
            if (dichVu == null) return NotFound();
            return View(dichVu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DichVu model)
        {
            if (ModelState.IsValid)
            {
                _db.Update(model);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dichVu = _db.DichVus.Find(id);
            if (dichVu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dịch vụ!" });
            }

            try
            {
                var existHD = _db.ChiTietDvs.Any(c => c.MaDv == id);
                if (existHD)
                {
                    return Json(new { success = false, message = "Dịch vụ đã được bán trong hóa đơn, không thể xóa!" });
                }

                _db.DichVus.Remove(dichVu);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
