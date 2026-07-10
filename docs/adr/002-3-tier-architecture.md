# ADR-002: Áp dụng kiến trúc 3 lớp (3-Tier Architecture)

Date: 2026-07-10

## Status

Implemented

## Context

Hệ thống quản lý rạp chiếu phim (CinemaManagementSystem) yêu cầu một kiến trúc phần mềm rõ ràng để dễ dàng phát triển, bảo trì và kiểm thử. Việc nhồi nhét tất cả logic xử lý vào trong các Controllers của ứng dụng web sẽ dẫn đến mã nguồn khó kiểm soát (spaghetti code), khó tái sử dụng và không thể thực hiện unit test hiệu quả.

## Decision

Chúng tôi quyết định áp dụng kiến trúc 3 lớp (3-Tier Architecture) cho dự án, bao gồm:
1. **Presentation Layer (Cinema.Web)**: Đảm nhiệm UI, HTTP request/response, và SignalR Hubs.
2. **Business Logic Layer (Cinema.BUS)**: Chứa toàn bộ nghiệp vụ của ứng dụng (ví dụ: đăng ký, đăng nhập, xử lý logic đặt vé, khuyến mãi).
3. **Data Access Layer (Cinema.DAL)**: Đảm nhiệm việc giao tiếp với cơ sở dữ liệu SQL Server thông qua Entity Framework Core (chính) và ADO.NET (cho một số truy vấn phức tạp hoặc báo cáo).

Ngoài ra, dự án còn sử dụng một class library chung là **Cinema.DTO (Data Transfer Objects)** để truyền tải dữ liệu giữa các lớp một cách an toàn mà không làm lộ các Entity Model của database.

## Consequences

**Tích cực:**
- Phân tách mối quan tâm (Separation of Concerns) rõ ràng, giúp mã nguồn dễ đọc và bảo trì.
- Lớp Business Logic hoàn toàn độc lập với UI, dễ dàng viết Unit Test (ví dụ: dự án `Cinema.Tests` dùng xUnit và Moq).
- Dễ dàng thay thế công nghệ ở từng lớp (ví dụ: đổi UI từ MVC sang nền tảng khác) mà không ảnh hưởng đến logic cốt lõi.

**Tiêu cực:**
- Tăng số lượng project và file trong Solution, đòi hỏi nỗ lực thiết lập ban đầu.
- Cần ánh xạ (mapping) dữ liệu giữa Entity và DTO, gây tốn một chút thời gian viết code lặp lại.
