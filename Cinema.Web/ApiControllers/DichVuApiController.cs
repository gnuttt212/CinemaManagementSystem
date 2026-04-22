using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoAnApiController : ControllerBase
    {
        private readonly QuanLyRapPhimContext _db;

        public DoAnApiController(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DoAnDTO>> GetAll()
        {
            var doAns = _db.DoAns
                .Select(d => new DoAnDTO
                {
                    MaDoAn = d.MaDoAn,
                    TenDoAn = d.TenDoAn,
                    Gia = d.Gia ?? 0,
                    Loai = d.Loai
                })
                .ToList();

            return Ok(doAns);
        }

        [HttpGet("{id}")]
        public ActionResult<DoAnDTO> GetById(int id)
        {
            var doAn = _db.DoAns.Find(id);
            if (doAn == null) return NotFound(new { message = $"Không tìm thấy đồ ăn mã {id}" });

            return Ok(new DoAnDTO
            {
                MaDoAn = doAn.MaDoAn,
                TenDoAn = doAn.TenDoAn,
                Gia = doAn.Gia ?? 0,
                Loai = doAn.Loai
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] DoAnDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TenDoAn))
            {
                return BadRequest(new { message = "Tên đồ ăn không được để trống" });
            }

            var doAn = new DoAn
            {
                TenDoAn = dto.TenDoAn,
                Gia = dto.Gia,
                Loai = dto.Loai
            };

            _db.DoAns.Add(doAn);
            _db.SaveChanges();

            dto.MaDoAn = doAn.MaDoAn;
            return CreatedAtAction(nameof(GetById), new { id = doAn.MaDoAn }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] DoAnDTO dto)
        {
            var doAn = _db.DoAns.Find(id);
            if (doAn == null) return NotFound(new { message = $"Không tìm thấy đồ ăn mã {id}" });

            doAn.TenDoAn = dto.TenDoAn;
            doAn.Gia = dto.Gia;
            doAn.Loai = dto.Loai;

            _db.SaveChanges();
            return Ok(new { message = "Cập nhật đồ ăn thành công", data = dto });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var doAn = _db.DoAns.Find(id);
            if (doAn == null) return NotFound(new { message = $"Không tìm thấy đồ ăn mã {id}" });

            var hasOrders = _db.ChiTietDoAns.Any(ct => ct.MaDoAn == id);
            if (hasOrders)
            {
                return Conflict(new { message = "Không thể xóa vì đồ ăn đã có trong hóa đơn" });
            }

            _db.DoAns.Remove(doAn);
            _db.SaveChanges();
            return Ok(new { message = $"Đã xóa đồ ăn mã {id}" });
        }
    }
}
