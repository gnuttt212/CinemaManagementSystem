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

    public virtual DbSet<ChiTietDv> ChiTietDvs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<Ghe> Ghes { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LoaiPhim> LoaiPhims { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<Phim> Phims { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<SuatChieu> SuatChieus { get; set; }

    public virtual DbSet<Ve> Ves { get; set; }

    public virtual DbSet<VwDoanhThuTheoPhim> VwDoanhThuTheoPhims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-0CBT09R\\SQLEXPRESS;Database=QuanLyRapPhim;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDv>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaDv }).HasName("PK__ChiTietD__4557FE856337680E");

            entity.ToTable("ChiTietDV");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaDv).HasColumnName("MaDV");
            entity.Property(e => e.DonGiaBan).HasColumnType("money");

            entity.HasOne(d => d.MaDvNavigation).WithMany(p => p.ChiTietDvs)
                .HasForeignKey(d => d.MaDv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDV__MaDV__46E78A0C");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietDvs)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDV__MaHD__45F365D3");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDv).HasName("PK__DichVu__27258657D6403FCF");

            entity.ToTable("DichVu");

            entity.Property(e => e.MaDv).HasColumnName("MaDV");
            entity.Property(e => e.DonGia).HasColumnType("money");
            entity.Property(e => e.TenDv)
                .HasMaxLength(100)
                .HasColumnName("TenDV");
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe).HasName("DF__Ghe__DaDat__3D5E1FD2");

            entity.ToTable("Ghe");

            entity.Property(e => e.LoaiGhe).HasMaxLength(20);
            entity.Property(e => e.TenGhe)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_Ghe_Phong");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E0E68C72B0");

            entity.ToTable("HoaDon", tb => tb.HasTrigger("trg_NganXoaHoaDonCoVe"));

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("money");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNd)
                .HasConstraintName("FK_HoaDon_NguoiDung");
        });

        modelBuilder.Entity<LoaiPhim>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiPhim__730A57599BA3C329");

            entity.ToTable("LoaiPhim");

            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NguoiDun__2725D724C88B3203");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TaiKhoan, "UQ__NguoiDun__D5B8C7F0B65F44B0").IsUnique();

            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Phim>(entity =>
        {
            entity.HasKey(e => e.MaPhim).HasName("PK__Phim__4AC03DE3078CB066");

            entity.ToTable("Phim");

            entity.Property(e => e.NgayKhoiChieu).HasColumnType("datetime");
            entity.Property(e => e.TenPhim).HasMaxLength(200);

            entity.HasOne(d => d.MaLoaiPhimNavigation).WithMany(p => p.Phims)
                .HasForeignKey(d => d.MaLoaiPhim)
                .HasConstraintName("FK_Phim_LoaiPhim");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B303B82C4");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong).ValueGeneratedNever();
            entity.Property(e => e.TenPhong).HasMaxLength(100);
        });

        modelBuilder.Entity<SuatChieu>(entity =>
        {
            entity.HasKey(e => e.MaSuat).HasName("PK__SuatChie__A69D0241CD876E94");

            entity.ToTable("SuatChieu");

            entity.Property(e => e.GiaVe).HasColumnType("money");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Sắp chiếu");

            entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhim)
                .HasConstraintName("FK__SuatChieu__MaPhi__398D8EEE");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_SuatChieu_Phong");
        });

        modelBuilder.Entity<Ve>(entity =>
        {
            entity.HasKey(e => e.MaVe).HasName("PK__Ve__2725100F68C2289C");

            entity.ToTable("Ve");

            entity.HasIndex(e => new { e.MaSuat, e.MaGhe }, "UC_SuatChieu_Ghe").IsUnique();

            entity.Property(e => e.GiaVe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.Ves)
                .HasForeignKey(d => d.MaGhe)
                .HasConstraintName("FK_Ve_Ghe");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Ves)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK__Ve__MaHD__4F7CD00D");

            entity.HasOne(d => d.MaSuatNavigation).WithMany(p => p.Ves)
                .HasForeignKey(d => d.MaSuat)
                .HasConstraintName("FK__Ve__MaSuat__4D94879B");
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
