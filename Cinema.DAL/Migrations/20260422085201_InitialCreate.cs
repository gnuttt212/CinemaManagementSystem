using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ghe_Phong",
                table: "Ghe");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_NguoiDung",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_Phim_LoaiPhim",
                table: "Phim");

            migrationBuilder.DropTable(
                name: "ChiTietDV");

            migrationBuilder.DropTable(
                name: "LoaiPhim");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Ve");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "SuatChieu");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Phim__4AC03DE3078CB066",
                table: "Phim");

            migrationBuilder.DropIndex(
                name: "IX_Phim_MaLoaiPhim",
                table: "Phim");

            migrationBuilder.DropPrimaryKey(
                name: "PK__HoaDon__2725A6E0E68C72B0",
                table: "HoaDon");

            migrationBuilder.DropPrimaryKey(
                name: "DF__Ghe__DaDat__3D5E1FD2",
                table: "Ghe");

            migrationBuilder.DropColumn(
                name: "GioiHanTuoi",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "MaLoaiPhim",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "TenGhe",
                table: "Ghe");

            migrationBuilder.RenameColumn(
                name: "Hinh",
                table: "Phim",
                newName: "Poster");

            migrationBuilder.RenameColumn(
                name: "NgayLap",
                table: "HoaDon",
                newName: "NgayDat");

            migrationBuilder.RenameColumn(
                name: "MaND",
                table: "HoaDon",
                newName: "MaNV");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_MaND",
                table: "HoaDon",
                newName: "IX_HoaDon_MaNV");

            migrationBuilder.AddColumn<string>(
                name: "DaoDien",
                table: "Phim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "Phim",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TheLoai",
                table: "Phim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaKH",
                table: "HoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "HoaDon",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Chờ thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiGhe",
                table: "Ghe",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Thường",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hang",
                table: "Ghe",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoGhe",
                table: "Ghe",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Phim",
                table: "Phim",
                column: "MaPhim");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon",
                column: "MaHD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ghe",
                table: "Ghe",
                column: "MaGhe");

            migrationBuilder.CreateTable(
                name: "DoAn",
                columns: table => new
                {
                    MaDoAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDoAn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gia = table.Column<decimal>(type: "money", nullable: true, defaultValue: 0m),
                    Loai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoAn", x => x.MaDoAn);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    TaiKhoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    MaKM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKM = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    DieuKien = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayBatDau = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.MaKM);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaiKhoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    PhanQuyen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "NhanVien")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyHeThong",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChi_IP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyHeThong", x => x.MaNhatKy);
                });

            migrationBuilder.CreateTable(
                name: "PhongChieu",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "2D"),
                    SucChua = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongChieu", x => x.MaPhong);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDoAn",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaDoAn = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    Gia = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDoAn", x => new { x.MaHD, x.MaDoAn });
                    table.ForeignKey(
                        name: "FK_CTDoAn_DoAn",
                        column: x => x.MaDoAn,
                        principalTable: "DoAn",
                        principalColumn: "MaDoAn");
                    table.ForeignKey(
                        name: "FK_CTDoAn_HoaDon",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                });

            migrationBuilder.CreateTable(
                name: "LichChieu",
                columns: table => new
                {
                    MaLich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    NgayChieu = table.Column<DateOnly>(type: "date", nullable: true),
                    GioChieu = table.Column<TimeOnly>(type: "time", nullable: true),
                    GiaVe = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichChieu", x => x.MaLich);
                    table.ForeignKey(
                        name: "FK_LichChieu_Phim",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "MaPhim");
                    table.ForeignKey(
                        name: "FK_LichChieu_PhongChieu",
                        column: x => x.MaPhong,
                        principalTable: "PhongChieu",
                        principalColumn: "MaPhong");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaGhe = table.Column<int>(type: "int", nullable: false),
                    MaLich = table.Column<int>(type: "int", nullable: true),
                    GiaVe = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHoaDon", x => new { x.MaHD, x.MaGhe });
                    table.ForeignKey(
                        name: "FK_CTHD_Ghe",
                        column: x => x.MaGhe,
                        principalTable: "Ghe",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK_CTHD_HoaDon",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                    table.ForeignKey(
                        name: "FK_CTHD_LichChieu",
                        column: x => x.MaLich,
                        principalTable: "LichChieu",
                        principalColumn: "MaLich");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDon",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDoAn_MaDoAn",
                table: "ChiTietDoAn",
                column: "MaDoAn");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaGhe",
                table: "ChiTietHoaDon",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaLich",
                table: "ChiTietHoaDon",
                column: "MaLich");

            migrationBuilder.CreateIndex(
                name: "UQ_KhachHang_TaiKhoan",
                table: "KhachHang",
                column: "TaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_MaPhim",
                table: "LichChieu",
                column: "MaPhim");

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_MaPhong",
                table: "LichChieu",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "UQ_NhanVien_TaiKhoan",
                table: "NhanVien",
                column: "TaiKhoan",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ghe_PhongChieu",
                table: "Ghe",
                column: "MaPhong",
                principalTable: "PhongChieu",
                principalColumn: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_KhachHang",
                table: "HoaDon",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_NhanVien",
                table: "HoaDon",
                column: "MaNV",
                principalTable: "NhanVien",
                principalColumn: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ghe_PhongChieu",
                table: "Ghe");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_KhachHang",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_NhanVien",
                table: "HoaDon");

            migrationBuilder.DropTable(
                name: "ChiTietDoAn");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDon");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "NhatKyHeThong");

            migrationBuilder.DropTable(
                name: "DoAn");

            migrationBuilder.DropTable(
                name: "LichChieu");

            migrationBuilder.DropTable(
                name: "PhongChieu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Phim",
                table: "Phim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ghe",
                table: "Ghe");

            migrationBuilder.DropColumn(
                name: "DaoDien",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "TheLoai",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "MaKH",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "Hang",
                table: "Ghe");

            migrationBuilder.DropColumn(
                name: "SoGhe",
                table: "Ghe");

            migrationBuilder.RenameColumn(
                name: "Poster",
                table: "Phim",
                newName: "Hinh");

            migrationBuilder.RenameColumn(
                name: "NgayDat",
                table: "HoaDon",
                newName: "NgayLap");

            migrationBuilder.RenameColumn(
                name: "MaNV",
                table: "HoaDon",
                newName: "MaND");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_MaNV",
                table: "HoaDon",
                newName: "IX_HoaDon_MaND");

            migrationBuilder.AddColumn<int>(
                name: "GioiHanTuoi",
                table: "Phim",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaLoaiPhim",
                table: "Phim",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiGhe",
                table: "Ghe",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Thường");

            migrationBuilder.AddColumn<string>(
                name: "TenGhe",
                table: "Ghe",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Phim__4AC03DE3078CB066",
                table: "Phim",
                column: "MaPhim");

            migrationBuilder.AddPrimaryKey(
                name: "PK__HoaDon__2725A6E0E68C72B0",
                table: "HoaDon",
                column: "MaHD");

            migrationBuilder.AddPrimaryKey(
                name: "DF__Ghe__DaDat__3D5E1FD2",
                table: "Ghe",
                column: "MaGhe");

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonGia = table.Column<decimal>(type: "money", nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true),
                    TenDV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DichVu__27258657D6403FCF", x => x.MaDV);
                });

            migrationBuilder.CreateTable(
                name: "LoaiPhim",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoaiPhim__730A57599BA3C329", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    TaiKhoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__2725D724C88B3203", x => x.MaND);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    SoLuongGhe = table.Column<int>(type: "int", nullable: true),
                    TenPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phong__20BD5E5B303B82C4", x => x.MaPhong);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDV",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaDV = table.Column<int>(type: "int", nullable: false),
                    DonGiaBan = table.Column<decimal>(type: "money", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__4557FE856337680E", x => new { x.MaHD, x.MaDV });
                    table.ForeignKey(
                        name: "FK__ChiTietDV__MaDV__46E78A0C",
                        column: x => x.MaDV,
                        principalTable: "DichVu",
                        principalColumn: "MaDV");
                    table.ForeignKey(
                        name: "FK__ChiTietDV__MaHD__45F365D3",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                });

            migrationBuilder.CreateTable(
                name: "SuatChieu",
                columns: table => new
                {
                    MaSuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    GiaVe = table.Column<decimal>(type: "money", nullable: true),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: true),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: true),
                    NgayChieu = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Sắp chiếu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SuatChie__A69D0241CD876E94", x => x.MaSuat);
                    table.ForeignKey(
                        name: "FK_SuatChieu_Phong",
                        column: x => x.MaPhong,
                        principalTable: "Phong",
                        principalColumn: "MaPhong");
                    table.ForeignKey(
                        name: "FK__SuatChieu__MaPhi__398D8EEE",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "MaPhim");
                });

            migrationBuilder.CreateTable(
                name: "Ve",
                columns: table => new
                {
                    MaVe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGhe = table.Column<int>(type: "int", nullable: true),
                    MaHD = table.Column<int>(type: "int", nullable: true),
                    MaSuat = table.Column<int>(type: "int", nullable: true),
                    GiaVe = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ve__2725100F68C2289C", x => x.MaVe);
                    table.ForeignKey(
                        name: "FK_Ve_Ghe",
                        column: x => x.MaGhe,
                        principalTable: "Ghe",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK__Ve__MaHD__4F7CD00D",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                    table.ForeignKey(
                        name: "FK__Ve__MaSuat__4D94879B",
                        column: x => x.MaSuat,
                        principalTable: "SuatChieu",
                        principalColumn: "MaSuat");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phim_MaLoaiPhim",
                table: "Phim",
                column: "MaLoaiPhim");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDV_MaDV",
                table: "ChiTietDV",
                column: "MaDV");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__D5B8C7F0B65F44B0",
                table: "NguoiDung",
                column: "TaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuatChieu_MaPhim",
                table: "SuatChieu",
                column: "MaPhim");

            migrationBuilder.CreateIndex(
                name: "IX_SuatChieu_MaPhong",
                table: "SuatChieu",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_MaGhe",
                table: "Ve",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_MaHD",
                table: "Ve",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "UC_SuatChieu_Ghe",
                table: "Ve",
                columns: new[] { "MaSuat", "MaGhe" },
                unique: true,
                filter: "[MaSuat] IS NOT NULL AND [MaGhe] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Ghe_Phong",
                table: "Ghe",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_NguoiDung",
                table: "HoaDon",
                column: "MaND",
                principalTable: "NguoiDung",
                principalColumn: "MaND");

            migrationBuilder.AddForeignKey(
                name: "FK_Phim_LoaiPhim",
                table: "Phim",
                column: "MaLoaiPhim",
                principalTable: "LoaiPhim",
                principalColumn: "MaLoai");
        }
    }
}
