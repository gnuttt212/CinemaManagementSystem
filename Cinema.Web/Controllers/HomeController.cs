using Cinema.BUS;
using Cinema.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Text.Json;
using Cinema.DTO;

namespace Cinema.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhimBUS _phimBus;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, IPhimBUS phimBus, IDistributedCache cache)
        {
            _logger = logger;
            _phimBus = phimBus;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            const string cacheKey = "PhimDangChieuList";
            List<PhimDTO>? dsPhim = null;

            var cachedJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                dsPhim = JsonSerializer.Deserialize<List<PhimDTO>>(cachedJson);
            }

            if (dsPhim == null)
            {
                dsPhim = _phimBus.LayDanhSachPhimDangChieu();

                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(dsPhim),
                    options);
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