# ADR-003: Sử dụng Redis cho Distributed State & SignalR Backplane

Date: 2026-07-10

## Status

Implemented

## Context

Hệ thống cần hỗ trợ scale horizontal (mở rộng ngang với nhiều instance của ứng dụng web) trong môi trường Production. 
Trước đây, các tính năng như khóa ghế khi đặt vé sử dụng `ConcurrentDictionary` lưu trong RAM của một web server duy nhất. Điều này không hoạt động khi có nhiều web server (Load Balancing), vì mỗi server sẽ có một tập hợp ghế khóa riêng biệt, dẫn đến rủi ro "đụng" ghế (race condition).
Hơn nữa, Session và Data Protection Keys cũng cần được chia sẻ đồng bộ giữa các server để đảm bảo trải nghiệm người dùng không bị gián đoạn.

## Decision

Chúng tôi quyết định tích hợp **Redis** vào hệ thống làm bộ nhớ phân tán (Distributed Store) để giải quyết các vấn đề trên:
1. **SignalR Backplane**: Sử dụng Redis để đồng bộ các tin nhắn SignalR giữa nhiều máy chủ, đảm bảo mọi client đều nhận được cập nhật trạng thái sơ đồ ghế thời gian thực (real-time).
2. **Khóa ghế nguyên tử (Atomic Seat Locking)**: Sử dụng cấu trúc dữ liệu Hash của Redis (lệnh `HSETNX`) trong `SeatHub` để khóa ghế. Thao tác này đảm bảo tính nguyên tử và đồng nhất trên toàn cụm server.
3. **Distributed Cache & Session**: Chuyển Session state từ in-memory sang Redis (`AddStackExchangeRedisCache`).
4. **Data Protection**: Lưu trữ các Data Protection Keys trong Redis để tất cả instance có thể giải mã cookie và auth token của nhau.

## Consequences

**Tích cực:**
- Hệ thống hoàn toàn stateless ở tầng Web, sẵn sàng cho việc mở rộng ngang (horizontal scaling).
- Ngăn chặn triệt để tình trạng hai khách hàng cùng lúc đặt một ghế (race condition) trên các web server khác nhau.
- Tăng hiệu suất toàn hệ thống nhờ lưu trữ cache phân tán tốc độ cao.

**Tiêu cực:**
- Tăng độ phức tạp của hạ tầng (phải quản lý, giám sát thêm service Redis).
- Ứng dụng phụ thuộc mạnh vào tính sẵn sàng của Redis. Nếu Redis downtime, tính năng đặt vé real-time sẽ bị ảnh hưởng.
