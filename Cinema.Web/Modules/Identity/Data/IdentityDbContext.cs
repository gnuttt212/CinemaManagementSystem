using Microsoft.EntityFrameworkCore;
using Cinema.Web.Modules.Identity.Entities;

namespace Cinema.Web.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }
    public virtual DbSet<NhanVien> NhanViens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh);
            entity.ToTable("KhachHang");
            entity.HasIndex(e => e.TaiKhoan, "UQ_KhachHang_TaiKhoan").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.Sdt).HasMaxLength(20).IsUnicode(false).HasColumnName("SDT");
            entity.Property(e => e.TaiKhoan).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv);
            entity.ToTable("NhanVien");
            entity.HasIndex(e => e.TaiKhoan, "UQ_NhanVien_TaiKhoan").IsUnique();

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.PhanQuyen).HasMaxLength(20).HasDefaultValue("NhanVien");
            entity.Property(e => e.TaiKhoan).HasMaxLength(50).IsUnicode(false);
        });
    }
}

