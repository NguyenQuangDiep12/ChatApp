# 01 — Tổng quan & Phạm vi

## 1. Giới thiệu

**ChatApp** là hệ thống nhắn tin (chat) đa nền tảng phục vụ mục đích **học tập**. Hệ thống cho phép người dùng đăng ký tài khoản, tạo/tham gia phòng chat (1-1, nhóm, kênh broadcast), gửi tin nhắn realtime kèm tệp đính kèm, quản lý quyền theo vai trò (RBAC), và nhận thông báo.

## 2. Mục tiêu

| Mục tiêu | Mô tả |
|----------|-------|
| Học kiến trúc N-Layer | Áp dụng Clean/N-Layer Architecture với .NET 8, tách bạch Core/Repository/Service/API |
| Chat realtime | Gửi/nhận tin nhắn tức thời qua WebSocket (SignalR) |
| Phân quyền linh hoạt | RBAC 2 phạm vi: System (toàn hệ thống) và Room (trong phòng) |
| Bảo mật cơ bản | JWT, hash mật khẩu, quản lý session/thiết bị |
| Có thể mở rộng | Cấu trúc rõ ràng để bổ sung tính năng dần theo sprint |

## 3. Đối tượng người dùng (Actor)

- **Khách (Guest)**: chưa đăng nhập — chỉ xem trang đăng ký/đăng nhập, tham gia phòng public qua link mời.
- **Người dùng (User)**: đã đăng nhập — chat, tạo phòng, quản lý hồ sơ & thiết lập.
- **Chủ phòng / Quản trị phòng (Room Owner/Admin)**: quản lý thành viên, vai trò, quyền, link mời trong phòng.
- **Quản trị hệ thống (System Admin/Moderator)**: quản lý người dùng & vai trò ở phạm vi hệ thống.

## 4. Phạm vi

### 4.1. Trong phạm vi (MVP → mở rộng)
- Xác thực & phiên đăng nhập (JWT + refresh/session theo thiết bị).
- Quản lý hồ sơ người dùng & thiết lập riêng tư (online status, last seen, read receipt).
- Phòng chat: tạo/sửa/xóa, 3 loại (`DIRECT`, `GROUP`, `CHANEL`), 3 mức riêng tư (`PUBLIC`, `PRIVATE`, `PASSWORD`).
- Thành viên phòng: tham gia/rời, vai trò trong phòng.
- Link mời (Invite Token) có giới hạn lượt dùng & thời hạn.
- Nhắn tin: text/ảnh/file, trả lời (reply), sửa, xóa mềm, phân trang.
- Tệp đính kèm (Attachment).
- Trạng thái đọc (MessageRead) & đếm tin chưa đọc.
- Realtime: gửi/nhận tin, hiện diện online/offline, "đang gõ".
- Thông báo (Notification).
- RBAC: Role, Permission, RolePermission, gán vai trò hệ thống & phòng.

### 4.2. Ngoài phạm vi (giai đoạn này)
- Gọi thoại / video call.
- Mã hóa đầu-cuối (E2EE).
- Reaction emoji, ghim tin, thread nâng cao.
- Tìm kiếm full-text nâng cao (chỉ làm cơ bản nếu còn thời gian).
- Mobile native app (chỉ web responsive).

## 5. Công nghệ

| Tầng | Công nghệ |
|------|-----------|
| Backend | .NET 8, ASP.NET Core Web API, EF Core 8 |
| Realtime | SignalR (WebSocket) |
| CSDL | PostgreSQL 14 |
| Auth | JWT Bearer + BCrypt (hash mật khẩu) |
| Frontend | ReactJS (Vite) + TailwindCSS |
| Tài liệu API | Swagger / OpenAPI |
| Kiểm thử | xUnit (backend), Vitest/RTL (frontend) |

## 6. Tiêu chí thành công của dự án (Done)

- Người dùng đăng ký/đăng nhập, tạo phòng nhóm, mời người khác và **chat realtime** thành công giữa nhiều client.
- Phân quyền hoạt động: chủ phòng quản lý được thành viên; member thường không có quyền quản trị.
- Backend build & chạy được, có Swagger; migration áp được lên PostgreSQL.
- Có tài liệu (bộ docs này) đồng bộ với code.