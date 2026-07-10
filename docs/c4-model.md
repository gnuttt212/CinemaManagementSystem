# Sơ đồ Kiến trúc C4 - Cinema Management System

Tài liệu này mô tả kiến trúc của hệ thống theo mô hình C4 (Context, Container) để cung cấp cái nhìn tổng quan từ người dùng đến các thành phần kỹ thuật.

## 1. Context Diagram (Ngữ cảnh hệ thống)

Sơ đồ này thể hiện hệ thống Cinema Management ở mức tổng quan nhất, cùng với người dùng và các hệ thống bên ngoài mà nó tương tác.

```mermaid
graph TD
    %% Actors
    Customer((Khách hàng))
    Admin((Quản trị viên))
    
    %% System under design
    subgraph SystemUnderDesign [ ]
        CinemaSystem["Cinema Management System\n(Hệ thống quản lý rạp phim)"]
    end
    
    %% External Systems
    VNPay["VNPay API\n(Cổng thanh toán)"]
    GoogleAuth["Google OAuth 2.0\n(Dịch vụ xác thực)"]

    %% Relationships
    Customer -->|Tìm phim, Đặt vé, Thanh toán| CinemaSystem
    Admin -->|Quản lý phim, lịch chiếu, thống kê| CinemaSystem
    
    CinemaSystem -->|Gửi yêu cầu thanh toán| VNPay
    CinemaSystem -->|Xác thực người dùng| GoogleAuth
```

## 2. Container Diagram (Mức Container)

Sơ đồ này đi sâu vào bên trong `Cinema Management System` để xem nó được cấu tạo từ các ứng dụng và nơi lưu trữ dữ liệu nào.

```mermaid
graph TD
    %% Actors
    Customer((Khách hàng))
    Admin((Quản trị viên))

    %% External Systems
    VNPay["VNPay API"]
    GoogleAuth["Google OAuth"]
    
    subgraph CinemaSystem [Cinema Management System]
        WebApp["Web Application\n(ASP.NET Core 8 MVC)"]
        
        DB[/"SQL Server Database\n(Lưu trữ dữ liệu lõi)"/]
        Redis[/"Redis\n(Distributed Cache, Session, SignalR Backplane)"/]
        MinIO[/"MinIO Object Storage\n(Lưu trữ Poster Phim)"/]
        
        BackgroundWorker["Cinema Backup\n(Cron Job / Bash Script)"]
    end

    %% Relationships
    Customer -->|HTTPS / WSS (SignalR)| WebApp
    Admin -->|HTTPS| WebApp
    
    WebApp -->|Đọc/Ghi dữ liệu (EF Core)| DB
    WebApp -->|Lưu Session, Khóa ghế real-time| Redis
    WebApp -->|Upload/Download hình ảnh| MinIO
    
    WebApp -->|Xác thực token| GoogleAuth
    WebApp -->|Tạo giao dịch| VNPay
    
    BackgroundWorker -->|Dump DB hàng ngày| DB
```

## 3. Component Diagram (Mức Thành phần - Ứng dụng Web)

Sơ đồ này mô tả cấu trúc bên trong của ứng dụng Web ASP.NET Core (Kiến trúc 3 lớp).

```mermaid
graph TD
    %% UI Layer
    subgraph PresentationLayer [Presentation Layer - Cinema.Web]
        Controllers["Controllers\n(Xử lý HTTP Requests)"]
        Hubs["SignalR Hubs\n(SeatHub - Xử lý Real-time)"]
        Views["Views\n(Razor Pages, HTML, JS)"]
    end

    %% Business Layer
    subgraph BusinessLayer [Business Logic Layer - Cinema.BUS]
        BUSServices["Business Services\n(KhachHangBUS, PhimBUS, HoaDonBUS)"]
    end

    %% Data Layer
    subgraph DataLayer [Data Access Layer - Cinema.DAL]
        EFCore["Entity Framework Core\n(QuanLyRapPhimContext)"]
        AdoNet["ADO.NET\n(CinemaAdoNetDAL - Queries phức tạp)"]
    end
    
    %% Storage
    DB[/"SQL Server"/]
    Redis[/"Redis"/]

    %% Relationships
    Views -.->|Gửi Request| Controllers
    Views -.->|WebSocket| Hubs
    
    Controllers -->|Gọi nghiệp vụ| BUSServices
    Hubs -->|Đọc/Ghi Hash| Redis
    
    BUSServices -->|Gọi truy vấn| EFCore
    BUSServices -->|Gọi truy vấn| AdoNet
    
    EFCore -->|SQL| DB
    AdoNet -->|SQL| DB
```
