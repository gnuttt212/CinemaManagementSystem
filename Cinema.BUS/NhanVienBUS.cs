using Cinema.DAL.Models;
using Cinema.DTO;
using System.Linq;
using BCrypt.Net;

namespace Cinema.BUS
{
    public class NhanVienBUS : INhanVienBUS
    {
        private readonly QuanLyRapPhimContext _db;

        public NhanVienBUS(QuanLyRapPhimContext db)
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
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}
