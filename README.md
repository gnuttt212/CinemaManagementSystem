# Cinema Management System (Production-Grade)

> **Hệ thống quản lý rạp chiếu phim toàn diện**, xây dựng trên nền tảng **ASP.NET Core 8 MVC** theo kiến trúc **3 lớp (3-Tier)**. Gần đây, hệ thống đã được nâng cấp mạnh mẽ với kiến trúc phân tán (Distributed Architecture), sẵn sàng **scale horizontal** (mở rộng ngang) cho môi trường Production thực tế.

---

## 🚀 Tính năng Nổi bật (Cập nhật mới)

Hệ thống giờ đây không chỉ là một đồ án môn học mà đã được trang bị các tiêu chuẩn của một ứng dụng doanh nghiệp (Enterprise-level):

### Kiến trúc Khả mở (Scalability & Reliability)
- **Redis Integration**: Thay thế hoàn toàn in-memory state. Sử dụng Redis cho Distributed Cache, Distributed Session, Data Protection Keys, và đặc biệt là **SignalR Backplane** (cho phép nhiều web server cùng đồng bộ trạng thái khóa ghế real-time).
- **MinIO Object Storage**: Trừu tượng hóa việc lưu trữ poster phim với giao diện `IPosterStorageService`. Môi trường Production sử dụng MinIO (tương thích AWS S3) thay vì lưu file local, giúp stateless web servers.
- **SeatHub Rewrite**: Cơ chế khóa ghế chuyển từ `ConcurrentDictionary` (local memory) sang **Redis Hash (`HSETNX`)** đảm bảo tính nguyên tử (atomic) và đồng nhất trên toàn cụm server.

### DevOps & CI/CD
- **Docker & Docker Compose**: Đóng gói toàn bộ ứng dụng thành 10 containers độc lập (Web, DB, Redis, MinIO, Nginx, Certbot, Prometheus, Grafana, Backup, DB-Init).
- **Nginx Reverse Proxy**: Tích hợp SSL/TLS (Let's Encrypt qua Certbot), bảo vệ chống brute-force đăng nhập (`limit_req`), và cấu hình security headers nghiêm ngặt.
- **GitHub Actions CI/CD**:
  - `ci-cd.yml`: Tự động Build, chạy Unit Tests (xUnit), build Docker image lên GHCR (GitHub Container Registry).
  - Tự động Deploy lên môi trường Staging & Production qua SSH.
- **Bảo mật Tự động**: Tích hợp **CodeQL** quét lỗ hổng mã nguồn và **Dependabot** tự động cập nhật thư viện cũ.
- **Automated Backups**: Container chuyên dụng tự động backup Database SQL Server định kỳ (Cron job) hàng ngày và cơ chế xoay vòng backup (Retention 7 ngày).

### Monitoring & Observability
- **Prometheus**: Tự động thu thập (scrape) HTTP metrics (tốc độ phản hồi, error rate, v.v.) và hệ thống (memory, GC, threadpool) thông qua `prometheus-net`.
- **Grafana**: Dashboard trực quan hóa dữ liệu real-time với 8 panels chính (Request Rate, 5xx Errors, P95 Latency, GC Collections, v.v.).
- **Alerting Rules**: Các quy tắc cảnh báo (Alert) cho HTTP 5xx cao, App down, hoặc tràn RAM.
- **Structured Logging & Health Checks**: Tích hợp Serilog (xuất log chuẩn JSON) và các endpoint `/healthz`, `/healthz/ready` kiểm tra trạng thái của cả SQL Server, Redis và MinIO.

---

## 💻 Tính năng Cốt lõi của Ứng dụng

### Dành cho Khách hàng
- **Xác thực hiện đại**: Đăng nhập qua **Google OAuth 2.0**, mật khẩu mã hóa BCrypt an toàn.
- **Đặt vé & Chọn ghế Real-time**: Sơ đồ ghế đồng bộ thời gian thực cho mọi khách hàng nhờ SignalR + Redis. Tránh triệt để tình trạng "đụng" ghế.
- **Thanh toán VNPay**: Tích hợp VNPay Sandbox, đảm bảo giao dịch (Transaction) nguyên tử.
- **E-Ticket (Vé điện tử)**: Cấp vé kèm mã **QR Code** ngay sau khi thanh toán.

### Dành cho Quản trị viên & Nhân viên
- **Quản lý toàn diện**: CRUD phim, phòng chiếu, sinh sơ đồ ghế tự động, đồ ăn thức uống, khuyến mãi.
- **Báo cáo Thống kê**: Biểu đồ trực quan (Chart.js) thống kê doanh thu theo phim/thời gian, hỗ trợ xuất báo cáo ra **Excel** (ClosedXML).
- **Phân quyền chặt chẽ**: Dashboard riêng biệt cho Admin và Staff.

---

## 🛠 Công nghệ Sử dụng

| Lĩnh vực | Công nghệ / Thư viện |
| :--- | :--- |
| **Backend & Framework** | ASP.NET Core 8.0 MVC, SignalR |
| **Cơ sở dữ liệu** | Microsoft SQL Server 2022 |
| **ORM & Data Access** | Entity Framework Core 8, ADO.NET (`SqlDataReader`) |
| **Distributed / Cloud** | Redis, MinIO (S3-compatible) |
| **Bảo mật & Thanh toán** | BCrypt.Net, Google OAuth 2.0, VNPay API |
| **Frontend UI** | Bootstrap 5, jQuery, Chart.js, GSAP, SweetAlert2 |
| **Monitoring** | Prometheus, Grafana, Serilog |
| **Infrastructure / DevOps**| Docker, Docker Compose, Nginx, Certbot, GitHub Actions |
| **Kiểm thử** | xUnit, Moq (20+ Unit Tests bao phủ BUS) |

---

## 🏗 Kiến trúc Dự án (3-Layer)

```text
CinemaManagementSystem/
├── Cinema.DAL/                    # Data Access Layer (EF Core + ADO.NET)
├── Cinema.BUS/                    # Business Logic Layer (Nghiệp vụ + Cache)
├── Cinema.DTO/                    # Data Transfer Objects
├── Cinema.Web/                    # Presentation Layer (MVC + Web API + SignalR)
│   ├── Hubs/SeatHub.cs            # Xử lý ghế Real-time (Redis backed)
│   ├── Services/                  # MinioPosterStorageService, Local...
│   ├── Program.cs                 # DI config: Redis, Serilog, Prometheus...
│   └── Areas/                     # Admin & NhanVien Modules
├── Cinema.Tests/                  # Unit Testing (xUnit)
├── deploy/                        # Infrastructure as Code (Docker, Nginx, Configs)
│   ├── docker-compose.prod.yml    # Production Stack (10 services)
│   ├── nginx/                     # Nginx Reverse Proxy Configs
│   ├── monitoring/                # Prometheus & Grafana configs
│   └── backup/                    # Auto DB Backup Scripts
└── .github/workflows/             # CI/CD Pipelines & Security Scanning
```

---

## ⚙️ Hướng dẫn Cài đặt & Chạy (Môi trường Dev)

Yêu cầu: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), SQL Server, Redis (hoặc dùng Docker).

