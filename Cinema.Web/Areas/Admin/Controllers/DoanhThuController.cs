using Cinema.DAL.Models;
using Cinema.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DoanhThuController : Controller
    {
        private readonly QuanLyRapPhimContext _db;

        public DoanhThuController(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new DoanhThuViewModel
            {
                TongDoanhThu = _db.HoaDons.Sum(h => h.TongTien ?? 0),
                TongHoaDon = _db.HoaDons.Count(),
                TongVeDaBan = _db.Ves.Count(),
                ThongKePhim = _db.Ves
                    .Include(v => v.MaSuatNavigation)
                    .ThenInclude(s => s!.MaPhimNavigation)
                    .Where(v => v.MaSuatNavigation != null && v.MaSuatNavigation.MaPhimNavigation != null)
                    .GroupBy(v => v.MaSuatNavigation!.MaPhimNavigation!.TenPhim)
                    .Select(g => new DoanhThuTheoPhim
                    {
                        TenPhim = g.Key ?? "N/A",
                        DoanhThu = g.Sum(v => v.MaSuatNavigation!.GiaVe ?? 0)
                    }).ToList()
            };

            return View(model);
        }
    }
}
