using Cinema.BUS;
using Cinema.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cinema.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhimBUS _phimBus;

        public HomeController(ILogger<HomeController> logger, IPhimBUS phimBus)
        {
            _logger = logger;
            _phimBus = phimBus;
        }

        public IActionResult Index()
        {
            
            var dsPhim = _phimBus.LayDanhSachPhimDangChieu();
            return View(dsPhim.Take(8).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}