**1. Clone dự án & Init Database**
```bash
git clone https://github.com/gnuttt212/CinemaManagementSystem.git
# Chạy file script DatabaseScripts/CinemaManagementSystem_Full.sql trong SSMS
```

**2. Khởi chạy Redis & MinIO cục bộ (Dùng Docker)**
```bash
# Ở thư mục gốc, có thể dùng file docker-compose.yml dành cho môi trường Dev
docker-compose up -d cinema-redis cinema-minio
```

**3. Cấu hình Secrets (Tùy chọn cho Dev)**
```bash
cd Cinema.Web
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
```

**4. Build & Chạy ứng dụng**
```bash
dotnet build
dotnet run
```
Truy cập: `https://localhost:7059/`

---

## 🚢 Triển khai Production (Docker Compose)

Hệ thống cung cấp sẵn file `deploy/docker-compose.prod.yml` chạy hoàn toàn khép kín.

1. **Chuẩn bị server Ubuntu/Linux**, cài đặt Docker & Git.
2. Copy thư mục `deploy/` lên server.
3. Chép file `deploy/.env.prod.example` thành `.env.prod` và điền đủ thông tin (Passwords, API Keys, Domain).
4. Khởi chạy toàn bộ hệ thống:
   ```bash
   cd deploy
   docker compose -f docker-compose.prod.yml up -d
   ```
5. Đợi SSL tự động cấp phát qua Let's Encrypt, truy cập domain của bạn. Check trạng thái: `https://yourdomain.com/healthz/ready`.

Xem chi tiết trong tài liệu [PRODUCTION.md](deploy/PRODUCTION.md).

---

## 🔑 Tài khoản Test Mặc định

Đã được seed sẵn trong script Database:
- **Admin**: `admin / 123456`
- **Nhân viên**: `nhanvien / 123456`
- **Khách hàng**: Tự đăng ký qua Form hoặc đăng nhập Google OAuth.