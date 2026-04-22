using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Models;

public partial class QuanLyRapPhimContext : DbContext
{
    public QuanLyRapPhimContext()
    {
    }

    public QuanLyRapPhimContext(DbContextOptions<QuanLyRapPhimContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDoAn> ChiTietDoAns { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DoAn> DoAns { get; set; }

    public virtual DbSet<Ghe> Ghes { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichChieu> LichChieus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phim> Phims { get; set; }

    public virtual DbSet<PhongChieu> PhongChieus { get; set; }

    public virtual DbSet<VwDoanhThuTheoPhim> VwDoanhThuTheoPhims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-0CBT09R\\SQLEXPRESS;Initial Catalog=QuanLyRapPhim;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDoAn>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaDoAn });

            entity.ToTable("ChiTietDoAn");

            entity.HasIndex(e => e.MaDoAn, "IX_ChiTietDoAn_MaDoAn");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.Gia).HasColumnType("money");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaDoAnNavigation).WithMany(p => p.ChiTietDoAns)
                .HasForeignKey(d => d.MaDoAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDoAn_DoAn");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietDoAns)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDoAn_HoaDon");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaGhe });

            entity.ToTable("ChiTietHoaDon");

            entity.HasIndex(e => e.MaLich, "IX_ChiTietHoaDon_MaLich");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.GiaVe).HasColumnType("money");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaGhe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_Ghe");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_HoaDon");

            entity.HasOne(d => d.MaLichNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaLich)
                .HasConstraintName("FK_CTHD_LichChieu");
        });

        modelBuilder.Entity<DoAn>(entity =>
        {
            entity.HasKey(e => e.MaDoAn);

            entity.ToTable("DoAn");

            entity.Property(e => e.Gia)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.Loai).HasMaxLength(50);
            entity.Property(e => e.TenDoAn).HasMaxLength(100);
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe);

            entity.ToTable("Ghe");

            entity.HasIndex(e => e.MaPhong, "IX_Ghe_MaPhong");

            entity.Property(e => e.Hang).HasMaxLength(5);
            entity.Property(e => e.LoaiGhe)
                .HasMaxLength(20)
                .HasDefaultValue("Thường");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_Ghe_PhongChieu");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd);

            entity.ToTable("HoaDon", tb => tb.HasTrigger("trg_NganXoaHoaDonCoCTHD"));

            entity.HasIndex(e => e.MaKh, "IX_HoaDon_MaKH");

            entity.HasIndex(e => e.MaNv, "IX_HoaDon_MaNV");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ thanh toán");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_HoaDon_KhachHang");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_HoaDon_NhanVien");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh);

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.TaiKhoan, "UQ_KhachHang_TaiKhoan").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm);

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.DieuKien).HasMaxLength(500);
            entity.Property(e => e.PhanTramGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenKm)
                .HasMaxLength(200)
                .HasColumnName("TenKM");
        });

        modelBuilder.Entity<LichChieu>(entity =>
        {
            entity.HasKey(e => e.MaLich);

            entity.ToTable("LichChieu");

            entity.HasIndex(e => e.MaPhim, "IX_LichChieu_MaPhim");

            entity.HasIndex(e => e.MaPhong, "IX_LichChieu_MaPhong");

            entity.Property(e => e.GiaVe).HasColumnType("money");

            entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.LichChieus)
                .HasForeignKey(d => d.MaPhim)
                .HasConstraintName("FK_LichChieu_Phim");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.LichChieus)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_LichChieu_PhongChieu");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv);

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.TaiKhoan, "UQ_NhanVien_TaiKhoan").IsUnique();

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhanQuyen)
                .HasMaxLength(20)
                .HasDefaultValue("NhanVien");
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Phim>(entity =>
        {
            entity.HasKey(e => e.MaPhim);

            entity.ToTable("Phim");

            entity.Property(e => e.DaoDien).HasMaxLength(100);
            entity.Property(e => e.NgayKhoiChieu).HasColumnType("datetime");
            entity.Property(e => e.TenPhim).HasMaxLength(200);
            entity.Property(e => e.TheLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<PhongChieu>(entity =>
        {
            entity.HasKey(e => e.MaPhong);

            entity.ToTable("PhongChieu");

            entity.Property(e => e.LoaiPhong)
                .HasMaxLength(20)
                .HasDefaultValue("2D");
            entity.Property(e => e.SucChua).HasDefaultValue(0);
            entity.Property(e => e.TenPhong).HasMaxLength(100);
        });

        modelBuilder.Entity<VwDoanhThuTheoPhim>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_DoanhThuTheoPhim");

            entity.Property(e => e.TenPhim).HasMaxLength(200);
            entity.Property(e => e.TongDoanhThu).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
