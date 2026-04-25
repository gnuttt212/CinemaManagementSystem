using Cinema.BUS;
using Cinema.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Cinema.DTO;

namespace Cinema.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhimBUS _phimBus;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IPhimBUS phimBus, IMemoryCache cache)
        {
            _logger = logger;
            _phimBus = phimBus;
            _cache = cache;
        }

        public IActionResult Index()
        {
            const string cacheKey = "PhimDangChieuList";
            
            if (!_cache.TryGetValue(cacheKey, out List<PhimDTO> dsPhim))
            {
                dsPhim = _phimBus.LayDanhSachPhimDangChieu();
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                    
                _cache.Set(cacheKey, dsPhim, cacheEntryOptions);
            }

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