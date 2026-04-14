using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.DAL.Models
{
    /// <summary>
    /// Model Code First: Bảng nhật ký hệ thống, được tạo bằng Migration 
    /// (minh họa kỹ thuật Code First bên cạnh Database First đã có)
    /// </summary>
    [Table("NhatKyHeThong")]
    public class NhatKyHeThong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhatKy { get; set; }

        [Required]
        [MaxLength(100)]
        public string HanhDong { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? MoTa { get; set; }

        [MaxLength(50)]
        public string? TaiKhoan { get; set; }

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? DiaChi_IP { get; set; }
    }
}
