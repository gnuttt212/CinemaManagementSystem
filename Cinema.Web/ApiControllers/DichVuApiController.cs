using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Web.ApiControllers
{
    /// <summary>
    /// RESTful Web API cho Dịch Vụ (Combo/Bắp nước)
    /// Đáp ứng yêu cầu: Web API có khả năng thao tác với CSDL + JSON
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuApiController : ControllerBase
    {
        private readonly QuanLyRapPhimContext _db;

        public DichVuApiController(QuanLyRapPhimContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET: api/dichvuapi
        /// Lấy toàn bộ danh sách dịch vụ (LINQ to Entities)
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<DichVuDTO>> GetAll()
        {
            var dichVus = _db.DichVus
                .Select(d => new DichVuDTO
                {
                    MaDV = d.MaDv,
                    TenDV = d.TenDv ?? "",
                    DonGia = d.DonGia ?? 0,
                    SoLuongTon = d.SoLuongTon ?? 0
                })
                .ToList();

            return Ok(dichVus);
        }

        /// <summary>
        /// GET: api/dichvuapi/5
        /// Lấy chi tiết 1 dịch vụ theo ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<DichVuDTO> GetById(int id)
        {
            var dv = _db.DichVus.Find(id);
            if (dv == null) return NotFound(new { message = $"Không tìm thấy dịch vụ mã {id}" });

            return Ok(new DichVuDTO
            {
                MaDV = dv.MaDv,
                TenDV = dv.TenDv ?? "",
                DonGia = dv.DonGia ?? 0,
                SoLuongTon = dv.SoLuongTon ?? 0
            });
        }

        /// <summary>
        /// POST: api/dichvuapi
        /// Thêm dịch vụ mới (RESTful Create)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] DichVuDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TenDV))
            {
                return BadRequest(new { message = "Tên dịch vụ không được để trống" });
            }

            var dichVu = new DichVu
            {
                TenDv = dto.TenDV,
                DonGia = dto.DonGia,
                SoLuongTon = dto.SoLuongTon ?? 0
            };

            _db.DichVus.Add(dichVu);
            _db.SaveChanges();

            dto.MaDV = dichVu.MaDv;
            return CreatedAtAction(nameof(GetById), new { id = dichVu.MaDv }, dto);
        }

        /// <summary>
        /// PUT: api/dichvuapi/5
        /// Cập nhật dịch vụ (RESTful Update)
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] DichVuDTO dto)
        {
            var dichVu = _db.DichVus.Find(id);
            if (dichVu == null) return NotFound(new { message = $"Không tìm thấy dịch vụ mã {id}" });

            dichVu.TenDv = dto.TenDV;
            dichVu.DonGia = dto.DonGia;
            dichVu.SoLuongTon = dto.SoLuongTon ?? 0;

            _db.SaveChanges();
            return Ok(new { message = "Cập nhật dịch vụ thành công", data = dto });
        }

        /// <summary>
        /// DELETE: api/dichvuapi/5
        /// Xóa dịch vụ (RESTful Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dichVu = _db.DichVus.Find(id);
            if (dichVu == null) return NotFound(new { message = $"Không tìm thấy dịch vụ mã {id}" });

            // Kiểm tra ràng buộc dữ liệu
            var hasOrders = _db.ChiTietDvs.Any(ct => ct.MaDv == id);
            if (hasOrders)
            {
                return Conflict(new { message = "Không thể xóa vì dịch vụ đã có trong hóa đơn" });
            }

            _db.DichVus.Remove(dichVu);
            _db.SaveChanges();
            return Ok(new { message = $"Đã xóa dịch vụ mã {id}" });
        }
    }
}
