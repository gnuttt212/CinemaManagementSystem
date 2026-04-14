using System;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DTO
{
    public class DoiMatKhauRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        public string MatKhauHienTai { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string MatKhauMoi { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
        public string XacNhanMatKhau { get; set; } = null!;
    }
}
