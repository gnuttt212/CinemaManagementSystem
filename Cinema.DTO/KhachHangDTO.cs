using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? SDT { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateOnly? NgaySinh { get; set; }
        public int DiemTichLuy { get; set; }
        public string TaiKhoan { get; set; } = string.Empty;

        public List<HoaDonDTO> LichSuHoaDon { get; set; } = new List<HoaDonDTO>();
    }

    public class KhachHangLoginRequest
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string TaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; } = string.Empty;
    }

    public class KhachHangRegisterRequest
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [MinLength(3, ErrorMessage = "Tài khoản ít nhất 3 ký tự")]
        public string TaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        public string? SDT { get; set; }
    }
}
