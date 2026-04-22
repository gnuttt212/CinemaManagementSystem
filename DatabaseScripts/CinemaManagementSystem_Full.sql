IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyRapPhim')
BEGIN
    CREATE DATABASE QuanLyRapPhim;
END
GO

USE QuanLyRapPhim;
GO

IF OBJECT_ID('trg_NganXoaHoaDonCoCTHD', 'TR') IS NOT NULL DROP TRIGGER trg_NganXoaHoaDonCoCTHD;
IF OBJECT_ID('trg_NganXoaHoaDonCoVe', 'TR') IS NOT NULL DROP TRIGGER trg_NganXoaHoaDonCoVe;
GO

IF OBJECT_ID('sp_LayDanhSachPhimDangChieu', 'P') IS NOT NULL DROP PROCEDURE sp_LayDanhSachPhimDangChieu;
GO

IF OBJECT_ID('fn_TinhTongTienHoaDon', 'FN') IS NOT NULL DROP FUNCTION fn_TinhTongTienHoaDon;
GO

IF OBJECT_ID('vw_DoanhThuTheoPhim', 'V') IS NOT NULL DROP VIEW vw_DoanhThuTheoPhim;
GO

IF OBJECT_ID('ChiTietDoAn', 'U') IS NOT NULL DROP TABLE ChiTietDoAn;
IF OBJECT_ID('ChiTietHoaDon', 'U') IS NOT NULL DROP TABLE ChiTietHoaDon;
IF OBJECT_ID('ChiTietDV', 'U') IS NOT NULL DROP TABLE ChiTietDV;
IF OBJECT_ID('Ve', 'U') IS NOT NULL DROP TABLE Ve;
IF OBJECT_ID('HoaDon', 'U') IS NOT NULL DROP TABLE HoaDon;
IF OBJECT_ID('LichChieu', 'U') IS NOT NULL DROP TABLE LichChieu;
IF OBJECT_ID('SuatChieu', 'U') IS NOT NULL DROP TABLE SuatChieu;
IF OBJECT_ID('Ghe', 'U') IS NOT NULL DROP TABLE Ghe;
IF OBJECT_ID('PhongChieu', 'U') IS NOT NULL DROP TABLE PhongChieu;
IF OBJECT_ID('Phong', 'U') IS NOT NULL DROP TABLE Phong;
IF OBJECT_ID('DoAn', 'U') IS NOT NULL DROP TABLE DoAn;
IF OBJECT_ID('DichVu', 'U') IS NOT NULL DROP TABLE DichVu;
IF OBJECT_ID('KhuyenMai', 'U') IS NOT NULL DROP TABLE KhuyenMai;
IF OBJECT_ID('Phim', 'U') IS NOT NULL DROP TABLE Phim;
IF OBJECT_ID('LoaiPhim', 'U') IS NOT NULL DROP TABLE LoaiPhim;
IF OBJECT_ID('KhachHang', 'U') IS NOT NULL DROP TABLE KhachHang;
IF OBJECT_ID('NhanVien', 'U') IS NOT NULL DROP TABLE NhanVien;
IF OBJECT_ID('NguoiDung', 'U') IS NOT NULL DROP TABLE NguoiDung;
GO

CREATE TABLE Phim (
    MaPhim INT NOT NULL IDENTITY(1,1),
    TenPhim NVARCHAR(200) NOT NULL,
    TheLoai NVARCHAR(100) NULL,
    DaoDien NVARCHAR(100) NULL,
    ThoiLuong INT NULL,
    NgayKhoiChieu DATETIME NULL,
    MoTa NVARCHAR(MAX) NULL,
    Poster NVARCHAR(MAX) NULL,
    CONSTRAINT PK_Phim PRIMARY KEY (MaPhim)
);
GO

CREATE TABLE PhongChieu (
    MaPhong INT NOT NULL IDENTITY(1,1),
    TenPhong NVARCHAR(100) NOT NULL,
    LoaiPhong NVARCHAR(20) NULL DEFAULT N'2D',
    SucChua INT NULL DEFAULT 0,
    CONSTRAINT PK_PhongChieu PRIMARY KEY (MaPhong)
);
GO

CREATE TABLE Ghe (
    MaGhe INT NOT NULL IDENTITY(1,1),
    MaPhong INT NULL,
    Hang NVARCHAR(5) NULL,
    SoGhe INT NULL,
    LoaiGhe NVARCHAR(20) NULL DEFAULT N'Thường',
    CONSTRAINT PK_Ghe PRIMARY KEY (MaGhe),
    CONSTRAINT FK_Ghe_PhongChieu FOREIGN KEY (MaPhong) REFERENCES PhongChieu(MaPhong)
);
GO

