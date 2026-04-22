using Cinema.DTO;
using System.Collections.Generic;

namespace Cinema.BUS
{
    public interface IKhachHangBUS
    {
        bool DangKy(KhachHangRegisterRequest request);
        bool DangNhap(KhachHangLoginRequest request);
        KhachHangDTO? LayKhachHangSauDangNhap(KhachHangLoginRequest request);
        KhachHangDTO LayThongTinProfile(string taiKhoan);
        bool CapNhatProfile(KhachHangDTO model);
        bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi);
        
        List<KhachHangDTO> LayDanhSach();
        KhachHangDTO? LayTheoMa(int maKh);
        bool Sua(KhachHangDTO model);
        bool Xoa(int maKh);
    }
}
