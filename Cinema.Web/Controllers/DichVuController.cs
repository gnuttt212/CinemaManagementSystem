using Cinema.BUS;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class DichVuController : Controller
    {
        private readonly IDichVuBUS _dichVuBus;
        public DichVuController(IDichVuBUS dichVuBus) { _dichVuBus = dichVuBus; }

       
        public IActionResult Index()
        {
            var data = _dichVuBus.LayDanhSachDichVu();
            return View(data); 
        }
    }
}
