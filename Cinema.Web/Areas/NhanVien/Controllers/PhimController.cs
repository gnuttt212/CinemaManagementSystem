using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Booking.Data;
using Cinema.Web.Modules.Identity.Services;
using Cinema.Web.Modules.Catalog.Services;
using Cinema.Web.Modules.Booking.Services;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Cinema.Web.Modules.Identity.Entities;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Booking.Entities;
using Cinema.Web.Services;
using System.Linq;

namespace Cinema.Web.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [NhanVienAuthorize]
    public class PhimController : Controller
    {
        private readonly IPhimBUS _phimBus;
        private readonly IPosterStorageService _posterStorage;
        private readonly CatalogDbContext _db;
        public PhimController(IPhimBUS phimBus, IPosterStorageService posterStorage, CatalogDbContext db)
        {
            _phimBus = phimBus;
            _posterStorage = posterStorage;
            _db = db;
        }
        public IActionResult Index()
        {
            var ds = _phimBus.LayDanhSachPhim();
            return View(ds);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhimDTO phimDto, IFormFile? ImageFile)
        {
            if (ImageFile != null)
            {
                phimDto.Poster = await _posterStorage.UploadAsync(ImageFile);
            }
            else
            {
                phimDto.Poster = "no-image.jpg";
            }

            int newId = _phimBus.ThemPhim(phimDto);

            if (newId > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(phimDto);
        }
        public IActionResult Edit(int id)
        {
            var phimDto = _phimBus.LayChiTietPhim(id);
            if (phimDto == null) return NotFound();

            return View(phimDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhimDTO phimDto, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    phimDto.Poster = await _posterStorage.UploadAsync(ImageFile);
                }

                bool result = _phimBus.SuaPhim(phimDto);
                if (result) return RedirectToAction(nameof(Index));
            }

            return View(phimDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            bool result = _phimBus.XoaPhim(id);
            if (result) return Json(new { success = true });
            return Json(new { success = false, message = "Không thể xóa phim này vì có dữ liệu liên quan!" });
        }
    }
}



