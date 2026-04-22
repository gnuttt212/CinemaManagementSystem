using Cinema.DAL.Models;
using Cinema.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Cinema.Web.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [NhanVienAuthorize]
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
                TongVeDaBan = _db.ChiTietHoaDons.Count(),
                ThongKePhim = _db.ChiTietHoaDons
                    .Include(ct => ct.MaLichNavigation)
                    .ThenInclude(lc => lc!.MaPhimNavigation)
                    .Where(ct => ct.MaLichNavigation != null && ct.MaLichNavigation.MaPhimNavigation != null)
                    .GroupBy(ct => ct.MaLichNavigation!.MaPhimNavigation!.TenPhim)
                    .Select(g => new DoanhThuTheoPhim
                    {
                        TenPhim = g.Key ?? "N/A",
                        DoanhThu = g.Sum(ct => ct.GiaVe ?? 0)
                    }).ToList()
            };

            return View(model);
        }
    }
}

