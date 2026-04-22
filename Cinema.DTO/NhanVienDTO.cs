using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DTO
{
    public class NhanVienDTO
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? ChucVu { get; set; }
        public string TaiKhoan { get; set; } = string.Empty;
        public string? PhanQuyen { get; set; }
        public string? MatKhau { get; set; }
    }

    public class NhanVienLoginRequest
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string TaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; } = string.Empty;
    }
}
