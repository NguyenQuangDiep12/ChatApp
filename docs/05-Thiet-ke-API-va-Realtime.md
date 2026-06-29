# 05 — Thiết kế API & Realtime

> Quy ước chung: prefix `/api`, JSON, xác thực `Authorization: Bearer <jwt>`.
> Body request/response dùng **DTO** (không phải entity). Định danh trên URL là `Guid`.

## 1. Định dạng phản hồi & lỗi

**Thành công**: trả thẳng DTO (hoặc danh sách).

**Phân trang** — ưu tiên **cursor (keyset)** cho dữ liệu lớn / cuộn theo thời gian (tin nhắn, search, notification). Xem chi tiết [08 §1](08-Cross-cutting-va-Phi-chuc-nang.md#1-cursor-pagination-keyset).
```json
// Cursor (mặc định cho list dài) — KHÔNG dùng skip/take cho bảng lớn
{ "items": [ ... ], "nextCursor": "0c4f...id-cũ-nhất", "hasMore": true }
```
```json
// Offset (chỉ cho list nhỏ/admin: danh sách phòng, thành viên...)
{ "items": [ ... ], "page": 1, "pageSize": 20, "total": 134 }
```

**Lỗi (thống nhất)** — sinh bởi middleware xử lý exception:
```json
{
  "type": "https://httpstatuses.io/404",
  "title": "NotFound",
  "status": 404,
  "detail": "Room {id} không tồn tại",
  "traceId": "..."
}
```
Ánh xạ exception → HTTP: `NotFoundException`→404, `ForbiddenException`→403, `ConflictException`→409, `ValidationException`→400, `UnauthorizedException`→401, còn lại→500.

## 2. REST Endpoints (theo nhóm)

### Auth — `/api/auth`
| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| POST | `/register` | Đăng ký (userName, email, password) | ❌ |
| POST | `/login` | Đăng nhập → `{ accessToken, expiresAt, user }` | ❌ |
| POST | `/logout` | Thu hồi session hiện tại | ✅ |
| GET | `/sessions` | Danh sách session đang hoạt động | ✅ |
| DELETE | `/sessions/{id}` | Thu hồi 1 session | ✅ |

### Users — `/api/users`
| GET | `/me` | Hồ sơ bản thân | ✅ |
| PUT | `/me` | Cập nhật userName/avatar | ✅ |
| PUT | `/me/password` | Đổi mật khẩu (old, new) | ✅ |
| GET | `/me/settings` | Lấy UserSetting | ✅ |
| PUT | `/me/settings` | Cập nhật UserSetting | ✅ |
| GET | `/{id}` | Hồ sơ công khai của user | ✅ |
| GET | `/online` | Danh sách user online | ✅ |

### Rooms — `/api/rooms`
| POST | `/` | Tạo phòng (GROUP/CHANEL) | ✅ |
| POST | `/direct` | Tạo/lấy phòng DIRECT với `{ targetUserId }` | ✅ |
| GET | `/` | Phòng của tôi | ✅ |
| GET | `/public` | Phòng PUBLIC để khám phá | ✅ |
| GET | `/{id}` | Chi tiết phòng | ✅ (member) |
| PUT | `/{id}` | Sửa tên/mô tả | ✅ (quyền) |
| PUT | `/{id}/privacy` | Đổi privacy (+ password nếu cần) | ✅ (quyền) |
| DELETE | `/{id}` | Xóa/lưu trữ phòng | ✅ (Owner) |
| POST | `/{id}/join` | Vào phòng (PUBLIC / PASSWORD `{ password }`) | ✅ |
| POST | `/{id}/leave` | Rời phòng | ✅ |
| GET | `/{id}/members` | Danh sách thành viên | ✅ (member) |
| DELETE | `/{id}/members/{userId}` | Kick thành viên | ✅ (quyền) |
| PUT | `/{id}/members/{userId}/role` | Đổi role thành viên | ✅ (quyền) |

### Invite Tokens — `/api/rooms/{roomId}/invites` & `/api/invites`
| POST | `/api/rooms/{roomId}/invites` | Tạo link mời (`maxUsage`, `expiresAt`, `note`) | ✅ (quyền) |
| GET | `/api/rooms/{roomId}/invites` | Link đang hoạt động | ✅ (quyền) |
| DELETE | `/api/invites/{id}` | Vô hiệu link | ✅ (quyền) |
| POST | `/api/invites/{token}/accept` | Tham gia phòng qua link | ✅ |

### Messages — `/api/rooms/{roomId}/messages`
| GET | `/` | Tin nhắn phân trang **cursor** `?cursor=&limit=` (xem [08 §1](08-Cross-cutting-va-Phi-chuc-nang.md#1-cursor-pagination-keyset)) | ✅ (member) |
| POST | `/` | Gửi tin (`content`, `type`, `replyToId?`, `attachments?`) | ✅ (member) |
| PUT | `/api/messages/{id}` | Sửa tin (chính chủ) | ✅ |
| DELETE | `/api/messages/{id}` | Xóa mềm (chính chủ / quyền) | ✅ |
| GET | `/api/messages/{id}/replies` | Danh sách reply | ✅ |
| POST | `/api/messages/{id}/read` | Đánh dấu đã đọc | ✅ |
| POST | `/{roomId}/messages/read-all` | Đọc hết phòng (cập nhật LastReadAt) | ✅ |
| GET | `/{roomId}/messages/unread-count` | Số tin chưa đọc | ✅ |

### Attachments — `/api/messages/{messageId}/attachments`
| POST | `/api/uploads` | Upload file → trả `{ fileUrl, fileName, fileType, fileSize }` | ✅ |
| GET | `/api/messages/{messageId}/attachments` | Attachment của message | ✅ |

### Notifications — `/api/notifications`
| GET | `/` | Danh sách (`?unread=true`) | ✅ |
| GET | `/unread-count` | Đếm chưa đọc | ✅ |
| POST | `/{id}/read` | Đánh dấu đã đọc | ✅ |
| POST | `/read-all` | Đánh dấu tất cả đã đọc | ✅ |

### RBAC — `/api/roles`, `/api/permissions`
| GET | `/api/permissions` | Liệt kê permission (`?scope=`) | ✅ (admin) |
| GET | `/api/rooms/{roomId}/roles` | Role của phòng | ✅ (member) |
| POST | `/api/rooms/{roomId}/roles` | Tạo role tùy biến | ✅ (quyền) |
| PUT | `/api/roles/{id}/permissions` | Gán quyền cho role | ✅ (quyền) |
| POST | `/api/users/{userId}/system-roles` | Gán role hệ thống | ✅ (admin) |
| DELETE | `/api/users/{userId}/system-roles/{roleId}` | Thu hồi role hệ thống | ✅ (admin) |

## 3. DTO (gợi ý — đặt ở `ChatApp.Service/DTOs`)

```csharp
// Auth
record RegisterRequest(string UserName, string Email, string Password);
record LoginRequest(string Email, string Password, string? DeviceInfo);
record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);

// User
record UserDto(Guid Id, string UserName, string Email, string AvatarUrl, string Status, DateTime? LastSeen);
record UpdateProfileRequest(string? UserName, string? AvatarUrl);
record UserSettingDto(bool ShowOnlineStatus, bool ShowLastSeen, bool SendReadReceipt);

// Room
record CreateRoomRequest(string Name, string? Description, string RoomType, string PrivacyType, string? Password);
record RoomDto(Guid Id, string Name, string Description, string RoomType, string PrivacyType, Guid CreatedBy, int MemberCount, int UnreadCount);
record RoomMemberDto(Guid UserId, string UserName, string AvatarUrl, Guid RoleId, string RoleName, DateTime JoinedAt);

// Message
record SendMessageRequest(string Content, string Type, Guid? ReplyToId, List<AttachmentDto>? Attachments);
record MessageDto(Guid Id, Guid RoomId, Guid SenderId, string SenderName, string Content, string Type,
                  Guid? ReplyToId, bool IsEdited, bool IsDeleted, List<AttachmentDto> Attachments, DateTime CreatedAt);
record AttachmentDto(Guid Id, string FileUrl, string FileName, string FileType, long FileSize);

// Invite / Notification ...
```

**Quy tắc mapping**: enum ⇄ string ở biên DTO; không lộ `PasswordHash`/`Token` ra response; map thủ công hoặc AutoMapper.

## 4. Realtime — SignalR

### Hub: `ChatHub` (`/hubs/chat`)
Yêu cầu JWT (truyền qua query `access_token` khi handshake WebSocket).

**Vòng đời kết nối**
- `OnConnectedAsync`: lấy `userId` từ claims → join group `user:{userId}` (đa thiết bị) → set `ONLINE` → broadcast presence.
- `OnDisconnectedAsync`: nếu không còn kết nối nào của user → set `OFFLINE` + `LastSeen` → broadcast presence.

**Phương thức client → server (invoke)**
| Method | Tham số | Tác dụng |
|--------|---------|---------|
| `JoinRoom(Guid roomId)` | | Kiểm tra là member → add vào group `room:{roomId}` |
| `LeaveRoom(Guid roomId)` | | Rời group |
| `SendMessage(SendMessageRequest req, Guid roomId)` | | Lưu qua Service → broadcast |
| `Typing(Guid roomId, bool isTyping)` | | Báo đang gõ |
| `MarkRead(Guid roomId)` | | Cập nhật LastReadAt |

**Sự kiện server → client (on)**
| Event | Payload | Ý nghĩa |
|-------|---------|---------|
| `ReceiveMessage` | `MessageDto` | Có tin mới trong phòng |
| `MessageEdited` | `MessageDto` | Tin được sửa |
| `MessageDeleted` | `{ roomId, messageId }` | Tin bị xóa |
| `PresenceChanged` | `{ userId, status, lastSeen }` | Thay đổi hiện diện |
| `UserTyping` | `{ roomId, userId, isTyping }` | Trạng thái gõ |
| `ReadReceipt` | `{ roomId, userId, readAt }` | Đã đọc (nếu bật setting) |
| `NotificationReceived` | `NotificationDto` | Thông báo mới |

### Nhóm (group) quy ước
- `user:{userId}` — tất cả thiết bị của một user (gửi thông báo cá nhân).
- `room:{roomId}` — các thành viên đang mở phòng (broadcast tin nhắn).

### Lưu ý triển khai
- Hub là lớp mỏng: **logic & lưu DB nằm ở Service**, Hub chỉ điều phối + broadcast.
- Khi gửi tin qua REST cũng phải **broadcast qua Hub** (inject `IHubContext<ChatHub>` vào Service hoặc Controller). Chốt một hướng để tránh trùng lặp.
- Presence dùng bộ đếm kết nối/user (in-memory hoặc Redis nếu scale nhiều instance).

---

# Phần II — Tính năng mở rộng kiểu Telegram (đề xuất)

> Các tính năng dưới đây **chưa có trong ERD/API hiện tại**. Mỗi mục nêu: API + realtime + **thay đổi schema bắt buộc**.
> Thay đổi schema chi tiết được tổng hợp ở [03-Mo-hinh-Du-lieu §7](03-Mo-hinh-Du-lieu-va-ERD.md#7-đề-xuất-mở-rộng-mô-hình-dữ-liệu-telegram-like). Lộ trình triển khai ở [07 §Sprint 9](07-Lo-trinh-Sprint.md#sprint-9--tìm-kiếm--tính-năng-nâng-cao-kiểu-telegram).

## 6. Tìm kiếm (Search)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| GET | `/api/search/messages?q=&roomId=&cursor=&limit=` | Tìm tin nhắn (chỉ trong phòng user là thành viên; `roomId` để giới hạn 1 phòng) | ✅ |
| GET | `/api/search/rooms?q=&cursor=&limit=` | Tìm phòng (PUBLIC + phòng user đã tham gia) | ✅ |
| GET | `/api/search/users?q=&cursor=&limit=` | Tìm user theo userName/email (loại user đã chặn mình) | ✅ |
| GET | `/api/search/attachments?q=&roomId=&fileType=` | Tìm tệp theo tên/loại trong phòng | ✅ |

**Schema**: không cần bảng mới. **Hiệu năng**: dùng PostgreSQL `ILIKE` cho MVP; nâng cấp `tsvector` + GIN index khi dữ liệu lớn. Luôn **giới hạn theo quyền thành viên** để tránh lộ dữ liệu.

## 7. Pin / Unpin Message

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| POST | `/api/messages/{id}/pin` | Ghim tin (cần quyền `room.message.pin`) | ✅ |
| DELETE | `/api/messages/{id}/pin` | Bỏ ghim | ✅ |
| GET | `/api/rooms/{roomId}/pinned-messages` | Danh sách tin đã ghim | ✅ (member) |

**Schema**: thêm bảng `pinned_messages(id, room_id, message_id, pinned_by, pinned_at)` (Telegram cho phép nhiều tin ghim/phòng). *Phương án đơn giản*: thêm cột `IsPinned/PinnedBy/PinnedAt` vào `messages`.
**Realtime**: `MessagePinned` / `MessageUnpinned` → group `room:{roomId}`.

## 8. Reaction (👍 ❤️ 😂 😢 😡)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| POST | `/api/messages/{id}/reactions` | Body `{ emoji }` — thêm/cập nhật reaction của mình | ✅ (member) |
| DELETE | `/api/messages/{id}/reactions/{emoji}` | Gỡ reaction | ✅ |
| GET | `/api/messages/{id}/reactions` | Danh sách reaction (gộp theo emoji + đếm) | ✅ (member) |

**Schema**: bảng `message_reactions(id, message_id, user_id, emoji, created_at)`, **unique `(message_id, user_id, emoji)`**.
**Realtime**: `ReactionAdded` / `ReactionRemoved` `{ messageId, userId, emoji }`. Tạo `Notification(REACTION)` cho người gửi tin.

## 9. Mention (@username)

- Khi gửi tin, parse `@username` → resolve ra `userId` → lưu liên kết + tạo `Notification(MENTION)`.
- Tùy chọn: `GET /api/messages/mentions?skip=&take=` — các tin nhắc tới tôi.

**Schema**: bảng `message_mentions(message_id, user_id)` (query nhanh "ai được nhắc"). **Realtime**: `NotificationReceived` tới `user:{mentionedId}`.

## 10. Forward Message

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| POST | `/api/messages/{id}/forward` | Body `{ targetRoomId }` — chuyển tiếp tin sang phòng khác | ✅ (member cả 2 phòng) |

**Schema**: thêm vào `messages`: `ForwardFromMessageId?`, `ForwardFromRoomId?`, `ForwardFromUserId?` (giữ nguồn gốc; khác với `ReplyToId`).
**Realtime**: `ReceiveMessage` tới group phòng đích.

## 11. Edit History

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| GET | `/api/messages/{id}/history` | Lịch sử chỉnh sửa | ✅ (member) |

**Schema**: bảng `message_histories(id, message_id, old_content, edited_at)`. `MessageService.EditMessageAsync` phải **snapshot nội dung cũ** vào bảng này trước khi cập nhật.

## 12. Soft Delete — 2 chế độ (Delete for me / for everyone)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| DELETE | `/api/messages/{id}?scope=me` | Ẩn tin chỉ với bản thân | ✅ |
| DELETE | `/api/messages/{id}?scope=everyone` | Xóa với mọi người (chính chủ / có quyền) | ✅ |

**Schema**: `messages` thêm `DeletedForEveryone (bool)`, `DeletedBy (Guid?)`; thêm bảng `message_hidden(id, user_id, message_id)` cho "delete for me". Query timeline phải lọc cả `IsDeleted`/`DeletedForEveryone` và bản ghi `message_hidden` của user.
**Realtime**: `MessageDeleted { roomId, messageId, scope }` (chỉ broadcast khi `everyone`).

## 13. Typing

Chỉ realtime (đã có ở §4 — `Typing`/`UserTyping`). **Không lưu DB.**

## 14. Active Sessions / Thiết bị (mở rộng `GET /api/auth/sessions`)

`GET /api/auth/sessions` trả thêm: `os`, `browser`, `ipAddress`, `lastActiveAt`, `isCurrent`.
**Schema**: `sessions` thêm `IpAddress`, `LastActiveAt` (và/hoặc parse `DeviceInfo` thành OS/Browser). Cập nhật `LastActiveAt` mỗi request có token.

## 15. Avatar người dùng

| POST | `/api/users/me/avatar` | Upload ảnh (multipart) → cập nhật `AvatarUrl` | ✅ |

Tận dụng `/api/uploads` (Sprint 3) hoặc upload trực tiếp. Không cần schema mới.

## 16. Avatar phòng

| POST | `/api/rooms/{id}/avatar` | Upload ảnh nhóm (cần quyền) | ✅ |

**Schema**: `rooms` thêm cột `AvatarUrl`.

## 17. Archive / Mute phòng (trạng thái theo từng user)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| PUT | `/api/rooms/{id}/state` | Body `{ isArchived?, isMuted?, mutedUntil? }` | ✅ (member) |
| GET | `/api/rooms?filter=archived` | Lọc phòng đã lưu trữ | ✅ |
| POST | `/api/rooms/{id}/mute` | Body `{ until? }` (null = vĩnh viễn) | ✅ |
| DELETE | `/api/rooms/{id}/mute` | Bỏ tắt thông báo | ✅ |

**Schema**: bảng `user_room_states(id, user_id, room_id, is_archived, is_muted, muted_until, updated_at)`, **unique `(user_id, room_id)`**. Khi `is_muted` → bỏ qua push `NotificationReceived` cho user đó.

## 18. Block User

| POST | `/api/users/{id}/block` | Chặn người dùng | ✅ |
| DELETE | `/api/users/{id}/block` | Bỏ chặn | ✅ |
| GET | `/api/users/blocked` | Danh sách đã chặn | ✅ |

**Schema**: bảng `blocked_users(id, user_id, blocked_user_id, created_at)`, unique `(user_id, blocked_user_id)`.
**Hệ quả nghiệp vụ**: người bị chặn không tạo được phòng DIRECT/không gửi DM tới người chặn; ẩn nhau trong search.

## 19. Report User / Message

| POST | `/api/reports` | Body `{ targetUserId?, messageId?, reason }` | ✅ |
| GET | `/api/reports?status=` | Danh sách báo cáo (admin) | ✅ (admin) |
| PUT | `/api/reports/{id}` | Cập nhật trạng thái xử lý (admin) | ✅ (admin) |

**Schema**: bảng `reports(id, reporter_id, target_user_id?, target_message_id?, reason, status, created_at, handled_by?, handled_at?)`.

## 20. Delete Account

| DELETE | `/api/users/me` | Body `{ password }` — xóa/ẩn danh tài khoản | ✅ |

**Hành vi**: xác thực lại mật khẩu → vô hiệu mọi session → ẩn danh dữ liệu (giữ tin nhắn nhưng anonymize) hoặc xóa mềm tài khoản. Cân nhắc cờ `User.IsDeleted` + `DeletedAt` (schema bổ sung).

## 21. Mở rộng loại Notification

`Notification` hiện chỉ có `MessageId` + `Type ∈ {Info,Warning,Error,Update}` — **không đủ** cho các sự kiện domain.

**Đề xuất `NotificationType` mới**: `NEW_MESSAGE`, `ROOM_INVITE`, `MENTION`, `REACTION`, `ROLE_CHANGED`, `ROOM_UPDATED` (giữ `Info/Warning/Error/Update` cho thông báo hệ thống nếu cần).

**Schema `notifications` cần đổi**: `MessageId` → **nullable** (vì `ROOM_INVITE`/`ROLE_CHANGED` không gắn message); thêm `RoomId?`, `ActorId?` (ai gây ra sự kiện), `Data` (JSON payload tùy loại).

**Bảng tổng hợp loại notification → ngữ cảnh**:
| Type | Khi nào | Trường gắn kèm |
|------|---------|----------------|
| `NEW_MESSAGE` | Tin mới trong phòng đang theo dõi | RoomId, MessageId |
| `MENTION` | Bị @nhắc trong tin | RoomId, MessageId, ActorId |
| `REACTION` | Tin của mình được thả cảm xúc | MessageId, ActorId, Data(emoji) |
| `ROOM_INVITE` | Được mời vào phòng | RoomId, ActorId |
| `ROLE_CHANGED` | Vai trò trong phòng bị đổi | RoomId, ActorId, Data(roleName) |
| `ROOM_UPDATED` | Phòng đổi tên/avatar/privacy | RoomId, ActorId |

## 22. Tổng hợp realtime events bổ sung

| Event | Payload | Nhóm nhận |
|-------|---------|-----------|
| `MessagePinned` / `MessageUnpinned` | `{ roomId, messageId, by }` | `room:{roomId}` |
| `ReactionAdded` / `ReactionRemoved` | `{ messageId, userId, emoji }` | `room:{roomId}` |
| `MessageDeleted` | `{ roomId, messageId, scope }` | `room:{roomId}` |
| `NotificationReceived` | `NotificationDto` (đa loại §21) | `user:{userId}` |

---

# Phần III — Vận hành & Cross-cutting (tóm tắt)

> Chi tiết thiết kế ở [08 — Cross-cutting & Phi chức năng](08-Cross-cutting-va-Phi-chuc-nang.md).

## 23. Audit log (admin)
| GET | `/api/audit-logs?action=&roomId=&actorId=&from=&to=&cursor=&limit=` | Lịch sử hành động nhạy cảm | ✅ (admin / Owner phòng) |

## 24. Health check
| GET | `/health` | Liveness | ❌ |
| GET | `/health/db` | Kết nối PostgreSQL | ❌ |
| GET | `/health/storage` | `IFileStorage` (+ Redis nếu có) | ❌ |
| GET | `/health/ready` | Readiness tổng hợp | ❌ |

## 25. Quy ước vận hành áp cho mọi endpoint
- **Rate limiting**: `auth`, `send-message`, `upload`, `global` → vượt ngưỡng trả `429` + `Retry-After` ([08 §2](08-Cross-cutting-va-Phi-chuc-nang.md#2-rate-limiting)).
- **Validation**: input sai → `400` + danh sách field lỗi (FluentValidation, [08 §9](08-Cross-cutting-va-Phi-chuc-nang.md#9-global-validation-fluentvalidation)).
- **Upload**: qua `IFileStorage`, validate loại & dung lượng, không hard-code `wwwroot/uploads` ([08 §5](08-Cross-cutting-va-Phi-chuc-nang.md#5-file-storage-abstraction)).
- **TraceId**: mọi response lỗi kèm `traceId` (Serilog, [08 §7](08-Cross-cutting-va-Phi-chuc-nang.md#7-logging-serilog)).
- **Swagger**: hỗ trợ Authorize JWT ([08 §10](08-Cross-cutting-va-Phi-chuc-nang.md#10-openapi-swagger)).