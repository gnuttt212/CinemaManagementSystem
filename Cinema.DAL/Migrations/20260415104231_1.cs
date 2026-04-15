using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DonGia = table.Column<decimal>(type: "money", nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true)
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
                    TaiKhoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
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
                    TenPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoLuongGhe = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phong__20BD5E5B303B82C4", x => x.MaPhong);
                });

            migrationBuilder.CreateTable(
                name: "Phim",
                columns: table => new
                {
                    MaPhim = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ThoiLuong = table.Column<int>(type: "int", nullable: true),
                    GioiHanTuoi = table.Column<int>(type: "int", nullable: true),
                    MaLoaiPhim = table.Column<int>(type: "int", nullable: true),
                    Hinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKhoiChieu = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phim__4AC03DE3078CB066", x => x.MaPhim);
                    table.ForeignKey(
                        name: "FK_Phim_LoaiPhim",
                        column: x => x.MaLoaiPhim,
                        principalTable: "LoaiPhim",
                        principalColumn: "MaLoai");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TongTien = table.Column<decimal>(type: "money", nullable: true, defaultValue: 0m),
                    MaND = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__2725A6E0E68C72B0", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK_HoaDon_NguoiDung",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND");
                });

            migrationBuilder.CreateTable(
                name: "Ghe",
                columns: table => new
                {
                    MaGhe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenGhe = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    LoaiGhe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DF__Ghe__DaDat__3D5E1FD2", x => x.MaGhe);
                    table.ForeignKey(
                        name: "FK_Ghe_Phong",
                        column: x => x.MaPhong,
                        principalTable: "Phong",
                        principalColumn: "MaPhong");
                });

            migrationBuilder.CreateTable(
                name: "SuatChieu",
                columns: table => new
                {
                    MaSuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    NgayChieu = table.Column<DateOnly>(type: "date", nullable: true),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: true),
                    GiaVe = table.Column<decimal>(type: "money", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: true),
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
                name: "ChiTietDV",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaDV = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    DonGiaBan = table.Column<decimal>(type: "money", nullable: true)
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
                name: "Ve",
                columns: table => new
                {
                    MaVe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSuat = table.Column<int>(type: "int", nullable: true),
                    MaGhe = table.Column<int>(type: "int", nullable: true),
                    MaHD = table.Column<int>(type: "int", nullable: true),
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
                name: "IX_ChiTietDV_MaDV",
                table: "ChiTietDV",
                column: "MaDV");

            migrationBuilder.CreateIndex(
                name: "IX_Ghe_MaPhong",
                table: "Ghe",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaND",
                table: "HoaDon",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__D5B8C7F0B65F44B0",
                table: "NguoiDung",
                column: "TaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phim_MaLoaiPhim",
                table: "Phim",
                column: "MaLoaiPhim");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDV");

            migrationBuilder.DropTable(
                name: "Ve");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "Ghe");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "SuatChieu");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "Phim");

            migrationBuilder.DropTable(
                name: "LoaiPhim");
        }
    }
}
