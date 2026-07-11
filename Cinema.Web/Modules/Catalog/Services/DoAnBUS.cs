using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Web.Modules.Catalog.Services
{
    public class DoAnBUS : IDoAnBUS
    {
        private readonly CatalogDbContext _db;

        public DoAnBUS(CatalogDbContext db)
        {
            _db = db;
        }

        public List<DoAnDTO> LayDanhSachDoAn()
        {
            return _db.DoAns
                .Select(d => new DoAnDTO
                {
                    MaDoAn = d.MaDoAn,
                    TenDoAn = d.TenDoAn,
                    Gia = d.Gia ?? 0,
                    Loai = d.Loai
                }).ToList();
        }

        public DoAnDTO LayTheoId(int id)
        {
            var result = _db.DoAns
                .Where(d => d.MaDoAn == id)
                .Select(d => new DoAnDTO
                {
                    MaDoAn = d.MaDoAn,
                    TenDoAn = d.TenDoAn,
                    Gia = d.Gia ?? 0,
                    Loai = d.Loai
                }).FirstOrDefault();
            return result;
        }
    }
}


