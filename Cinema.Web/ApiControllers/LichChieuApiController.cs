using Cinema.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichChieuApiController : ControllerBase
    {
        private readonly QuanLyRapPhimContext _db;

        public LichChieuApiController(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        // GET: api/LichChieuApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var lichChieus = _db.LichChieus
                .Include(lc => lc.MaPhimNavigation)
                .Include(lc => lc.MaPhongNavigation)
                .OrderByDescending(lc => lc.NgayChieu)
                .ThenByDescending(lc => lc.GioChieu)
                .Select(lc => new
                {
                    lc.MaLich,
                    lc.MaPhim,
                    TenPhim = lc.MaPhimNavigation != null ? lc.MaPhimNavigation.TenPhim : "",
                    lc.MaPhong,
                    TenPhong = lc.MaPhongNavigation != null ? lc.MaPhongNavigation.TenPhong : "",
                    lc.NgayChieu,
                    GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "",
                    lc.GiaVe
                })
                .ToList();

            return Ok(lichChieus);
        }

        // GET: api/LichChieuApi/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var lc = _db.LichChieus
                .Include(l => l.MaPhimNavigation)
                .Include(l => l.MaPhongNavigation)
                .FirstOrDefault(l => l.MaLich == id);

            if (lc == null)
                return NotFound(new { message = $"Không tìm thấy lịch chiếu mã {id}" });

            return Ok(new
            {
                lc.MaLich,
                lc.MaPhim,
                TenPhim = lc.MaPhimNavigation?.TenPhim ?? "",
                lc.MaPhong,
                TenPhong = lc.MaPhongNavigation?.TenPhong ?? "",
                lc.NgayChieu,
                GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "",
                lc.GiaVe
            });
        }

        // GET: api/LichChieuApi/phim/5
        [HttpGet("phim/{maPhim}")]
        public IActionResult GetByPhim(int maPhim)
        {
            var lichChieus = _db.LichChieus
                .Include(lc => lc.MaPhongNavigation)
                .Where(lc => lc.MaPhim == maPhim && lc.NgayChieu >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(lc => lc.NgayChieu)
                .ThenBy(lc => lc.GioChieu)
                .Select(lc => new
                {
                    lc.MaLich,
                    lc.MaPhim,
                    lc.MaPhong,
                    TenPhong = lc.MaPhongNavigation != null ? lc.MaPhongNavigation.TenPhong : "",
                    lc.NgayChieu,
                    GioChieu = lc.GioChieu.HasValue ? lc.GioChieu.Value.ToString(@"hh\:mm") : "",
                    lc.GiaVe
                })
                .ToList();

            return Ok(lichChieus);
        }

        // POST: api/LichChieuApi
        [HttpPost]
        public IActionResult Create([FromBody] LichChieuCreateRequest req)
        {
            if (req == null || req.MaPhim == null || req.MaPhong == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            // Kiểm tra phim tồn tại
            if (!_db.Phims.Any(p => p.MaPhim == req.MaPhim))
                return BadRequest(new { message = $"Không tìm thấy phim mã {req.MaPhim}" });

            // Kiểm tra phòng tồn tại
            if (!_db.PhongChieus.Any(p => p.MaPhong == req.MaPhong))
                return BadRequest(new { message = $"Không tìm thấy phòng chiếu mã {req.MaPhong}" });

            var lichChieu = new LichChieu
            {
                MaPhim = req.MaPhim,
                MaPhong = req.MaPhong,
                NgayChieu = req.NgayChieu,
                GioChieu = req.GioChieu,
                GiaVe = req.GiaVe
            };

            _db.LichChieus.Add(lichChieu);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = lichChieu.MaLich }, new
            {
                lichChieu.MaLich,
                lichChieu.MaPhim,
                lichChieu.MaPhong,
                lichChieu.NgayChieu,
                GioChieu = lichChieu.GioChieu.HasValue ? lichChieu.GioChieu.Value.ToString(@"hh\:mm") : "",
                lichChieu.GiaVe
            });
        }

        // PUT: api/LichChieuApi/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LichChieuCreateRequest req)
        {
            var lichChieu = _db.LichChieus.Find(id);
            if (lichChieu == null)
                return NotFound(new { message = $"Không tìm thấy lịch chiếu mã {id}" });

            lichChieu.MaPhim = req.MaPhim;
            lichChieu.MaPhong = req.MaPhong;
            lichChieu.NgayChieu = req.NgayChieu;
            lichChieu.GioChieu = req.GioChieu;
            lichChieu.GiaVe = req.GiaVe;

            _db.SaveChanges();

            return Ok(new { message = "Cập nhật lịch chiếu thành công", data = new
            {
                lichChieu.MaLich,
                lichChieu.MaPhim,
                lichChieu.MaPhong,
                lichChieu.NgayChieu,
                GioChieu = lichChieu.GioChieu.HasValue ? lichChieu.GioChieu.Value.ToString(@"hh\:mm") : "",
                lichChieu.GiaVe
            }});
        }

        // DELETE: api/LichChieuApi/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var lichChieu = _db.LichChieus.Find(id);
            if (lichChieu == null)
                return NotFound(new { message = $"Không tìm thấy lịch chiếu mã {id}" });

            // Kiểm tra có chi tiết hóa đơn liên quan không
            var hasOrders = _db.ChiTietHoaDons.Any(ct => ct.MaLich == id);
            if (hasOrders)
            {
                return Conflict(new { message = "Không thể xóa lịch chiếu vì đã có vé được đặt" });
            }

            _db.LichChieus.Remove(lichChieu);
            _db.SaveChanges();

            return Ok(new { message = $"Đã xóa lịch chiếu mã {id}" });
        }
    }

    // Request model for Create/Update
    public class LichChieuCreateRequest
    {
        public int? MaPhim { get; set; }
        public int? MaPhong { get; set; }
        public DateOnly? NgayChieu { get; set; }
        public TimeOnly? GioChieu { get; set; }
        public decimal? GiaVe { get; set; }
    }
}
