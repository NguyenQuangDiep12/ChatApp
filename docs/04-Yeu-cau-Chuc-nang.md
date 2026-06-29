# 04 — Yêu cầu chức năng

Ký hiệu: **FR** = Functional Requirement, **NFR** = Non-Functional Requirement.
Cột "Sprint" cho biết sprint dự kiến triển khai (xem [07-Lo-trinh-Sprint](07-Lo-trinh-Sprint.md)).

## 1. Xác thực & Phiên (Auth & Session)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-A1 | Người dùng đăng ký bằng `userName`, `email`, `password`. Email là duy nhất. | 1 |
| FR-A2 | Mật khẩu được hash (BCrypt) trước khi lưu. | 1 |
| FR-A3 | Đăng nhập bằng email + password, trả về **JWT access token**. | 1 |
| FR-A4 | Mỗi lần đăng nhập tạo một `Session` (token + `DeviceInfo`, hạn 7 ngày). | 1 |
| FR-A5 | Đăng xuất → vô hiệu session hiện tại (`RevokeToken`). | 1 |
| FR-A6 | Liệt kê & thu hồi các phiên đang hoạt động của bản thân (đa thiết bị). | 1 |
| FR-A7 | Token hết hạn/không hợp lệ → 401; endpoint yêu cầu xác thực. | 1 |

## 2. Người dùng & Thiết lập

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-U1 | Xem hồ sơ bản thân & người khác (thông tin công khai). | 1 |
| FR-U2 | Cập nhật `userName`, `avatarUrl`. | 1 |
| FR-U3 | Đổi mật khẩu (yêu cầu mật khẩu cũ). | 1 |
| FR-U4 | Quản lý `UserSetting`: `ShowOnlineStatus`, `ShowLastSeen`, `SendReadReceipt`. | 1 |
| FR-U5 | Liệt kê người dùng đang online (tôn trọng `ShowOnlineStatus`). | 4 |

## 3. Phòng chat (Room)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-R1 | Tạo phòng (`GROUP`/`CHANEL`) với tên, mô tả, mức riêng tư. Người tạo là `Owner`. | 2 |
| FR-R2 | Tạo/lấy phòng `DIRECT` 1-1 giữa 2 người (idempotent — không tạo trùng). | 2 |
| FR-R3 | Sửa thông tin phòng (tên, mô tả) — cần quyền. | 2 |
| FR-R4 | Đổi mức riêng tư; nếu `PASSWORD` thì đặt/hash mật khẩu phòng. | 2 |
| FR-R5 | Xóa/lưu trữ phòng — cần quyền `Owner`. | 2 |
| FR-R6 | Liệt kê phòng của user; liệt kê phòng `PUBLIC` để khám phá. | 2 |
| FR-R7 | Vào phòng: `PUBLIC` tự do; `PASSWORD` cần đúng mật khẩu; `PRIVATE` cần link mời. | 2 |

## 4. Thành viên & Link mời

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-M1 | Tham gia/rời phòng; ràng buộc 1 user 1 bản ghi/phòng. | 2 |
| FR-M2 | Liệt kê thành viên của phòng (kèm vai trò). | 2 |
| FR-M3 | Tạo `InviteToken` (giới hạn lượt dùng + thời hạn, mặc định 45 phút / 10 lượt). | 5 |
| FR-M4 | Tham gia qua link mời: kiểm tra `CanBeUsed()`, tăng `UseCount`, gắn `InviteTokenId` vào `RoomMember`. | 5 |
| FR-M5 | Vô hiệu hóa link mời; liệt kê link đang hoạt động của phòng. | 5 |
| FR-M6 | Kick thành viên; đổi vai trò thành viên — cần quyền. | 5 |

## 5. Nhắn tin (Message)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-S1 | Gửi tin nhắn (`Text`/`Image`/`File`) vào phòng mình là thành viên. | 3 |
| FR-S2 | Với `CHANEL`, chỉ user được ủy quyền mới gửi được; member khác chỉ đọc. | 3/5 |
| FR-S3 | Trả lời (reply) một tin nhắn (`ReplyToId`). | 3 |
| FR-S4 | Sửa tin nhắn của chính mình (`Edit`, đánh dấu `IsEdited`). | 3 |
| FR-S5 | Xóa mềm tin nhắn (`Delete`); người có quyền xóa tin của người khác. | 3/5 |
| FR-S6 | Lấy tin nhắn theo phòng có phân trang (`skip`/`take`), mới nhất trước, bỏ tin đã xóa. | 3 |
| FR-S7 | Đính kèm tệp vào tin nhắn (`Attachment`). | 3 |
| FR-S8 | Lấy danh sách attachment theo message / theo loại file trong phòng. | 3 |

