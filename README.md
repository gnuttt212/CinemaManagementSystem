# Hệ Thống Quản Lý Rạp Phim (Cinema Management System)

## Giới thiệu

Đồ án cuối kỳ môn Lập trình Cơ sở dữ liệu — Xây dựng ứng dụng web quản lý rạp chiếu phim với đầy đủ chức năng: đặt vé, chọn ghế, lọc suất chiếu theo ngày giờ trực quan, quản lý phim/suất chiếu/đồ ăn, báo cáo doanh thu và bảo vệ an toàn danh tính người dùng bằng thuật toán băm mật khẩu BCrypt. 
Dự án đã được khắc phục hoàn toàn các lỗi về logic, đồng bộ hóa 1 file SQL duy nhất và phát triển đầy đủ các tính năng nâng cao.

## Thành viên nhóm

| STT | Họ và Tên       | MSSV       | Vai trò     |
| --- | --------------- | ---------- | ----------- |
| 1   | Trần Thanh Tung | 2351010232 | Trưởng nhóm |
| 2   |                 |            | Thành viên  |
| 3   |                 |            | Thành viên  |

## Phiên bản phần mềm sử dụng

| Phần mềm                            | Phiên bản     |
| ----------------------------------- | ------------- |
| .NET SDK                            | 8.0           |
| ASP.NET Core                        | 8.0           |
| Entity Framework Core               | 8.0.0         |
| SQL Server                          | Express 2019+ |
| Visual Studio                       | 2022 (17.x)   |
| Bootstrap / jQuery / Chart.js       | Mới nhất      |

## Kiến trúc dự án (3-Layer Architecture)

```
CinemaManagementSystem/
├── Cinema.DAL/            # Data Access Layer (EF Core DbContext, Models, ADO.NET)
│   ├── Models/            # Entity classes (Database First) + DbContext
│   └── AdoNet/            # ADO.NET thuần (SqlConnection, DataAdapter, DataSet)
├── Cinema.BUS/            # Business Logic Layer (xử lý nghiệp vụ, Interface + Implementation)
├── Cinema.DTO/            # Data Transfer Objects (trao đổi giữa các lớp)
├── Cinema.Web/            # Presentation Layer (ASP.NET Core MVC + Web API)
│   ├── Controllers/       # MVC Controllers (giao diện người dùng)
│   ├── ApiControllers/    # RESTful Web API Controllers (JSON)
│   ├── Areas/           
│   │   ├── Admin/         # Khu vực Quản lý (Admin)
│   │   └── NhanVien/      # Khu vực Nhân viên (Staff)
│   └── Views/             # Razor Views
└── DatabaseScripts/       # T-SQL Scripts hợp nhất duy nhất
```

## Các kỹ thuật trọng tâm đã triển khai

### 1. Lập trình CSDL (T-SQL)
- **Database Scripts:** Đã hợp nhất vào một file duy nhất `CinemaManagementSystem_Full.sql` (bao gồm schema, seed data đã hash BCrypt sẵn).
- **View:** `vw_DoanhThuTheoPhim` — Thống kê doanh thu theo từng phim.
- **Function:** Tự tính toán doanh thu bán vé trong CSDL.
- **Stored Procedure:** `sp_LayDanhSachPhimDangChieu` — Lấy danh sách phim có lịch chiếu hôm nay.

### 2. Entity Framework Core & Bảo mật
- **Database First:** Sử dụng EF Core thao tác với 10 entity (`Phim`, `LichChieu`, `HoaDon`,...).
- **Code First:** Migration tự động tạo bảng `NhatKyHeThong`.
- **Bảo mật (Authentication):** Quản lý Session chặt chẽ, mật khẩu tài khoản quản trị/nhân viên được mã hóa **BCrypt**. Cấu trúc phân quyền `AdminAuthorize` và `NhanVienAuthorize`.

### 3. ADO.NET (Kết nối và Phi kết nối)
- `SqlDataReader`: Đọc dữ liệu View `vw_DoanhThuTheoPhim` an toàn, có kiểm tra duplicate key cho Chart.js.
- `SqlDataAdapter` & `DataTable`/`DataSet`: Cung cấp dữ liệu dạng bảng/danh sách thông qua Web API (`/api/phimapi/adonet`).

### 4. LINQ & LINQ to XML
- **LINQ to Objects/Entities:** Tối ưu truy vấn dữ liệu từ DB, Eager Loading (`Include`, `ThenInclude`).
- **LINQ to XML:** Có khả năng Export và Import thông tin danh sách phim ra file định dạng XML thông qua Web API.

### 5. Web API (RESTful)
- **PhimApiController:** Full CRUD phim, xuất/nhập XML, cung cấp dữ liệu qua ADO.NET.
- **DoAnApiController:** Full CRUD đồ ăn/dịch vụ.
- **LichChieuApiController:** Full CRUD lịch chiếu (Suất chiếu) với các kiểm tra Foreign Key.

## Hướng dẫn cài đặt & chạy

### Yêu cầu
- .NET 8 SDK
- SQL Server Express

### Các bước
1. **Clone dự án**
```bash
git clone https://github.com/gnuttt212/CinemaManagementSystem.git
cd CinemaManagementSystem
```

2. **Khởi tạo Database**
- Mở SSMS, tạo một cơ sở dữ liệu trống.
- Mở file `DatabaseScripts/CinemaManagementSystem_Full.sql` và chạy toàn bộ (F5) để tự động tạo Tables, Views, Stored Procedures và Seed Data (tài khoản đăng nhập).

3. **Cấu hình Connection String**
- Mở file `Cinema.Web/appsettings.json` và cập nhật `DefaultConnection` trỏ đến DB bạn vừa tạo.

4. **Chạy ứng dụng**
```bash
cd Cinema.Web
dotnet build
dotnet run
```

5. **Truy cập & Tài khoản mặc định**
- **Trang khách hàng:** `https://localhost:7059/`
- **Trang Quản lý (Admin):** `https://localhost:7059/Admin`
- **Trang Nhân viên:** `https://localhost:7059/NhanVien`
- **Tài khoản test:** (Mật khẩu: `123456`)
  - Admin: `admin` / `123456`
  - Nhân viên: `staff` / `123456`
