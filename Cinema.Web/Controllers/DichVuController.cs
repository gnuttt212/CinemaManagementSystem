using Cinema.BUS;
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