## 6. Trạng thái đọc & Đếm chưa đọc

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-D1 | Đánh dấu đã đọc một message (`MessageRead`). | 3 |
| FR-D2 | Đánh dấu đã đọc toàn bộ tin trong phòng (cập nhật `RoomMember.LastReadAt`). | 3 |
| FR-D3 | Đếm số tin chưa đọc của user trong phòng (dựa `LastReadAt`). | 3 |
| FR-D4 | Read receipt chỉ hiển thị nếu người gửi bật `SendReadReceipt`. | 4 |

## 7. Realtime (SignalR)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-RT1 | Client kết nối Hub có xác thực JWT; join nhóm theo `roomId`. | 4 |
| FR-RT2 | Gửi tin → broadcast `ReceiveMessage` tới các thành viên đang online trong phòng. | 4 |
| FR-RT3 | Sự kiện sửa/xóa tin nhắn được phát realtime. | 4 |
| FR-RT4 | Hiện diện: cập nhật `ONLINE`/`OFFLINE` khi connect/disconnect; broadcast trạng thái. | 4 |
| FR-RT5 | Sự kiện "đang gõ" (typing) trong phòng. | 4 |
| FR-RT6 | Thông báo realtime khi có tin mới/được nhắc tới. | 4/6 |

## 8. Thông báo (Notification)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-N1 | Tạo thông báo gắn với message (`Info/Warning/Error/Update`). | 6 |
| FR-N2 | Liệt kê thông báo của user; lọc chưa đọc; đếm chưa đọc. | 6 |
| FR-N3 | Đánh dấu một / tất cả thông báo là đã đọc. | 6 |

## 9. Phân quyền (RBAC)

| ID | Yêu cầu | Sprint |
|----|---------|--------|
| FR-P1 | Seed system roles (`SuperAdmin`, `Moderator`, `User`) & permissions. | 5 |
| FR-P2 | Gán/thu hồi vai trò hệ thống cho user (`UserSystemRole`). | 5 |
| FR-P3 | Mỗi phòng có room roles (`Owner`, `Admin`, `Member`) + role tùy biến. | 5 |
| FR-P4 | Gán quyền cho role (`RolePermission`); kiểm tra `HasPermission(role, code)`. | 5 |
| FR-P5 | Service kiểm tra quyền trước hành động nhạy cảm (xóa tin người khác, kick, đổi role, sửa/xóa phòng). | 5 |

## 10. Yêu cầu phi chức năng (NFR)

| ID | Yêu cầu |
|----|---------|
| NFR-1 | **Bảo mật**: hash mật khẩu (BCrypt), JWT ký HMAC/secret cấu hình qua env, không log secret/token. |
| NFR-2 | **Hiệu năng**: truy vấn timeline dùng index `(RoomId, CreatedAt)`; phân trang bắt buộc; `AsNoTracking` cho read. |
| NFR-3 | **Nhất quán**: mọi thay đổi ghi DB đi qua `UnitOfWork.CommitAsync()`; thao tác nhiều bảng dùng transaction. |
| NFR-4 | **Khả dụng realtime**: hỗ trợ nhiều kết nối/1 user (đa thiết bị) qua nhóm SignalR theo userId. |
| NFR-5 | **Khả bảo trì**: tuân theo quy ước ở [06](06-Quy-uoc-Code-va-Lam-viec-voi-AI.md); API trả lỗi theo định dạng thống nhất. |
| NFR-6 | **Khả kiểm thử**: Service phụ thuộc interface (DI), có unit test cho logic nghiệp vụ quan trọng. |
| NFR-7 | **Cấu hình**: connection string, JWT secret, CORS đọc từ `appsettings`/biến môi trường, không hard-code. |
| NFR-8 | **Validation**: dữ liệu vào được validate (FluentValidation) trước khi xuống Service. |

### Cross-cutting concerns (chi tiết: [08](08-Cross-cutting-va-Phi-chuc-nang.md))

