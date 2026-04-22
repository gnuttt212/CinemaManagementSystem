using Cinema.DTO;

namespace Cinema.BUS
{
    public interface INhanVienBUS
    {
        bool DangNhap(NhanVienLoginRequest request);
        NhanVienDTO? LayNhanVienSauDangNhap(NhanVienLoginRequest request);
        bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi);
        
        List<NhanVienDTO> LayDanhSach();
        NhanVienDTO? LayTheoMa(int maNv);
        bool Them(NhanVienDTO dto);
        bool Sua(NhanVienDTO dto);
        bool Xoa(int maNv);
    }
}
