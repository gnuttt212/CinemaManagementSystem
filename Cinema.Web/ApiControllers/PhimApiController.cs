using Cinema.BUS;
using Cinema.DAL.AdoNet;
using Cinema.DAL.Models;
using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Cinema.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimApiController : ControllerBase
    {
        private readonly IPhimBUS _phimBus;
        private readonly ICinemaAdoNetDAL _adoNetDal;
        private readonly QuanLyRapPhimContext _db;

        public PhimApiController(IPhimBUS phimBus, ICinemaAdoNetDAL adoNetDal, QuanLyRapPhimContext db)
        {
            _phimBus = phimBus;
            _adoNetDal = adoNetDal;
            _db = db;
        }

        // ========================================================
        // PHẦN 1: RESTful CRUD API (EF Core + LINQ)
        // ========================================================

        /// <summary>
        /// GET: api/phimapi
        /// Lấy toàn bộ danh sách phim (EF Core + LINQ)
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<PhimDTO>> GetPhims()
        {
            try
            {
                var phims = _phimBus.LayDanhSachPhimDangChieu_SP();
                return Ok(phims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/phimapi/5
        /// Lấy chi tiết 1 phim theo ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<PhimDTO> GetPhim(int id)
        {
            var phim = _phimBus.LayChiTietPhim(id);
            if (phim == null)
            {
                return NotFound(new { message = $"Không tìm thấy phim với mã {id}" });
            }
            return Ok(phim);
        }

        /// <summary>
        /// POST: api/phimapi
        /// Thêm phim mới (RESTful Create)
        /// </summary>
        [HttpPost]
        public ActionResult<PhimDTO> CreatePhim([FromBody] PhimDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TenPhim))
            {
                return BadRequest(new { message = "Dữ liệu phim không hợp lệ" });
            }

            int newId = _phimBus.ThemPhim(dto);
            if (newId > 0)
            {
                dto.MaPhim = newId;
                return CreatedAtAction(nameof(GetPhim), new { id = newId }, dto);
            }
            return StatusCode(500, new { message = "Không thể thêm phim" });
        }

        /// <summary>
        /// PUT: api/phimapi/5
        /// Cập nhật phim (RESTful Update)
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdatePhim(int id, [FromBody] PhimDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            dto.MaPhim = id;
            bool result = _phimBus.SuaPhim(dto);
            if (result)
            {
                return Ok(new { message = "Cập nhật phim thành công", data = dto });
            }
            return NotFound(new { message = $"Không tìm thấy phim với mã {id}" });
        }

        /// <summary>
        /// DELETE: api/phimapi/5
        /// Xóa phim (RESTful Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletePhim(int id)
        {
            bool result = _phimBus.XoaPhim(id);
            if (result)
            {
                return Ok(new { message = $"Đã xóa phim mã {id}" });
            }
            return NotFound(new { message = $"Không tìm thấy phim với mã {id} hoặc không thể xóa do có dữ liệu liên quan" });
        }

        // ========================================================
        // PHẦN 2: ADO.NET (DataAdapter + DataTable)
        // ========================================================

        /// <summary>
        /// GET: api/phimapi/adonet
        /// Lấy danh sách phim bằng ADO.NET thuần (SqlDataAdapter + DataTable)
        /// </summary>
        [HttpGet("adonet")]
        public IActionResult GetPhimsAdoNet()
        {
            try
            {
                DataTable dt = _adoNetDal.LayDanhSachPhimAdoNet();

                var resultList = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    resultList.Add(dict);
                }

                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi ADO.NET", details = ex.Message });
            }
        }

        
        [HttpGet("adonet-dataset")]
        public IActionResult GetPhimsDataSet()
        {
            try
            {
                DataSet ds = _adoNetDal.LayPhimVaSuatChieu_DataSet();

                var result = new Dictionary<string, object>();
                foreach (DataTable table in ds.Tables)
                {
                    var rows = new List<Dictionary<string, object>>();
                    foreach (DataRow row in table.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            dict[col.ColumnName] = row[col] == DBNull.Value ? null! : row[col];
                        }
                        rows.Add(dict);
                    }
                    result[table.TableName] = rows;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi ADO.NET DataSet", details = ex.Message });
            }
        }

        // ========================================================
        // PHẦN 3: LINQ to XML (Export / Import)
        // ========================================================

        /// <summary>
        /// GET: api/phimapi/export-xml
        /// Xuất danh sách phim ra định dạng XML sử dụng LINQ to XML (XDocument + XElement)
        /// </summary>
        [HttpGet("export-xml")]
        public IActionResult ExportXml()
        {
            try
            {
                // Sử dụng LINQ to Objects lấy dữ liệu
                var phims = _db.Phims.ToList();

                // Sử dụng LINQ to XML để xây dựng tài liệu XML
                XDocument xmlDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("DanhSachPhim",
                        new XAttribute("NgayXuat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XAttribute("TongSoPhim", phims.Count),
                        from p in phims
                        select new XElement("Phim",
                            new XAttribute("MaPhim", p.MaPhim),
                            new XElement("TenPhim", p.TenPhim),
                            new XElement("ThoiLuong", p.ThoiLuong ?? 0),
                            new XElement("GioiHanTuoi", p.GioiHanTuoi ?? 0),
                            new XElement("NgayKhoiChieu", p.NgayKhoiChieu?.ToString("yyyy-MM-dd") ?? ""),
                            new XElement("Hinh", p.Hinh ?? ""),
                            new XElement("MaLoaiPhim", p.MaLoaiPhim ?? 0)
                        )
                    )
                );

                // Trả về dạng XML
                return Content(xmlDoc.ToString(), "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Export XML", details = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/phimapi/import-xml
        /// Nhập danh sách phim từ XML sử dụng LINQ to XML
        /// Body: raw XML content
        /// </summary>
        [HttpPost("import-xml")]
        public IActionResult ImportXml([FromBody] string xmlContent)
        {
            try
            {
                // Parse XML bằng LINQ to XML
                XDocument xmlDoc = XDocument.Parse(xmlContent);

                // Dùng LINQ to XML để trích xuất dữ liệu từ các XElement
                var phimElements = xmlDoc.Descendants("Phim");

                var importedPhims = (from elem in phimElements
                                    select new PhimDTO
                                    {
                                        TenPhim = (string?)elem.Element("TenPhim") ?? "Không tên",
                                        ThoiLuong = (int?)elem.Element("ThoiLuong") ?? 0,
                                        GioiHanTuoi = (int?)elem.Element("GioiHanTuoi") ?? 0,
                                        Hinh = (string?)elem.Element("Hinh") ?? ""
                                    }).ToList();

                int count = 0;
                foreach (var dto in importedPhims)
                {
                    var phim = new Phim
                    {
                        TenPhim = dto.TenPhim,
                        ThoiLuong = dto.ThoiLuong,
                        GioiHanTuoi = dto.GioiHanTuoi,
                        Hinh = dto.Hinh,
                        MaLoaiPhim = 1
                    };
                    _db.Phims.Add(phim);
                    count++;
                }
                _db.SaveChanges();

                return Ok(new
                {
                    message = $"Import thành công {count} phim từ XML",
                    soLuong = count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "XML không hợp lệ", details = ex.Message });
            }
        }
    }
}
