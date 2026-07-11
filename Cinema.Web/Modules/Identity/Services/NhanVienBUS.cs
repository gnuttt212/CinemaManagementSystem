using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Identity.Entities;
using Cinema.Web.Modules.Identity.Entities;
using Cinema.DTO;
using System.Linq;
using BCrypt.Net;

namespace Cinema.Web.Modules.Identity.Services
{
    public class NhanVienBUS : INhanVienBUS
    {
        private readonly IdentityDbContext _db;

        public NhanVienBUS(IdentityDbContext db)
        {
            _db = db;
        }

        public bool DangNhap(NhanVienLoginRequest req)
        {
            var nv = _db.NhanViens.FirstOrDefault(n => n.TaiKhoan == req.TaiKhoan);
            if (nv == null) return false;
            return BCrypt.Net.BCrypt.Verify(req.MatKhau, nv.MatKhau);
        }

        public NhanVienDTO? LayNhanVienSauDangNhap(NhanVienLoginRequest req)
        {
            var nv = _db.NhanViens.FirstOrDefault(n => n.TaiKhoan == req.TaiKhoan);
            if (nv == null || !BCrypt.Net.BCrypt.Verify(req.MatKhau, nv.MatKhau))
                return null;

            return new NhanVienDTO
            {
                MaNV = nv.MaNv,
                TaiKhoan = nv.TaiKhoan,
                HoTen = nv.HoTen ?? "",
                ChucVu = nv.ChucVu,
                PhanQuyen = nv.PhanQuyen
            };
        }

        public bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi)
        {
            try
            {
                var nv = _db.NhanViens.FirstOrDefault(n => n.TaiKhoan == taiKhoan);
                if (nv == null) return false;

                if (!BCrypt.Net.BCrypt.Verify(matKhauCu, nv.MatKhau)) return false;

                nv.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
                _db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public NhanVienDTO? LayThongTinProfileNhanVien(string taiKhoan)
        {
            var nv = _db.NhanViens.FirstOrDefault(n => n.TaiKhoan == taiKhoan);
            if (nv == null) return null;
            return new NhanVienDTO
            {
                MaNV = nv.MaNv,
                HoTen = nv.HoTen ?? "",
                ChucVu = nv.ChucVu,
                TaiKhoan = nv.TaiKhoan,
                PhanQuyen = nv.PhanQuyen
            };
        }

        public System.Collections.Generic.List<NhanVienDTO> LayDanhSach()
        {
            return _db.NhanViens.Select(n => new NhanVienDTO
            {
                MaNV = n.MaNv,
                HoTen = n.HoTen ?? "",
                ChucVu = n.ChucVu,
                TaiKhoan = n.TaiKhoan,
                PhanQuyen = n.PhanQuyen
            }).ToList();
        }

        public NhanVienDTO? LayTheoMa(int maNv)
        {
            var n = _db.NhanViens.Find(maNv);
            if (n == null) return null;
            return new NhanVienDTO
            {
                MaNV = n.MaNv,
                HoTen = n.HoTen ?? "",
                ChucVu = n.ChucVu,
                TaiKhoan = n.TaiKhoan,
                PhanQuyen = n.PhanQuyen
            };
        }

        public bool Them(NhanVienDTO dto)
        {
            try
            {
                var nv = new NhanVien
                {
                    HoTen = dto.HoTen,
                    ChucVu = dto.ChucVu,
                    TaiKhoan = dto.TaiKhoan,
                    PhanQuyen = dto.PhanQuyen,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau ?? "123456")
                };
                _db.NhanViens.Add(nv);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool Sua(NhanVienDTO dto)
        {
            try
            {
                var nv = _db.NhanViens.Find(dto.MaNV);
                if (nv == null) return false;

                nv.HoTen = dto.HoTen;
                nv.ChucVu = dto.ChucVu;
                nv.TaiKhoan = dto.TaiKhoan;
                nv.PhanQuyen = dto.PhanQuyen;
                
                if (!string.IsNullOrEmpty(dto.MatKhau))
                {
                    nv.MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau);
                }

                _db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool Xoa(int maNv)
        {
            try
            {
                var nv = _db.NhanViens.Find(maNv);
                if (nv == null) return false;
                
                if (nv.TaiKhoan.ToLower() == "admin")
                {
                    return false; // Không cho phép xóa tài khoản admin mặc định
                }
                
                _db.NhanViens.Remove(nv);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}


