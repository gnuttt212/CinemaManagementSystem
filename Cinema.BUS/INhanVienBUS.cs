using Cinema.DTO;

namespace Cinema.BUS
{
    public interface INhanVienBUS
    {
        bool DangNhap(NhanVienLoginRequest request);
        NhanVienDTO? LayNhanVienSauDangNhap(NhanVienLoginRequest request);
        bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi);
    }
}
