using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SuatChieuController : Controller
    {
        private readonly QuanLyRapPhimContext _db;

        public SuatChieuController(QuanLyRapPhimContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var ds = _db.SuatChieus
                .Include(s => s.MaPhimNavigation)
                .Include(s => s.MaPhongNavigation)
                .OrderByDescending(s => s.NgayChieu)
                .ThenByDescending(s => s.GioBatDau)
                .ToList();
            return View(ds);
        }

        public IActionResult Create()
        {
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.Phongs.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SuatChieu sc)
        {
            if (ModelState.IsValid)
            {
                var phim = _db.Phims.Find(sc.MaPhim);
                if (phim != null && phim.ThoiLuong.HasValue && sc.GioBatDau.HasValue)
                {
                    sc.GioKetThuc = sc.GioBatDau.Value.AddMinutes(phim.ThoiLuong.Value);
                }
                if (string.IsNullOrEmpty(sc.TrangThai)) sc.TrangThai = "Sắp chiếu";

                _db.SuatChieus.Add(sc);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.Phongs.ToList();
            return View(sc);
        }
        public IActionResult Edit(int id)
        {
            var suatChieu = _db.SuatChieus.Find(id);
            if (suatChieu == null) return NotFound();

            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.Phongs.ToList();
            return View(suatChieu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SuatChieu sc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var phim = _db.Phims.Find(sc.MaPhim);
                    if (phim != null && phim.ThoiLuong.HasValue && sc.GioBatDau.HasValue)
                    {
                        sc.GioKetThuc = sc.GioBatDau.Value.AddMinutes(phim.ThoiLuong.Value);
                    }
                    if (string.IsNullOrEmpty(sc.TrangThai)) sc.TrangThai = "Sắp chiếu";

                    _db.Update(sc);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.SuatChieus.Any(e => e.MaSuat == sc.MaSuat)) return NotFound();
                    else throw;
                }
            }
            ViewBag.MaPhim = _db.Phims.ToList();
            ViewBag.MaPhong = _db.Phongs.ToList();
            return View(sc);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var suatChieu = _db.SuatChieus.Find(id);
            if (suatChieu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy suất chiếu!" });
            }

            try
            {
                _db.SuatChieus.Remove(suatChieu);
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