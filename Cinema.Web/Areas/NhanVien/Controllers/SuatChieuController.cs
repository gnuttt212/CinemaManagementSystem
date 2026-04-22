using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cinema.Web.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [NhanVienAuthorize]
    public class SuatChieuController : Controller
    {
        private readonly QuanLyRapPhimContext _db;

        public SuatChieuController(QuanLyRapPhimContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var ds = _db.LichChieus
                .Include(lc => lc.MaPhimNavigation)
                .Include(lc => lc.MaPhongNavigation)
                .OrderByDescending(lc => lc.NgayChieu)
                .ThenByDescending(lc => lc.GioChieu)
                .ToList();
            return View(ds);
        }

        public IActionResult Create()
        {
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.PhongChieus.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LichChieu lc)
        {
            ModelState.Remove("MaPhimNavigation");
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("ChiTietHoaDons");

            if (ModelState.IsValid)
            {
                _db.LichChieus.Add(lc);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.PhongChieus.ToList();
            return View(lc);
        }
        public IActionResult Edit(int id)
        {
            var lichChieu = _db.LichChieus.Find(id);
            if (lichChieu == null) return NotFound();

            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.PhongChieus.ToList();
            return View(lichChieu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(LichChieu lc)
        {
            ModelState.Remove("MaPhimNavigation");
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("ChiTietHoaDons");

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(lc);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.LichChieus.Any(e => e.MaLich == lc.MaLich)) return NotFound();
                    else throw;
                }
            }
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.PhongChieus.ToList();
            return View(lc);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var lichChieu = _db.LichChieus.Find(id);
            if (lichChieu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch chiếu!" });
            }

            try
            {
                _db.LichChieus.Remove(lichChieu);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
