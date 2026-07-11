# Cinema Management System (Production-Grade)

Hệ thống quản lý rạp chiếu phim toàn diện, xây dựng trên nền tảng ASP.NET Core 8 MVC theo kiến trúc 3 lớp (3-Tier) và phát triển vươn lên Kiến Trúc Phân Tán (Distributed Architecture) hiện đại. Hệ thống được thiết kế theo các mẫu thiết kế (Design Patterns) tiêu chuẩn ngành, bảo đảm tính sẵn sàng cao (High Availability), tính nhất quán (Eventual Consistency), và mở rộng ngang (Horizontal Scalability) dễ dàng.

---

## Các Nâng Cấp & Kiến Trúc Mới Nhất

### 1. Kiến trúc Khả mở (Scalability & Polyglot Persistence)
- **MongoDB Integration (Polyglot Persistence):** Phân tách dữ liệu không có cấu trúc chặt chẽ (như Review, Rating của phim) sang cơ sở dữ liệu NoSQL (MongoDB), giảm tải cho CSDL SQL Server cốt lõi, cải thiện hiệu năng truy xuất các dữ liệu đọc/ghi cường độ cao độc lập.
- **Redis Integration:** Thay thế hoàn toàn in-memory state. Sử dụng Redis cho Distributed Cache, Distributed Session, Data Protection Keys, và đặc biệt là SignalR Backplane (giúp đồng bộ trạng thái đặt ghế real-time xuyên suốt nhiều máy chủ web).
- **SeatHub Rewrite:** Cơ chế khóa ghế chuyển từ local memory sang **Redis Hash (`HSETNX`)** bảo đảm nguyên tử hóa và đồng nhất dữ liệu phân tán.
- **MinIO Object Storage:** Trừu tượng hóa việc lưu trữ qua `IPosterStorageService`. Môi trường Production dùng MinIO (AWS S3-compatible) cho môi trường Web Servers Stateless (phi trạng thái).

### 2. Transaction Management & Sự kiện Phân tán (Saga / Outbox)
- **Transactional Outbox Pattern:** Tích hợp bộ đôi **MassTransit & Entity Framework Core Outbox** để xử lý các giao dịch (ví dụ: Thanh toán VNPay xong, đặt ghế). Tránh tình trạng chẹn luồng khi gọi API VNPay hoặc gửi Email bằng cách xuất log vào bảng Outbox cùng transaction SQL, sau đó MassTransit background worker sẽ tự động đẩy sang RabbitMQ một cách tin cậy.
- **RabbitMQ Message Broker:** Triển khai luồng giao tiếp không đồng bộ qua các Event (ví dụ `InvoiceCreatedEvent`) bằng RabbitMQ. Nếu hệ thống Email hoặc dịch vụ bên ngoài bị chậm, luồng chính của website vẫn không bị ảnh hưởng.

### 3. Tối ưu Hệ thống Kiểm thử (Testing Suite)
- Loại bỏ các lớp mock dữ liệu nguyên khối, áp dụng extension **MockQueryable.EntityFrameworkCore** để cô lập và test các Business Logic (`NhanVienBUS`, `PhimBUS`, vv.) với Entity Framework Core nhanh, chính xác.
- Bổ sung **Security Unit Testing** kiểm định chặt chẽ các rủi ro bảo mật cốt lõi:
  - **SQL Injection Prevention:** Test xác minh ORM layer chặn các chuỗi truy vấn độc hại.
  - **Password Hashing:** Xác minh dữ liệu mật khẩu không được lưu Plain-Text, kiểm tra quá trình Salting và Hashing thông qua `BCrypt.Net`.

---

## DevOps, CI/CD & Giám sát

- **Docker & Docker Compose:** Container hóa 10 dịch vụ: Web, SQL Server, MongoDB, Redis, RabbitMQ, MinIO, Nginx, Prometheus, Grafana, và Backup cron jobs.
- **GitHub Actions CI/CD:**
  - `ci-cd.yml`: Tự động Build, chạy Unit Tests (xUnit), build image và lưu trữ tại GHCR.
  - **Bảo mật Tự động:** Tích hợp **CodeQL** (quét lỗ hổng tĩnh) và **Dependabot** (vá lỗi thư viện).
- **Monitoring & Observability:**
  - **Prometheus + Grafana:** Biểu đồ Real-time theo dõi HTTP metrics, GC, Latency.
  - **Health Checks & Serilog:** Log chuẩn JSON và các endpoints kiểm tra trạng thái (`/healthz`).

---

## Công nghệ Sử dụng

| Lĩnh vực | Công nghệ / Thư viện |
| :--- | :--- |
| **Backend & Framework** | ASP.NET Core 8.0 MVC, SignalR |
| **Cơ sở dữ liệu** | MS SQL Server 2022, MongoDB |
| **Message Broker & Bus**| RabbitMQ, MassTransit |
| **Distributed / Cloud** | Redis, MinIO (S3-compatible) |
| **Bảo mật & Thanh toán** | BCrypt.Net, Google OAuth 2.0, VNPay API |
| **Frontend UI** | Bootstrap 5, jQuery, Chart.js, GSAP, SweetAlert2 |
| **Monitoring** | Prometheus, Grafana, Serilog |
| **Infrastructure / DevOps**| Docker, Docker Compose, Nginx, Certbot, GitHub Actions |
| **Kiểm thử** | xUnit, Moq, MockQueryable |

---

## Hướng dẫn Khởi chạy (Môi trường Dev)

Yêu cầu: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) và Docker.

**1. Clone dự án**
```bash
git clone https://github.com/gnuttt212/CinemaManagementSystem.git
cd CinemaManagementSystem
```

**2. Khởi chạy Docker local**
```bash
docker compose up -d --build
```

Local compose mac dinh chay `cinema-web`, `cinema-db`, va `cinema-db-init`.
Neu branch hien tai bat buoc Redis/RabbitMQ/MongoDB/MinIO, xem them `docs/environment-and-secrets.md` de biet cach bo sung dependency cho staging/production.

**3. Khởi tạo Database & Chạy Website**
```bash
# Update migrations cho SQL Server
dotnet ef database update --project Cinema.DAL --startup-project Cinema.Web

# Chạy ứng dụng
dotnet build
dotnet run --project Cinema.Web
```
Truy cập: `https://localhost:7059/`

---

## Môi trường Production

Triển khai qua `deploy/docker-compose.prod.yml`:
- Môi trường hoàn toàn tách biệt, tự động cấu hình SSL/TLS (Let's Encrypt).
- Tích hợp Reverse Proxy Nginx, Rate Limiting chặn brute-force.
- Xem hướng dẫn chi tiết tại [PRODUCTION.md](deploy/PRODUCTION.md).

---

## Documentation

Detailed technical and operational docs:

- [Documentation Index](docs/README.md)
- [Docker Development Setup](DOCKER.md)
- [CI/CD Pipeline](docs/ci-cd.md)
- [Environment and Secrets](docs/environment-and-secrets.md)
- [Staging Deployment](deploy/README.md)
- [Production Deployment](deploy/PRODUCTION.md)
- [Modular Monolith and Messaging Roadmap](docs/modular-monolith-roadmap.md)
