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
            var ds = _db.DoAns.OrderByDescending(d => d.MaDoAn).ToList();
            return View(ds);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DoAn model)
        {
            if (ModelState.IsValid)
            {
                _db.DoAns.Add(model);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var doAn = _db.DoAns.Find(id);
            if (doAn == null) return NotFound();
            return View(doAn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DoAn model)
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
            var doAn = _db.DoAns.Find(id);
            if (doAn == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đồ ăn!" });
            }

            try
            {
                var existHD = _db.ChiTietDoAns.Any(c => c.MaDoAn == id);
                if (existHD)
                {
                    return Json(new { success = false, message = "Đồ ăn đã được bán trong hóa đơn, không thể xóa!" });
                }

                _db.DoAns.Remove(doAn);
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