CREATE TABLE LichChieu (
    MaLich INT NOT NULL IDENTITY(1,1),
    MaPhim INT NULL,
    MaPhong INT NULL,
    NgayChieu DATE NULL,
    GioChieu TIME NULL,
    GiaVe MONEY NULL,
    CONSTRAINT PK_LichChieu PRIMARY KEY (MaLich),
    CONSTRAINT FK_LichChieu_Phim FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim),
    CONSTRAINT FK_LichChieu_PhongChieu FOREIGN KEY (MaPhong) REFERENCES PhongChieu(MaPhong)
);
GO

CREATE TABLE KhachHang (
    MaKH INT NOT NULL IDENTITY(1,1),
    HoTen NVARCHAR(100) NULL,
    SDT VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    NgaySinh DATE NULL,
    DiemTichLuy INT NULL DEFAULT 0,
    TaiKhoan VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    CONSTRAINT PK_KhachHang PRIMARY KEY (MaKH),
    CONSTRAINT UQ_KhachHang_TaiKhoan UNIQUE (TaiKhoan)
);
GO

CREATE TABLE NhanVien (
    MaNV INT NOT NULL IDENTITY(1,1),
    HoTen NVARCHAR(100) NULL,
    ChucVu NVARCHAR(50) NULL,
    TaiKhoan VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    PhanQuyen NVARCHAR(20) NULL DEFAULT N'NhanVien',
    CONSTRAINT PK_NhanVien PRIMARY KEY (MaNV),
    CONSTRAINT UQ_NhanVien_TaiKhoan UNIQUE (TaiKhoan)
);
GO

CREATE TABLE HoaDon (
    MaHD INT NOT NULL IDENTITY(1,1),
    MaKH INT NULL,
    MaNV INT NULL,
    NgayDat DATETIME NULL DEFAULT GETDATE(),
    TongTien MONEY NULL DEFAULT 0,
    TrangThai NVARCHAR(50) NULL DEFAULT N'Chờ thanh toán',
    CONSTRAINT PK_HoaDon PRIMARY KEY (MaHD),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
GO

CREATE TABLE ChiTietHoaDon (
    MaHD INT NOT NULL,
    MaGhe INT NOT NULL,
    MaLich INT NULL,
    GiaVe MONEY NULL,
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaGhe),
    CONSTRAINT FK_CTHD_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_CTHD_Ghe FOREIGN KEY (MaGhe) REFERENCES Ghe(MaGhe),
    CONSTRAINT FK_CTHD_LichChieu FOREIGN KEY (MaLich) REFERENCES LichChieu(MaLich)
);
GO

CREATE TABLE KhuyenMai (
    MaKM INT NOT NULL IDENTITY(1,1),
    TenKM NVARCHAR(200) NOT NULL,
    PhanTramGiam DECIMAL(5,2) NULL DEFAULT 0,
    DieuKien NVARCHAR(500) NULL,
    NgayBatDau DATE NULL,
    NgayKetThuc DATE NULL,
    CONSTRAINT PK_KhuyenMai PRIMARY KEY (MaKM)
);
GO

CREATE TABLE DoAn (
    MaDoAn INT NOT NULL IDENTITY(1,1),
    TenDoAn NVARCHAR(100) NOT NULL,
    Gia MONEY NULL DEFAULT 0,
    Loai NVARCHAR(50) NULL,
    CONSTRAINT PK_DoAn PRIMARY KEY (MaDoAn)
);
GO

CREATE TABLE ChiTietDoAn (
    MaHD INT NOT NULL,
    MaDoAn INT NOT NULL,
    SoLuong INT NULL DEFAULT 1,
    Gia MONEY NULL,
    CONSTRAINT PK_ChiTietDoAn PRIMARY KEY (MaHD, MaDoAn),
    CONSTRAINT FK_CTDoAn_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_CTDoAn_DoAn FOREIGN KEY (MaDoAn) REFERENCES DoAn(MaDoAn)
);
GO

CREATE INDEX IX_Ghe_MaPhong ON Ghe(MaPhong);
CREATE INDEX IX_LichChieu_MaPhim ON LichChieu(MaPhim);
CREATE INDEX IX_LichChieu_MaPhong ON LichChieu(MaPhong);
CREATE INDEX IX_HoaDon_MaKH ON HoaDon(MaKH);
CREATE INDEX IX_HoaDon_MaNV ON HoaDon(MaNV);
CREATE INDEX IX_ChiTietHoaDon_MaLich ON ChiTietHoaDon(MaLich);
CREATE INDEX IX_ChiTietDoAn_MaDoAn ON ChiTietDoAn(MaDoAn);
GO

CREATE VIEW vw_DoanhThuTheoPhim AS
SELECT 
    p.MaPhim,
    p.TenPhim,
    COUNT(DISTINCT hd.MaHD) AS SoLuongHoaDon,
    ISNULL(SUM(hd.TongTien), 0) AS TongDoanhThu
