using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Booking.Data;
using Cinema.Web.Modules.Identity.Services;
using Cinema.Web.Modules.Catalog.Services;
using Cinema.Web.Modules.Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class DichVuController : Controller
    {
        private readonly IDoAnBUS _doAnBus;
        public DichVuController(IDoAnBUS doAnBus) { _doAnBus = doAnBus; }

        public IActionResult Index()
        {
            var data = _doAnBus.LayDanhSachDoAn();
            return View(data);
        }
    }
}



