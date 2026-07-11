using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Identity.Entities;
using Cinema.DTO;

namespace Cinema.Web.Modules.Identity.Services
{
    public interface INhanVienBUS
    {
        bool DangNhap(NhanVienLoginRequest request);
        NhanVienDTO? LayNhanVienSauDangNhap(NhanVienLoginRequest request);
        bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi);
        NhanVienDTO? LayThongTinProfileNhanVien(string taiKhoan);
        
        List<NhanVienDTO> LayDanhSach();
        NhanVienDTO? LayTheoMa(int maNv);
        bool Them(NhanVienDTO dto);
        bool Sua(NhanVienDTO dto);
        bool Xoa(int maNv);
    }
}


