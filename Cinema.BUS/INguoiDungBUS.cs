using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DAL.Models;
using Cinema.DTO;

namespace Cinema.BUS
{
    public interface INguoiDungBUS
    {
        bool CapNhatProfile(NguoiDungDTO model);
        bool DangKy(RegisterRequest request);
        bool DangNhap(LoginRequest request);
        NguoiDungDTO? LayNguoiDungSauDangNhap(LoginRequest request);
        NguoiDungDTO LayThongTinProfile(string taiKhoan);
        bool DoiMatKhau(string taiKhoan, string matKhauCu, string matKhauMoi);
    }
}