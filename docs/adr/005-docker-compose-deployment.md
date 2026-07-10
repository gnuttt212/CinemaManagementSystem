# ADR-005: Triển khai Production với Docker Compose

Date: 2026-07-10

## Status

Implemented

## Context

Hệ thống CinemaManagementSystem đã phát triển thành một kiến trúc phức tạp bao gồm rất nhiều thành phần cần hoạt động cùng lúc: Ứng dụng Web (.NET 8), Cơ sở dữ liệu (SQL Server 2022), Redis, MinIO, Nginx (Reverse Proxy), Certbot (cấp phát SSL tự động), cơ chế Backup dữ liệu, và các công cụ Monitoring (Prometheus, Grafana). 
Việc cài đặt và cấu hình thủ công từng thành phần này trên một máy chủ Linux (chẳng hạn Ubuntu) là công việc rườm rà, dễ xảy ra sai sót (human error), khó đồng bộ môi trường giữa Dev và Prod, và cực kỳ khó khăn nếu cần khôi phục khi máy chủ gặp sự cố vật lý.

## Decision

Chúng tôi quyết định sử dụng **Docker và Docker Compose** làm phương pháp chuẩn mực để đóng gói và triển khai toàn bộ hệ thống ứng dụng trên môi trường Production:
- Mọi thành phần được định nghĩa dưới dạng Service trong tệp mã nguồn `deploy/docker-compose.prod.yml`.
- Hệ thống chạy hoàn toàn khép kín trong một mạng riêng ảo của Docker (`cinema-net`).
- Tích hợp thêm **Nginx** làm Reverse Proxy để định tuyến yêu cầu, xử lý SSL/TLS (với Let's Encrypt qua Certbot) và giới hạn tải (`limit_req` chống DDoS/Brute-force cơ bản).
- Bổ sung một container `cinema-backup` chạy cronjob nội bộ tự động sao lưu CSDL hàng ngày.

## Consequences

**Tích cực:**
- Infrastructure as Code (IaC): Toàn bộ cấu hình máy chủ hạ tầng được lưu trữ dưới dạng mã trong Git, có lịch sử phiên bản.
- Quá trình triển khai cực kỳ nhanh chóng chỉ với một lệnh `docker compose up -d`.
- Đảm bảo tính nhất quán (Consistency) rất cao giữa các môi trường (Dev, Staging, Production).
- Khả năng tự phục hồi của ứng dụng nhờ vào chính sách `restart: unless-stopped`.

**Tiêu cực:**
- Đội ngũ triển khai/vận hành (DevOps) cần trang bị kiến thức chuyên sâu về hệ sinh thái Docker, quản lý Volume, và Linux networking.
- Nếu muốn scale ứng dụng web ra nhiều Node (nhiều máy chủ vật lý) thì Docker Compose sẽ không còn phù hợp, lúc đó cần phải nâng cấp lên kiến trúc Kubernetes (K8s) hoặc Docker Swarm phức tạp hơn nhiều.
