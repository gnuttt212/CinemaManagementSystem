using Microsoft.EntityFrameworkCore;
using Cinema.Web.Modules.Booking.Entities;
using Cinema.Web.Modules.Catalog.Entities;

namespace Cinema.Web.Modules.Booking.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDoAn> ChiTietDoAns { get; set; }
    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    public virtual DbSet<HoaDon> HoaDons { get; set; }
    public virtual DbSet<VwDoanhThuTheoPhim> VwDoanhThuTheoPhims { get; set; }

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
            entity.Property(e => e.LoaiGhe).HasMaxLength(20).HasDefaultValue("Thuong");
        });

        modelBuilder.Entity<LichChieu>(entity =>
        {
            entity.HasKey(e => e.MaLich);
            entity.ToTable("LichChieu");
            entity.HasIndex(e => e.MaPhim, "IX_LichChieu_MaPhim");
            entity.Property(e => e.GiaVe).HasColumnType("money");
            entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.LichChieus)
                .HasForeignKey(d => d.MaPhim)
                .HasConstraintName("FK_LichChieu_Phim");
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

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd);
            entity.ToTable("HoaDon", tb => tb.HasTrigger("trg_NganXoaHoaDonCoCTHD"));
            entity.HasIndex(e => e.MaKh, "IX_HoaDon_MaKH");
            entity.HasIndex(e => e.MaNv, "IX_HoaDon_MaNV");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasDefaultValue(0m).HasColumnType("money");
            entity.Property(e => e.TrangThai).HasMaxLength(50).HasDefaultValue("Chờ thanh toán");
        });

        modelBuilder.Entity<VwDoanhThuTheoPhim>(entity =>
        {
            entity.HasNoKey().ToView("vw_DoanhThuTheoPhim");
            entity.Property(e => e.TenPhim).HasMaxLength(200);
            entity.Property(e => e.TongDoanhThu).HasColumnType("money");
        });
    }
}

