using Cinema.BUS;
using Cinema.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Cinema.DAL.Models;
using System.Linq;

namespace Cinema.Web.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [NhanVienAuthorize]
    public class PhimController : Controller
    {
        private readonly IPhimBUS _phimBus;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly QuanLyRapPhimContext _db;
        public PhimController(IPhimBUS phimBus, IWebHostEnvironment hostEnvironment, QuanLyRapPhimContext db)
        {
            _phimBus = phimBus;
            _hostEnvironment = hostEnvironment;
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
        public async Task<IActionResult> Create(PhimDTO phimDto, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string path = Path.Combine(wwwRootPath, "images/phim", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                phimDto.Poster = fileName;
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
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath, "images/phim", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    phimDto.Poster = fileName;
                }

                bool result = _phimBus.SuaPhim(phimDto);
                if (result) return RedirectToAction(nameof(Index));
            }

            return View(phimDto);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool result = _phimBus.XoaPhim(id);
            if (result) return Json(new { success = true });
            return Json(new { success = false, message = "Không thể xóa phim này vì có dữ liệu liên quan!" });
        }
    }
}
