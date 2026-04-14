USE QuanLyRapPhim;
GO

IF OBJECT_ID('vw_DoanhThuTheoPhim', 'V') IS NOT NULL
DROP VIEW vw_DoanhThuTheoPhim;
GO
CREATE VIEW vw_DoanhThuTheoPhim AS
SELECT 
    p.MaPhim,
    p.TenPhim,
    COUNT(DISTINCT hd.MaHD) AS SoLuongHoaDon,
    ISNULL(SUM(hd.TongTien), 0) AS TongDoanhThu
FROM Phim p
LEFT JOIN SuatChieu s ON p.MaPhim = s.MaPhim
LEFT JOIN Ve v ON s.MaSuat = v.MaSuat
LEFT JOIN HoaDon hd ON v.MaHD = hd.MaHD
GROUP BY p.MaPhim, p.TenPhim;
GO

IF OBJECT_ID('fn_TinhTongTienHoaDon', 'FN') IS NOT NULL
DROP FUNCTION fn_TinhTongTienHoaDon;
GO
CREATE FUNCTION fn_TinhTongTienHoaDon(@MaHD INT)
RETURNS MONEY
AS
BEGIN
    DECLARE @TongTienVe MONEY = 0;
    DECLARE @TongTienDV MONEY = 0;
    
    SELECT @TongTienVe = ISNULL(SUM(GiaVe), 0) 
    FROM Ve 
    WHERE MaHD = @MaHD;

    SELECT @TongTienDV = ISNULL(SUM(DonGiaBan * SoLuong), 0) 
    FROM ChiTietDV 
    WHERE MaHD = @MaHD;
    
    RETURN @TongTienVe + @TongTienDV;
END
GO

IF OBJECT_ID('sp_LayDanhSachPhimDangChieu', 'P') IS NOT NULL
DROP PROCEDURE sp_LayDanhSachPhimDangChieu;
GO
CREATE PROCEDURE sp_LayDanhSachPhimDangChieu
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.MaPhim, 
        p.TenPhim, 
        p.ThoiLuong, 
        p.MaLoaiPhim, 
        p.GioiHanTuoi, 
        p.NgayKhoiChieu, 
        p.NgayKetThuc, 
        p.Hinh, 
        p.TrangThai, 
        p.MoTa,
        p.DaoDien,
        p.DienVien,
        p.NamPhatHanh
    FROM Phim p
    WHERE EXISTS (
        SELECT 1 
        FROM SuatChieu s 
        WHERE s.MaPhim = p.MaPhim AND CAST(s.NgayChieu AS DATE) >= CAST(GETDATE() AS DATE)
    );
END
GO

IF OBJECT_ID('trg_NganXoaHoaDonCoVe', 'TR') IS NOT NULL
DROP TRIGGER trg_NganXoaHoaDonCoVe;
GO
CREATE TRIGGER trg_NganXoaHoaDonCoVe
ON HoaDon
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM Ve v
        INNER JOIN deleted d ON v.MaHD = d.MaHD
    )
    BEGIN
        RAISERROR('Không thể xóa hóa đơn đã có dữ liệu xuất vé!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    DELETE HD
    FROM HoaDon HD
    INNER JOIN deleted d ON HD.MaHD = d.MaHD;
END
GO