| ID | Yêu cầu |
|----|---------|
| NFR-9 | **Cursor pagination** (keyset) cho list dài (message/search/notification); không dùng `Skip()` trên bảng lớn. |
| NFR-10 | **Rate limiting** cho login/register/send-message/upload; vượt ngưỡng → 429. |
| NFR-11 | **Audit log** cho hành động nhạy cảm (xóa phòng, đổi role/permission, kick...); admin truy vấn được. |
| NFR-12 | **Background jobs** cho việc định kỳ/tốn thời gian (dọn invite/session hết hạn, fan-out notification); không chạy trong request. |
| NFR-13 | **File storage abstraction** `IFileStorage` (Local/MinIO/S3/Azure Blob), đổi provider qua config. |
| NFR-14 | **Cache** (Redis) cho dữ liệu nóng (permission/room/setting/unread count) + invalidation đúng. |
| NFR-15 | **Logging** có cấu trúc (Serilog) + traceId; **Health check** `/health`, `/health/db`, `/health/storage`. |

## 11. Use case chính (tóm tắt luồng)

**UC-1 Đăng ký → đăng nhập**: nhập thông tin → tạo `User` + `UserSetting` mặc định + gán role `User` → đăng nhập → nhận JWT + tạo `Session`.

**UC-2 Tạo nhóm & mời bạn**: tạo `Room(GROUP)` → tạo room roles + gán người tạo làm `Owner` (`RoomMember`) → tạo `InviteToken` → gửi link → người khác mở link → `ValidateAndConsumeToken` → tạo `RoomMember(Member)`.

**UC-3 Chat realtime**: hai client join Hub group `roomId` → A gửi tin (REST hoặc Hub) → Service lưu `Message` → broadcast `ReceiveMessage` → B nhận tức thì → B đọc → `MarkAsRead` + cập nhật `LastReadAt`.

**UC-4 Quản trị phòng**: `Owner/Admin` kick member / đổi role / xóa tin vi phạm — Service kiểm tra `HasPermission` trước khi thực thi.

## 12. Yêu cầu mở rộng — Telegram-like (Sprint 9)

> Chi tiết API: [05 Phần II](05-Thiet-ke-API-va-Realtime.md#phần-ii--tính-năng-mở-rộng-kiểu-telegram-đề-xuất). Schema: [03 §7](03-Mo-hinh-Du-lieu-va-ERD.md#7-đề-xuất-mở-rộng-mô-hình-dữ-liệu-telegram-like).

| ID | Yêu cầu | Gói |
|----|---------|-----|
| FR-X1 | Tìm kiếm tin nhắn / phòng / người dùng / tệp (giới hạn theo quyền) | 9A |
| FR-X2 | Ghim / bỏ ghim tin nhắn; xem danh sách tin ghim của phòng | 9B |
| FR-X3 | Thả / gỡ cảm xúc (reaction emoji) trên tin nhắn | 9B |
| FR-X4 | Mention `@username` → tạo thông báo & lưu liên kết để query nhanh | 9B |
| FR-X5 | Chuyển tiếp (forward) tin nhắn sang phòng khác, giữ nguồn gốc | 9C |
| FR-X6 | Lịch sử chỉnh sửa tin nhắn (edit history) | 9C |
| FR-X7 | Xóa tin 2 chế độ: "for me" và "for everyone" | 9C |
| FR-X8 | Upload avatar người dùng & avatar phòng | 9D |
| FR-X9 | Active sessions hiển thị OS/Browser/IP/Last active; thu hồi từng phiên | 9D |
| FR-X10 | Lưu trữ (archive) & tắt thông báo (mute, có thời hạn) theo từng phòng/người dùng | 9E |
| FR-X11 | Chặn / bỏ chặn người dùng; áp hệ quả (không DM, ẩn trong search) | 9E |
| FR-X12 | Báo cáo (report) người dùng / tin nhắn; admin xử lý | 9E |
| FR-X13 | Xóa tài khoản (xác thực mật khẩu, ẩn danh dữ liệu, thu hồi session) | 9E |
| FR-X14 | Notification đa loại: `NEW_MESSAGE`, `MENTION`, `REACTION`, `ROOM_INVITE`, `ROLE_CHANGED`, `ROOM_UPDATED` | 9F |
| FR-X15 | Tôn trọng mute: không push notification cho phòng đã tắt thông báo | 9F |