FROM Phim p
LEFT JOIN LichChieu lc ON p.MaPhim = lc.MaPhim
LEFT JOIN ChiTietHoaDon cthd ON lc.MaLich = cthd.MaLich
LEFT JOIN HoaDon hd ON cthd.MaHD = hd.MaHD
GROUP BY p.MaPhim, p.TenPhim;
GO

CREATE FUNCTION fn_TinhTongTienHoaDon(@MaHD INT)
RETURNS MONEY
AS
BEGIN
    DECLARE @TongTienVe MONEY = 0;
    DECLARE @TongTienDoAn MONEY = 0;
    
    SELECT @TongTienVe = ISNULL(SUM(GiaVe), 0) 
    FROM ChiTietHoaDon 
    WHERE MaHD = @MaHD;

    SELECT @TongTienDoAn = ISNULL(SUM(Gia * SoLuong), 0) 
    FROM ChiTietDoAn 
    WHERE MaHD = @MaHD;
    
    RETURN @TongTienVe + @TongTienDoAn;
END
GO

CREATE PROCEDURE sp_LayDanhSachPhimDangChieu
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.MaPhim, 
        p.TenPhim, 
        p.TheLoai,
        p.DaoDien,
        p.ThoiLuong, 
        p.NgayKhoiChieu, 
        p.MoTa,
        p.Poster
    FROM Phim p
    WHERE EXISTS (
        SELECT 1 
        FROM LichChieu lc 
        WHERE lc.MaPhim = p.MaPhim AND CAST(lc.NgayChieu AS DATE) >= CAST(GETDATE() AS DATE)
    );
END
GO

CREATE TRIGGER trg_NganXoaHoaDonCoCTHD
ON HoaDon
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM ChiTietHoaDon cthd
        INNER JOIN deleted d ON cthd.MaHD = d.MaHD
    )
    BEGIN
        RAISERROR(N'Không thể xóa hóa đơn đã có chi tiết đặt vé!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    DELETE HD
    FROM HoaDon HD
    INNER JOIN deleted d ON HD.MaHD = d.MaHD;
END
GO

INSERT INTO NhanVien (HoTen, ChucVu, TaiKhoan, MatKhau, PhanQuyen)
VALUES (N'Admin Hệ Thống', N'Quản lý', 'admin', '$2a$11$e69V9rvzGBm9m1GxlyCkHuCz/hSDW0H7WARXJON0rzrOPt4MdQ5Q6', N'Admin');

INSERT INTO NhanVien (HoTen, ChucVu, TaiKhoan, MatKhau, PhanQuyen)
VALUES (N'Nhân Viên Bán Vé', N'Nhân viên', 'staff', '$2a$11$e69V9rvzGBm9m1GxlyCkHuCz/hSDW0H7WARXJON0rzrOPt4MdQ5Q6', N'NhanVien');
GO

INSERT INTO PhongChieu (TenPhong, LoaiPhong, SucChua) VALUES
(N'Phòng 1', N'2D', 80),
(N'Phòng 2', N'3D', 60),
(N'Phòng 3', N'IMAX', 120);
GO

DECLARE @row CHAR(1), @col INT;
DECLARE @maPhong INT = 1;
SET @row = 'A';
WHILE @row <= 'H'
BEGIN
    SET @col = 1;
    WHILE @col <= 10
    BEGIN
        INSERT INTO Ghe (MaPhong, Hang, SoGhe, LoaiGhe)
        VALUES (@maPhong, @row, @col, 
            CASE WHEN @row IN ('G', 'H') THEN N'VIP' ELSE N'Thường' END);
        SET @col = @col + 1;
    END
    SET @row = CHAR(ASCII(@row) + 1);
END
GO

INSERT INTO DoAn (TenDoAn, Gia, Loai) VALUES
(N'Bắp rang bơ (S)', 35000, N'Bắp rang'),
(N'Bắp rang bơ (L)', 49000, N'Bắp rang'),
(N'Coca Cola', 25000, N'Nước'),
(N'Pepsi', 25000, N'Nước'),
(N'Nước suối', 15000, N'Nước'),
(N'Combo 1 (Bắp S + Coca)', 55000, N'Combo'),
(N'Combo 2 (Bắp L + 2 Coca)', 89000, N'Combo');
GO

INSERT INTO KhuyenMai (TenKM, PhanTramGiam, DieuKien, NgayBatDau, NgayKetThuc) VALUES
(N'Giảm giá sinh nhật', 10.00, N'Áp dụng cho khách hàng có sinh nhật trong tháng', '2026-01-01', '2026-12-31'),
(N'Ưu đãi thành viên mới', 5.00, N'Áp dụng cho đơn hàng đầu tiên', '2026-01-01', '2026-06-30');
GO

PRINT N'=== DATABASE QUẢN LÝ RẠP PHIM ĐÃ ĐƯỢC TẠO THÀNH CÔNG! ===';
PRINT N'Tài khoản Admin:    admin / 123456';
PRINT N'Tài khoản NhanVien: staff / 123456';
GO
