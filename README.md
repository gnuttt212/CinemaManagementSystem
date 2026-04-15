# 🎬 Cinema Management System (Hệ Thống Quản Lý Rạp Phim)

## Giới thiệu

Đồ án cuối kỳ môn Lập trình Cơ sở dữ liệu — Xây dựng ứng dụng web quản lý rạp chiếu phim với đầy đủ chức năng: đặt vé, chọn ghế, mua combo bắp nước, quản lý phim/suất chiếu, báo cáo doanh thu.

## Thành viên nhóm

| STT | Họ và Tên       | MSSV       | Vai trò     |
| --- | --------------- | ---------- | ----------- |
| 1   | Trần Thanh Tung | 2351010232 | Trưởng nhóm |
| 2   |                 |            | Thành viên  |
| 3   |                 |            | Thành viên  |
| 4   |                 |            | Thành viên  |
| 5   |                 |            | Thành viên  |
## Phiên bản phần mềm sử dụng

| Phần mềm                            | Phiên bản     |
| ----------------------------------- | ------------- |
| .NET SDK                            | 8.0           |
| ASP.NET Core                        | 8.0           |
| Entity Framework Core               | 8.0.0         |
| SQL Server                          | Express 2019+ |
| SQL Server Management Studio (SSMS) | 19.x          |
| Visual Studio                       | 2022 (17.x)   |
| Bootstrap                           | 5.3.2         |
| jQuery                              | 3.7.1         |
| Chart.js                            | latest        |
| SweetAlert2                         | 11            |

## Kiến trúc dự án (3-Layer Architecture)

```
CinemaManagementSystem/
├── Cinema.DAL/            # Data Access Layer (EF Core DbContext, Models, ADO.NET)
│   ├── Models/            # Entity classes (Database First) + DbContext
│   └── AdoNet/            # ADO.NET thuần (SqlConnection, DataAdapter, DataSet)
├── Cinema.BUS/            # Business Logic Layer (xử lý nghiệp vụ)
├── Cinema.DTO/            # Data Transfer Objects (trao đổi giữa các lớp)
├── Cinema.Web/            # Presentation Layer (ASP.NET Core MVC + Web API)
│   ├── Controllers/       # MVC Controllers (giao diện người dùng)
│   ├── ApiControllers/    # RESTful Web API Controllers (JSON)
│   ├── Areas/Admin/       # Khu vực quản trị
│   └── Views/             # Razor Views
└── DatabaseScripts/       # T-SQL Scripts (View, Function, SP, Trigger)
```

## Các kỹ thuật trọng tâm đã triển khai

### 1. Lập trình CSDL (T-SQL)

- **View:** `vw_DoanhThuTheoPhim` — Thống kê doanh thu theo phim
- **Function:** `fn_TinhTongTienHoaDon` — Tính tổng tiền hóa đơn (Vé + Dịch vụ)
- **Stored Procedure:** `sp_LayDanhSachPhimDangChieu` — Lấy phim đang chiếu
- **Trigger:** `trg_NganXoaHoaDonCoVe` — Ngăn xóa hóa đơn có vé
- **Transaction:** Xử lý giao tác trong `HoaDonBUS.LuuVaThanhToan()` (Commit/Rollback)

### 2. ADO.NET

- Mô hình **kết nối**: `SqlConnection`, `SqlCommand`, `SqlDataReader`
- Mô hình **phi kết nối**: `SqlDataAdapter`, `DataTable`, `DataSet`

### 3. LINQ

- **LINQ to Objects**: Truy vấn dữ liệu xuyên suốt lớp BUS
- **LINQ to XML**: Export/Import danh sách phim qua XML (`XDocument`, `XElement`)

### 4. Entity Framework Core

- **Database First**: Scaffold từ SQL Server (`QuanLyRapPhimContext`)
- **Code First**: Model `NhatKyHeThong` với Migration

### 5. Web API (RESTful)

- `PhimApiController` — CRUD phim (GET/POST/PUT/DELETE), export XML, ADO.NET endpoint
- `DichVuApiController` — CRUD dịch vụ
- Trả dữ liệu định dạng **JSON** chuẩn RESTful

## Hướng dẫn cài đặt & chạy

### Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express (hoặc cao hơn)
- Visual Studio 2022

### Các bước

1. **Clone dự án**

```bash
git clone https://github.com/gnuttt212/CinemaManagementSystem.git
cd CinemaManagementSystem
```

2. **Tạo Database**

- Mở SSMS, tạo database `QuanLyRapPhim`
- Chạy script tạo bảng (nếu có)
- Chạy file `DatabaseScripts/CinemaManagement.sql` để tạo View, Function, SP, Trigger

3. **Cấu hình Connection String**

- Mở file `Cinema.Web/appsettings.json`
- Thay đổi `Server=...` cho phù hợp với máy bạn

4. **Chạy ứng dụng**

```bash
cd Cinema.Web
dotnet run
```

5. **Truy cập**

- Website: `https://localhost:7059`
- Admin: `https://localhost:7059//Admin`
- API: `https://localhost:7059/api/phimapi`
