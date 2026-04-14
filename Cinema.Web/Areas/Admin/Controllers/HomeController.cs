using Cinema.DAL.AdoNet;
using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace Cinema.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly QuanLyRapPhimContext _context;
        private readonly ICinemaAdoNetDAL _adoNetDal;

        public HomeController(QuanLyRapPhimContext context, ICinemaAdoNetDAL adoNetDal)
        {
            _context = context;
            _adoNetDal = adoNetDal;
        }

        public IActionResult Index()
        {
            ViewBag.TongSoPhim = _context.Phims.Count();
            ViewBag.VeDaBan = _context.Ves.Count();
            
            decimal doanhThu = _context.HoaDons.Sum(h => h.TongTien) ?? 0;
            if (doanhThu >= 1000000)
            {
                ViewBag.DoanhThu = (doanhThu / 1000000).ToString("0.##") + "M";
            }
            else
            {
                ViewBag.DoanhThu = doanhThu.ToString("N0") + "đ";
            }

            ViewBag.KhachHang = _context.NguoiDungs.Count();

            var chartData = _adoNetDal.GetDoanhThuTheoPhimChart();
            ViewBag.ChartLabels = JsonSerializer.Serialize(chartData.Keys.ToList());
            ViewBag.ChartValues = JsonSerializer.Serialize(chartData.Values.ToList());

            return View();
        }
    }
}
