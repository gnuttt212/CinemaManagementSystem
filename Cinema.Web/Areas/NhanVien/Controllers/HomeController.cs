using Cinema.Web.Modules.Catalog.Services;
using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Booking.Data;

using Cinema.Web.Modules.Identity.Entities;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Booking.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace Cinema.Web.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [NhanVienAuthorize]
    public class HomeController : Controller
    {
        private readonly CatalogDbContext _context;
        private readonly BookingDbContext _bookingContext;
        private readonly IdentityDbContext _identityContext;
        private readonly ICinemaAdoNetDAL _adoNetDal;

        public HomeController(CatalogDbContext context, BookingDbContext bookingContext, IdentityDbContext identityContext, ICinemaAdoNetDAL adoNetDal)
        {
            _context = context;
            _bookingContext = bookingContext;
            _identityContext = identityContext;
            _adoNetDal = adoNetDal;
        }

        public IActionResult Index()
        {
            ViewBag.TongSoPhim = _context.Phims.Count();
            ViewBag.VeDaBan = _bookingContext.ChiTietHoaDons.Count();
            
            decimal doanhThu = _bookingContext.HoaDons.Sum(h => h.TongTien) ?? 0;
            if (doanhThu >= 1000000)
            {
                ViewBag.DoanhThu = (doanhThu / 1000000).ToString("0.##") + "M";
            }
            else
            {
                ViewBag.DoanhThu = doanhThu.ToString("N0") + "đ";
            }

            ViewBag.KhachHang = _identityContext.KhachHangs.Count();

            var chartData = _adoNetDal.GetDoanhThuTheoPhimChart();
            ViewBag.ChartLabels = JsonSerializer.Serialize(chartData.Keys.ToList());
            ViewBag.ChartValues = JsonSerializer.Serialize(chartData.Values.ToList());

            return View();
        }
    }
}





