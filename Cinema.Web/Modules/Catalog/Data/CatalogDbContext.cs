using Microsoft.EntityFrameworkCore;
using Cinema.Web.Modules.Catalog.Entities;

namespace Cinema.Web.Modules.Catalog.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DoAn> DoAns { get; set; }
    public virtual DbSet<Ghe> Ghes { get; set; }
    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
    public virtual DbSet<LichChieu> LichChieus { get; set; }
    public virtual DbSet<Phim> Phims { get; set; }
    public virtual DbSet<PhongChieu> PhongChieus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoAn>(entity =>
        {
            entity.HasKey(e => e.MaDoAn);
            entity.ToTable("DoAn");
            entity.Property(e => e.Gia).HasDefaultValue(0m).HasColumnType("money");
            entity.Property(e => e.Loai).HasMaxLength(50);
            entity.Property(e => e.TenDoAn).HasMaxLength(100);
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe);
            entity.ToTable("Ghe");
            entity.HasIndex(e => e.MaPhong, "IX_Ghe_MaPhong");
            entity.Property(e => e.Hang).HasMaxLength(5);
            entity.Property(e => e.LoaiGhe).HasMaxLength(20).HasDefaultValue("Thường");
            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_Ghe_PhongChieu");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm);
            entity.ToTable("KhuyenMai");
            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.DieuKien).HasMaxLength(500);
            entity.Property(e => e.PhanTramGiam).HasDefaultValue(0m).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenKm).HasMaxLength(200).HasColumnName("TenKM");
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
            entity.Property(e => e.LoaiPhong).HasMaxLength(20).HasDefaultValue("2D");
            entity.Property(e => e.SucChua).HasDefaultValue(0);
            entity.Property(e => e.TenPhong).HasMaxLength(100);
        });
    }
}

