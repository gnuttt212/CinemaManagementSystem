CREATE TABLE [DichVu] (
    [MaDV] int NOT NULL IDENTITY,
    [TenDV] nvarchar(100) NULL,
    [DonGia] money NULL,
    [SoLuongTon] int NULL,
    CONSTRAINT [PK__DichVu__27258657D6403FCF] PRIMARY KEY ([MaDV])
);
GO


CREATE TABLE [LoaiPhim] (
    [MaLoai] int NOT NULL IDENTITY,
    [TenLoai] nvarchar(100) NOT NULL,
    CONSTRAINT [PK__LoaiPhim__730A57599BA3C329] PRIMARY KEY ([MaLoai])
);
GO


CREATE TABLE [NguoiDung] (
    [MaND] int NOT NULL IDENTITY,
    [TaiKhoan] varchar(50) NOT NULL,
    [MatKhau] varchar(255) NOT NULL,
    [HoTen] nvarchar(100) NULL,
    [Email] varchar(100) NULL,
    [IsAdmin] bit NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK__NguoiDun__2725D724C88B3203] PRIMARY KEY ([MaND])
);
GO


CREATE TABLE [Phong] (
    [MaPhong] int NOT NULL,
    [TenPhong] nvarchar(100) NOT NULL,
    [SoLuongGhe] int NULL,
    CONSTRAINT [PK__Phong__20BD5E5B303B82C4] PRIMARY KEY ([MaPhong])
);
GO


CREATE TABLE [Phim] (
    [MaPhim] int NOT NULL IDENTITY,
    [TenPhim] nvarchar(200) NOT NULL,
    [ThoiLuong] int NULL,
    [GioiHanTuoi] int NULL,
    [MaLoaiPhim] int NULL,
    [Hinh] nvarchar(max) NULL,
    [NgayKhoiChieu] datetime NULL,
    CONSTRAINT [PK__Phim__4AC03DE3078CB066] PRIMARY KEY ([MaPhim]),
    CONSTRAINT [FK_Phim_LoaiPhim] FOREIGN KEY ([MaLoaiPhim]) REFERENCES [LoaiPhim] ([MaLoai])
);
GO


CREATE TABLE [HoaDon] (
    [MaHD] int NOT NULL IDENTITY,
    [NgayLap] datetime NULL DEFAULT ((getdate())),
    [TongTien] money NULL DEFAULT 0.0,
    [MaND] int NULL,
    CONSTRAINT [PK__HoaDon__2725A6E0E68C72B0] PRIMARY KEY ([MaHD]),
    CONSTRAINT [FK_HoaDon_NguoiDung] FOREIGN KEY ([MaND]) REFERENCES [NguoiDung] ([MaND])
);
GO


CREATE TABLE [Ghe] (
    [MaGhe] int NOT NULL IDENTITY,
    [TenGhe] varchar(10) NULL,
    [LoaiGhe] nvarchar(20) NULL,
    [MaPhong] int NULL,
    CONSTRAINT [DF__Ghe__DaDat__3D5E1FD2] PRIMARY KEY ([MaGhe]),
    CONSTRAINT [FK_Ghe_Phong] FOREIGN KEY ([MaPhong]) REFERENCES [Phong] ([MaPhong])
);
GO


CREATE TABLE [SuatChieu] (
    [MaSuat] int NOT NULL IDENTITY,
    [MaPhim] int NULL,
    [NgayChieu] date NULL,
    [GioBatDau] time NULL,
    [GiaVe] money NULL,
    [MaPhong] int NULL,
    [GioKetThuc] time NULL,
    [TrangThai] nvarchar(50) NULL DEFAULT N'Sắp chiếu',
    CONSTRAINT [PK__SuatChie__A69D0241CD876E94] PRIMARY KEY ([MaSuat]),
    CONSTRAINT [FK_SuatChieu_Phong] FOREIGN KEY ([MaPhong]) REFERENCES [Phong] ([MaPhong]),
    CONSTRAINT [FK__SuatChieu__MaPhi__398D8EEE] FOREIGN KEY ([MaPhim]) REFERENCES [Phim] ([MaPhim])
);
GO


CREATE TABLE [ChiTietDV] (
    [MaHD] int NOT NULL,
    [MaDV] int NOT NULL,
    [SoLuong] int NULL,
    [DonGiaBan] money NULL,
    CONSTRAINT [PK__ChiTietD__4557FE856337680E] PRIMARY KEY ([MaHD], [MaDV]),
    CONSTRAINT [FK__ChiTietDV__MaDV__46E78A0C] FOREIGN KEY ([MaDV]) REFERENCES [DichVu] ([MaDV]),
    CONSTRAINT [FK__ChiTietDV__MaHD__45F365D3] FOREIGN KEY ([MaHD]) REFERENCES [HoaDon] ([MaHD])
);
GO


CREATE TABLE [Ve] (
    [MaVe] int NOT NULL IDENTITY,
    [MaSuat] int NULL,
    [MaGhe] int NULL,
    [MaHD] int NULL,
    [GiaVe] decimal(18,2) NULL,
    CONSTRAINT [PK__Ve__2725100F68C2289C] PRIMARY KEY ([MaVe]),
    CONSTRAINT [FK_Ve_Ghe] FOREIGN KEY ([MaGhe]) REFERENCES [Ghe] ([MaGhe]),
    CONSTRAINT [FK__Ve__MaHD__4F7CD00D] FOREIGN KEY ([MaHD]) REFERENCES [HoaDon] ([MaHD]),
    CONSTRAINT [FK__Ve__MaSuat__4D94879B] FOREIGN KEY ([MaSuat]) REFERENCES [SuatChieu] ([MaSuat])
);
GO


CREATE INDEX [IX_ChiTietDV_MaDV] ON [ChiTietDV] ([MaDV]);
GO


CREATE INDEX [IX_Ghe_MaPhong] ON [Ghe] ([MaPhong]);
GO


CREATE INDEX [IX_HoaDon_MaND] ON [HoaDon] ([MaND]);
GO


CREATE UNIQUE INDEX [UQ__NguoiDun__D5B8C7F0B65F44B0] ON [NguoiDung] ([TaiKhoan]);
GO


CREATE INDEX [IX_Phim_MaLoaiPhim] ON [Phim] ([MaLoaiPhim]);
GO


CREATE INDEX [IX_SuatChieu_MaPhim] ON [SuatChieu] ([MaPhim]);
GO


CREATE INDEX [IX_SuatChieu_MaPhong] ON [SuatChieu] ([MaPhong]);
GO


CREATE INDEX [IX_Ve_MaGhe] ON [Ve] ([MaGhe]);
GO


CREATE INDEX [IX_Ve_MaHD] ON [Ve] ([MaHD]);
GO


CREATE UNIQUE INDEX [UC_SuatChieu_Ghe] ON [Ve] ([MaSuat], [MaGhe]) WHERE [MaSuat] IS NOT NULL AND [MaGhe] IS NOT NULL;
GO


