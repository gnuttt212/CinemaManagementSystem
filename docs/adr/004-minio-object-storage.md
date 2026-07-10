# ADR-004: Sử dụng MinIO cho Object Storage

Date: 2026-07-10

## Status

Implemented

## Context

Ứng dụng cần lưu trữ các tệp tĩnh như poster phim do quản trị viên tải lên. Trong môi trường Development, việc lưu trực tiếp vào thư mục `wwwroot/images/phim` (Local filesystem) là đủ. 
Tuy nhiên, trên môi trường Production với kiến trúc phân tán (có thể scale nhiều web servers chạy song song), việc lưu file cục bộ trên một server sẽ khiến các server khác không thể hiển thị hình ảnh đó, dẫn đến lỗi 404 (Not Found) trên giao diện người dùng.

## Decision

Chúng tôi quyết định áp dụng mẫu thiết kế Strategy thông qua interface `IPosterStorageService` để trừu tượng hóa logic việc lưu trữ file:
- Trên môi trường **Development**: Hệ thống tiếp tục sử dụng `LocalPosterStorageService` để lưu vào ổ cứng cho tiện lợi.
- Trên môi trường **Production**: Quyết định triển khai và sử dụng **MinIO** (hệ thống object storage tương thích với AWS S3) thông qua `MinioPosterStorageService`. MinIO được triển khai cùng cụm dưới dạng một container độc lập.

## Consequences

**Tích cực:**
- Các container Web Server hoàn toàn stateless (không chứa dữ liệu ứng dụng sinh ra lúc runtime).
- Hình ảnh được quản lý tập trung, an toàn, dễ dàng backup và có khả năng scale dung lượng vô hạn.
- Hệ thống đã sẵn sàng 100% để chuyển đổi sang các dịch vụ Public Cloud thực tế như AWS S3 hoặc Google Cloud Storage nếu cần trong tương lai vì MinIO sử dụng chung chuẩn giao tiếp API S3.

**Tiêu cực:**
- Tốn thêm tài nguyên RAM/CPU máy chủ để duy trì dịch vụ MinIO.
- Đội ngũ phát triển cần thiết lập cấu hình và hiểu cách hoạt động cơ bản của S3 API.
