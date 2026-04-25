# Cinema Management System

Đồ án môn học **Lập trình Cơ sở dữ liệu** — Xây dựng ứng dụng web quản lý rạp chiếu phim toàn diện với đầy đủ chức năng dành cho Khách hàng, Nhân viên và Quản trị viên (Admin). Hệ thống được thiết kế theo kiến trúc **3 lớp (3-Tier Architecture)** chuẩn mực, tối ưu hiệu năng và tích hợp nhiều công nghệ hiện đại.

## Các tính năng nổi bật

### Dành cho Khách hàng

- **Xác thực hiện đại:** Đăng ký, đăng nhập tài khoản an toàn (mật khẩu mã hóa BCrypt), hỗ trợ **đăng nhập một chạm bằng Google OAuth 2.0**.
- **Đặt vé mượt mà:** Xem danh sách phim đang chiếu, sắp chiếu (tải cực nhanh nhờ **IMemoryCache**). Lọc suất chiếu trực quan theo ngày/giờ.
- **Sơ đồ ghế động:** Chọn ghế với sơ đồ phòng chiếu tự động sinh theo sức chứa thực tế.
- **Dịch vụ đi kèm:** Chọn bắp, nước, combo dễ dàng.
- **Thanh toán trực tuyến:** Tích hợp cổng thanh toán **VNPay Sandbox** an toàn, nhanh chóng.
- **Vé điện tử (E-Ticket):** Nhận ngay vé điện tử kèm **mã QR** sau khi thanh toán thành công.
- **Cá nhân hóa:** Quản lý hồ sơ cá nhân, lịch sử giao dịch và đổi mật khẩu an toàn.

### Dành cho Quản trị viên (Admin) / Nhân viên

- **Quản lý danh mục:** Toàn quyền thêm, sửa, xóa Phim (hỗ trợ upload ảnh), Dịch vụ (đồ ăn/thức uống).
- **Quản lý Phòng chiếu & Suất chiếu:** Thiết lập phòng chiếu (tự sinh sơ đồ ghế), sắp xếp lịch chiếu thông minh (tự động tính toán giờ kết thúc dựa trên thời lượng phim).
- **Thống kê & Báo cáo:** Xem dashboard tổng quan, biểu đồ doanh thu theo phim trực quan (Chart.js) và **xuất báo cáo ra file Excel** (ClosedXML).

## Công nghệ sử dụng

| Lĩnh vực                 | Công nghệ / Thư viện                                            |
| :----------------------- | :-------------------------------------------------------------- |
| **Framework Chính**      | ASP.NET Core MVC (.NET 8.0)                                     |
| **Kiến trúc**            | 3-Layer Architecture (Presentation - BUS - DAL)                 |
| **Cơ sở dữ liệu**        | Microsoft SQL Server (T-SQL)                                    |
| **Data Access**          | Entity Framework Core 8.0, ADO.NET (`SqlDataReader`, `DataSet`) |
| **Truy vấn & Export**    | LINQ to Objects, LINQ to Entities, LINQ to XML                  |
| **Giao diện (UI)**       | Razor Views, Bootstrap 5, jQuery, SweetAlert2, Chart.js         |
| **Bảo mật & Thanh toán** | BCrypt.Net, Google OAuth 2.0, VNPay API                         |
| **Tối ưu & Báo cáo**     | IMemoryCache, ClosedXML (Xuất Excel)                            |
| **Kiểm thử (Testing)**   | xUnit, Moq (20 Unit Tests bao phủ tầng BUS)                     |

## Kiến trúc dự án (Solution Structure)

```text
CinemaManagementSystem/
├── Cinema.DAL/            # Data Access Layer (EF Core DbContext, Models, ADO.NET)
├── Cinema.BUS/            # Business Logic Layer (xử lý nghiệp vụ, Interface + Impl)
├── Cinema.DTO/            # Data Transfer Objects (trao đổi giữa các lớp)
├── Cinema.Web/            # Presentation Layer (ASP.NET Core MVC + RESTful Web API)
│   ├── Areas/
│   │   ├── Admin/         # Khu vực Quản lý
│   │   └── NhanVien/      # Khu vực Nhân viên
│   └── ApiControllers/    # Web API endpoints
├── Cinema.Tests/          # Unit Testing (xUnit + Moq, 20 test cases bao phủ tầng BUS)
└── DatabaseScripts/       # T-SQL Scripts hợp nhất duy nhất
```

## Các kỹ thuật T-SQL & CSDL trọng tâm

- **Database Scripts:** Hợp nhất schema và seed data vào 1 file duy nhất `CinemaManagementSystem_Full.sql`.
- **View:** `vw_DoanhThuTheoPhim` thống kê doanh thu phục vụ biểu đồ và Excel.
- **Function & Stored Procedure:** Tính toán tổng tiền hóa đơn, lấy danh sách phim đang chiếu `sp_LayDanhSachPhimDangChieu`.
- **Trigger:** Ngăn chặn xóa hóa đơn sai quy tắc (`trg_NganXoaHoaDonCoCTHD`).
- **Transaction:** Đảm bảo tính nguyên tử (Atomicity) khi thanh toán (Lưu Hóa đơn + Vé + Dịch vụ cùng lúc bằng `BeginTransaction`).

## Hướng dẫn Cài đặt & Chạy dự án

### Yêu cầu hệ thống

- **.NET 8.0 SDK** trở lên
- **SQL Server** (Express hoặc Developer)
- Visual Studio 2022 (khuyên dùng)

### Các bước khởi chạy

1. **Clone dự án**

   ```bash
   git clone https://github.com/gnuttt212/CinemaManagementSystem.git
   cd CinemaManagementSystem
   ```

2. **Khởi tạo Database**
   - Mở SQL Server Management Studio (SSMS), tạo Database mới tên `QuanLyRapPhim`.
   - Mở file `DatabaseScripts/CinemaManagementSystem_Full.sql` và nhấn `F5` để chạy. Script sẽ tự động tạo bảng, View, SP và dữ liệu mẫu (các tài khoản Admin/Nhân viên).

3. **Cấu hình Connection String**
   - Mở file `Cinema.Web/appsettings.json` và cập nhật chuỗi `DefaultConnection` cho phù hợp với SQL Server của máy bạn.

4. **Cấu hình Google OAuth (User Secrets - Tùy chọn nhưng khuyên dùng để test Login Google)**
   Mở terminal tại thư mục `Cinema.Web` và chạy:

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_GOOGLE_CLIENT_ID"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET"
   ```

5. **Build và Chạy**

   ```bash
   dotnet build
   cd Cinema.Web
   dotnet run
   ```

6. **Đường dẫn truy cập**
   - **Khách hàng:** `https://localhost:7059/`
   - **Admin:** `https://localhost:7059/Admin` (Tài khoản/Mật khẩu nằm trong file Script)
   - **Nhân viên:** `https://localhost:7059/NhanVien`

---

_Được phát triển bởi Trần Thanh Tung và các cộng sự (Năm học 2025-2026)._
