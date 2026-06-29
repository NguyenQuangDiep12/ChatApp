# 07 — Lộ trình Sprint

> Mỗi sprint ~1–2 tuần. Mỗi sprint gồm: **Mục tiêu**, **Phụ thuộc**, **Checklist công việc**, **Definition of Done (DoD)**, **Prompt mẫu cho AI**.
> Thứ tự sprint phản ánh phụ thuộc kỹ thuật — **không nên đảo Sprint 0 và 1**.

## Bản đồ tổng thể

| Sprint | Tên | Mục tiêu cốt lõi |
|--------|-----|------------------|
| 0 | Dọn nợ kỹ thuật & Nền tảng | Build xanh, chuẩn hóa `Guid`, DI, UnitOfWork, middleware lỗi |
| 1 | Xác thực & Người dùng | JWT, đăng ký/đăng nhập, session, hồ sơ, settings |
| 2 | Phòng & Thành viên | CRUD phòng, join/leave, danh sách thành viên |
| 3 | Nhắn tin & Đính kèm | Gửi/sửa/xóa/reply, phân trang, attachment, read state |
| 4 | Realtime (SignalR) | Hub, broadcast tin, presence, typing |
| 5 | RBAC & Link mời | Roles/permissions, kiểm quyền, invite token |
| 6 | Thông báo & Hoàn thiện | Notification, áp dụng setting riêng tư, tìm kiếm cơ bản |
| 7 | Frontend (React) | Scaffold, auth UI, chat UI realtime |
| 8 | Kiểm thử & Hardening | Test, logging, rate limit, CORS, triển khai |
| 9 | Tìm kiếm & Nâng cao kiểu Telegram | Search, pin, reaction, mention, forward, block, archive/mute... |
| 10 | Cache & Mở rộng quy mô | Redis cache + invalidation, SignalR backplane, tối ưu truy vấn |

> **Lưu ý phân bổ**: Sprint 9 gom các tính năng mở rộng (xem [05 Phần II](05-Thiet-ke-API-va-Realtime.md#phần-ii--tính-năng-mở-rộng-kiểu-telegram-đề-xuất)). Một số mục **có thể kéo lên sớm** nếu cần: Search & Avatar (sau Sprint 3), Reaction/Pin/Mention realtime (sau Sprint 4), Notification đa loại (gộp Sprint 6). Sprint 9 đóng vai trò "túi chứa" để không phình các sprint nền tảng.

---

## Sprint 0 — Dọn nợ kỹ thuật & Nền tảng

**Mục tiêu**: toàn solution build xanh, chuẩn hóa khóa `Guid`, hoàn thiện hạ tầng (DI, UnitOfWork, middleware lỗi) để các sprint sau xây tính năng trơn tru.

**Phụ thuộc**: không.

**Checklist**
- [ ] **Chuẩn hóa `Guid`**: đổi mọi `int id` → `Guid` trong `IGenericRepository<T>`, `IService<T>` và tất cả interface service/repository theo entity (`int roomId`→`Guid roomId`, v.v.).
- [ ] Sửa `GenericRepository.GetByIdAsync` nhận `Guid`; bỏ overload `int` thừa ở `UserRepository`.
- [ ] Sửa `GenericRepository.RemoveRange` → `_dbSet.RemoveRange(entities)`.
- [ ] Sửa `MessageRepository.SoftDeleteAsync` dùng hành vi domain `message.Delete()` (không gán private setter).
- [ ] Đổi namespace `NLayerArchitecture.*` → `ChatApp.*` (`IService`, `GenericRepository`, `UnitOfWork` impl...).
- [ ] **Hoàn thiện Unit of Work**: chốt hướng (A: UoW expose repository; B: DI repository riêng + `IUnitOfWork.CommitAsync`). Khuyến nghị **B** cho đơn giản.
- [ ] Tạo `ChatApp.API/Extensions/ServiceCollectionExtensions.cs`: đăng ký toàn bộ Repository, (Service sẽ thêm dần), `IUnitOfWork` — đều **Scoped**.
- [ ] Thêm `ChatApp.Service/Exceptions/` (`AppException`, `NotFoundException`, `ForbiddenException`, `ConflictException`, `ValidationException`).
- [ ] Thêm `ChatApp.API/Middlewares/ExceptionHandlingMiddleware.cs` (map exception → ProblemDetails).
- [ ] Bật CORS (cho phép origin frontend Vite) + cấu hình cơ bản trong `Program.cs`.
- [ ] Verify migration `InitialCreate` áp được lên PostgreSQL sạch (`database update`). Sửa nếu vướng multiple-cascade.
- [ ] Tách connection string nhạy cảm khỏi `appsettings.json` (dùng user-secrets/env); để placeholder trong file commit.
- [ ] **Cross-cutting nền tảng** (xem [08](08-Cross-cutting-va-Phi-chuc-nang.md)): cấu hình **Serilog** (§7) + middleware traceId; **Health check** `/health`,`/health/db` (§8); pipeline **FluentValidation** tự trả 400 (§9); **Swagger** thêm security scheme JWT (§10).

**Definition of Done**
- `dotnet build ChatApp.sln` xanh; `dotnet ef database update` thành công trên DB trống.
- Không còn `int` cho định danh; không còn `NotImplementedException` rơi rớt.
- API chạy, Swagger mở được (dù chưa có controller nghiệp vụ).

**Prompt mẫu**
```
Sprint 0. Đọc docs/02 (mục 5 nợ kỹ thuật) và docs/06.
Task 1: Chuẩn hóa toàn bộ định danh từ int sang Guid trong ChatApp.Core (interfaces) và
ChatApp.Repository (implementations). Sau đó dotnet build và liệt kê file đã sửa.
KHÔNG đổi tên entity/property. Báo cáo DoD.
```

---

## Sprint 1 — Xác thực & Người dùng

**Mục tiêu**: người dùng đăng ký, đăng nhập (JWT), quản lý phiên đa thiết bị, hồ sơ và thiết lập.

**Phụ thuộc**: Sprint 0.

**Checklist**
- [ ] Cấu hình JWT (`Program.cs`): `AddAuthentication().AddJwtBearer(...)`, secret/issuer/audience từ config.
- [ ] Tiện ích: `IPasswordHasher` (BCrypt) + `IJwtTokenGenerator`.
- [ ] Implement `UserService` (register, đổi mật khẩu, update profile, get me/by id, online users, đảm bảo tạo `UserSetting.CreateDefault` khi đăng ký, gán system role `User`).
  - *Lưu ý*: gán role `User` cần seed role (có thể seed tối thiểu ở Sprint 1, hoàn chỉnh ở Sprint 5).
- [ ] Implement `SessionService` (create/get/deactivate/validate token theo `Session`).
- [ ] Implement `UserSettingService`.
- [ ] Implement các repository còn thiếu liên quan (UserSetting, Session) nếu chưa hoàn chỉnh.
- [ ] `AuthController` (register, login, logout, sessions, revoke session).
- [ ] `UsersController` (me, update me, password, settings, get by id, online).
- [ ] Đăng ký DI cho các service mới.
- [ ] DTO + mapping cho User/Auth/Session/Setting.

**Definition of Done**
- Đăng ký → đăng nhập → gọi `GET /api/users/me` với Bearer token thành công.
- Mật khẩu lưu dạng hash; token JWT hợp lệ, hết hạn → 401.
- Logout vô hiệu session; danh sách session phản ánh đúng.

**Prompt mẫu**
```
Sprint 1. Tuân thủ docs/05 (Auth/Users) và docs/06.
Task: Thiết lập JWT + IPasswordHasher(BCrypt) + IJwtTokenGenerator, implement UserService & SessionService,
thêm AuthController với register/login/logout. Trả DTO, dùng UnitOfWork.CommitAsync, đăng ký DI.
Hoàn tất: build xanh + thử login trên Swagger. Báo cáo DoD.
```

---

## Sprint 2 — Phòng & Thành viên

**Mục tiêu**: tạo và quản lý phòng (GROUP/CHANEL/DIRECT), tham gia/rời, danh sách thành viên.

**Phụ thuộc**: Sprint 1 (cần user + auth).

**Checklist**
- [ ] Implement `RoomService`: tạo phòng (người tạo thành `Owner` qua `RoomMember`), DIRECT idempotent, sửa/đổi privacy (+hash password phòng), xóa, list của tôi / public, validate password phòng.
- [ ] Implement `RoomMemberService`: join/leave, list members, đổi role, kiểm tra is-member, cập nhật LastReadAt.
- [ ] Tạo room roles mặc định (`Owner`/`Admin`/`Member`) khi tạo phòng (tạm thời chưa gắn permission — sẽ hoàn ở Sprint 5).
- [ ] `RoomsController` + endpoint members (theo docs/05).
- [ ] Ràng buộc vào phòng theo `PrivacyType` (PUBLIC tự do, PASSWORD cần đúng mật khẩu, PRIVATE chặn — chờ invite ở Sprint 5).
- [ ] DTO Room/RoomMember + mapping (kèm `MemberCount`).
- [ ] Đăng ký DI.

**Definition of Done**
- Tạo phòng GROUP → người tạo là Owner; tạo DIRECT 2 lần với cùng người → trả cùng phòng.
- Vào phòng PUBLIC/PASSWORD đúng luật; list phòng của tôi & public chính xác.
- Rời phòng xóa đúng `RoomMember`.

**Prompt mẫu**
```
Sprint 2. Theo docs/03 (Room/RoomMember/Role) và docs/05 (Rooms).
Task: Implement RoomService & RoomMemberService + RoomsController.
Khi tạo phòng phải tạo room roles Owner/Admin/Member và thêm người tạo làm Owner.
DIRECT phải idempotent. Trả DTO, kiểm quyền cơ bản (chỉ member xem chi tiết). Báo cáo DoD.
```

---

## Sprint 3 — Nhắn tin & Đính kèm

**Mục tiêu**: nhắn tin đầy đủ (gửi/sửa/xóa/reply, phân trang), attachment, trạng thái đọc & đếm chưa đọc. (Chưa realtime — làm ở Sprint 4.)

**Phụ thuộc**: Sprint 2.

**Checklist**
- [ ] Implement `MessageService`: send (kiểm tra là member; với CHANEL kiểm quyền gửi — tạm cho Owner/Admin), edit (chính chủ), soft delete (chính chủ; xóa của người khác để Sprint 5), get by room **cursor pagination** ([08 §1](08-Cross-cutting-va-Phi-chuc-nang.md#1-cursor-pagination-keyset) — đổi chữ ký `(roomId, Guid? cursor, int limit)`), get replies, unread count.
- [ ] Implement `AttachmentService`: upload metadata, list theo message/loại file.
- [ ] **`IFileStorage` abstraction** ([08 §5](08-Cross-cutting-va-Phi-chuc-nang.md#5-file-storage-abstraction)) + `LocalFileStorage` (dev); endpoint `/api/uploads` dùng abstraction, validate loại/dung lượng, **không** hard-code `wwwroot/uploads` → trả URL/key.
- [ ] Implement `MessageReadService`: mark read, mark room read (cập nhật `RoomMember.LastReadAt`), is-read.
- [ ] `MessagesController` + `AttachmentsController` theo docs/05.
- [ ] DTO Message/Attachment + mapping (kèm sender name, attachments).
- [ ] Đăng ký DI.

**Definition of Done**
- Gửi/sửa/xóa/reply hoạt động; tin đã xóa không xuất hiện trong list.
- Phân trang đúng (mới nhất trước); `unread-count` đúng theo `LastReadAt`.
- Đính kèm file gửi & lấy lại được URL.

**Prompt mẫu**
```
Sprint 3. Theo docs/03 (Message/Attachment/MessageRead) và docs/05 (Messages/Attachments).
Task: Implement MessageService/AttachmentService/MessageReadService + controllers.
Soft delete qua message.Delete(); phân trang skip/take; AsNoTracking cho read.
Trả DTO kèm tên người gửi và attachments. Báo cáo DoD.
```

---

## Sprint 4 — Realtime (SignalR)

**Mục tiêu**: tin nhắn, hiện diện, typing realtime giữa nhiều client.

**Phụ thuộc**: Sprint 3.

**Checklist**
- [ ] Cài SignalR; `app.MapHub<ChatHub>("/hubs/chat")`.
- [ ] Xác thực JWT cho Hub (đọc `access_token` từ query khi WebSocket).
- [ ] `ChatHub`: `OnConnected/OnDisconnected` (presence + group `user:{id}`), `JoinRoom/LeaveRoom`, `SendMessage`, `Typing`, `MarkRead`.
- [ ] Cơ chế presence đếm kết nối/user → set `UserStatus` ONLINE/OFFLINE + `LastSeen`.
- [ ] Broadcast khi gửi tin (cả khi gửi qua REST): inject `IHubContext<ChatHub>`; phát `ReceiveMessage`, `MessageEdited`, `MessageDeleted`, `PresenceChanged`, `UserTyping`.
- [ ] Áp dụng quyền: chỉ member mới `JoinRoom`/nhận tin.
- [ ] Tôn trọng `ShowOnlineStatus` khi broadcast presence.

**Definition of Done**
- 2 client trong cùng phòng: A gửi → B nhận tức thì không cần reload.
- Presence cập nhật khi vào/thoát; typing hiển thị.
- Gửi tin qua REST cũng phát realtime (không double-send).

**Prompt mẫu**
```
Sprint 4. Theo docs/05 mục 4 (SignalR).
Task: Thêm ChatHub + JWT cho hub + presence + broadcast. Hub mỏng, logic ở Service.
Đảm bảo gửi tin (REST hoặc Hub) đều broadcast ReceiveMessage đúng group room:{id}. Báo cáo DoD.
```

---

## Sprint 5 — RBAC & Link mời

**Mục tiêu**: phân quyền đầy đủ (system + room) và tham gia phòng qua link mời.

**Phụ thuộc**: Sprint 2 (room/role), Sprint 3 (message để áp quyền xóa).

**Checklist**
- [ ] Seed dữ liệu: system roles (`SuperAdmin/Moderator/User`) + permissions (xem danh sách gợi ý docs/03 mục 4) + map RolePermission mặc định.
- [ ] Gán room roles permission mặc định khi tạo phòng (Owner full, Admin quản trị, Member gửi tin).
- [ ] Implement `RoleService`, `PermissionService`, `RolePermissionService`, `UserSystemRoleService`.
- [ ] Implement `InviteTokenService`: generate, validate-and-consume (`CanBeUsed`/`IncreaseUsage`), list active, deactivate.
- [ ] Cơ chế kiểm quyền dùng được ở Service/Controller: `IPermissionChecker.HasRoomPermission(userId, roomId, code)` + `HasSystemPermission(...)`. Cân nhắc `AuthorizationHandler` policy-based.
- [ ] Áp quyền vào hành động nhạy cảm: sửa/xóa phòng, kick, đổi role, xóa tin người khác, gửi tin trong CHANEL, quản lý role/invite.
- [ ] Controllers: invites, roles, permissions, system-roles (theo docs/05).
- [ ] Endpoint tham gia qua link `/api/invites/{token}/accept` → tạo `RoomMember` gắn `InviteTokenId`.

**Definition of Done**
- Member thường bị 403 khi cố kick/đổi role/xóa tin người khác; Owner/Admin làm được.
- Tạo link mời, dùng link tham gia phòng PRIVATE thành công; hết lượt/hết hạn → từ chối.
- Seed chạy idempotent (chạy lại không nhân đôi).

**Prompt mẫu**
```
Sprint 5. Theo docs/03 (RBAC + seed) và docs/04 (mục 9) + docs/05.
Task: Seed roles/permissions; implement Role/Permission/RolePermission/UserSystemRole/InviteToken service;
thêm IPermissionChecker và áp vào các hành động nhạy cảm; thêm controllers invites/roles.
Báo cáo DoD + liệt kê các điểm đã chèn kiểm quyền.
```

---

## Sprint 6 — Thông báo & Hoàn thiện

**Mục tiêu**: thông báo, áp dụng triệt để thiết lập riêng tư, tìm kiếm cơ bản.

**Phụ thuộc**: Sprint 4 (realtime để push), Sprint 1 (settings).

**Checklist**
- [ ] Implement `NotificationService`: tạo khi có tin nhắn nhắc tới/được reply, list, unread count, mark read/all.
- [ ] Push thông báo realtime qua `user:{id}` (`NotificationReceived`).
- [ ] Áp dụng `UserSetting`: ẩn online status/last seen, chỉ gửi read receipt khi bật.
- [ ] Tìm kiếm cơ bản: tìm phòng public theo tên; (tùy chọn) tìm message trong phòng theo từ khóa.
- [ ] Rà soát & lấp các method interface còn thiếu implement.

**Definition of Done**
- Có thông báo khi được reply/nhắc tới; đếm & đánh dấu đã đọc hoạt động.
- Tôn trọng setting riêng tư trong toàn bộ luồng (REST + realtime).

**Prompt mẫu**
```
Sprint 6. Theo docs/04 (mục 6,8) và docs/05 (Notifications).
Task: Implement NotificationService + push realtime; áp UserSetting vào presence/read-receipt; tìm kiếm phòng cơ bản.
Báo cáo DoD.
```

---

## Sprint 7 — Frontend (React)

**Mục tiêu**: giao diện web hoàn chỉnh, tích hợp REST + SignalR.

**Phụ thuộc**: Sprint 1–6 (API + realtime).

**Checklist**
- [ ] Scaffold Vite + React + TailwindCSS; cấu trúc `features/` (auth, rooms, chat, settings).
- [ ] Lớp API client (axios) + interceptor gắn JWT + xử lý 401.
- [ ] State/auth: lưu token an toàn, route guard.
- [ ] Trang: Đăng ký/Đăng nhập, Danh sách phòng, Phòng chat, Hồ sơ/Thiết lập.
- [ ] Tích hợp SignalR (`@microsoft/signalr`): nhận `ReceiveMessage`, presence, typing; gửi typing/markRead.
- [ ] UI chat: timeline phân trang (infinite scroll), gửi tin, reply, đính kèm, badge chưa đọc.
- [ ] Quản trị phòng: thành viên, vai trò, link mời (tùy quyền).
- [ ] Responsive cơ bản.

**Definition of Done**
- Đăng nhập → chat realtime 2 cửa sổ trình duyệt thấy tin tức thì.
- Badge chưa đọc, presence, typing hiển thị đúng.

**Prompt mẫu**
```
Sprint 7. Frontend React+Vite+Tailwind, tích hợp API docs/05 + SignalR.
Task: Scaffold app + auth flow (login/register, route guard) + màn hình chat realtime cơ bản.
API client gắn JWT, xử lý 401. Báo cáo những màn hình đã chạy được.
```

---

## Sprint 8 — Kiểm thử & Hardening

**Mục tiêu**: chất lượng, bảo mật, sẵn sàng triển khai.

**Phụ thuộc**: tất cả.

**Checklist**
- [ ] Project test `ChatApp.Tests` (xUnit): unit test cho service nghiệp vụ trọng yếu (auth, message, permission, invite).
- [ ] Integration test cho vài endpoint chính (WebApplicationFactory + DB test).
- [ ] Hoàn thiện logging (Serilog đã dựng ở Sprint 0): rà soát không log secret, thêm sink phù hợp production.
- [ ] **Rate limiting** ([08 §2](08-Cross-cutting-va-Phi-chuc-nang.md#2-rate-limiting)) cho auth/send-message/upload; cấu hình CORS chặt theo origin thật.
- [ ] **Audit log** ([08 §3](08-Cross-cutting-va-Phi-chuc-nang.md#3-audit-log)): `IAuditLogger` + bảng `audit_logs` + ghi tại hành động nhạy cảm + endpoint admin.
- [ ] **Background jobs** ([08 §4](08-Cross-cutting-va-Phi-chuc-nang.md#4-background-jobs)): dọn invite/session hết hạn, attachment cũ (BackgroundService/Hangfire).
- [ ] Rà soát bảo mật: kiểm quyền mọi endpoint nhạy cảm, validate input, kích thước/loại file upload.
- [ ] Cấu hình production: env vars, HTTPS, health check.
- [ ] Tài liệu hóa: cập nhật README + bộ docs khớp hành vi cuối.
- [ ] (Tùy chọn) Dockerfile + docker-compose (API + PostgreSQL).

**Definition of Done**
- `dotnet test` xanh; coverage hợp lý cho logic nghiệp vụ.
- Quét nhanh bảo mật không còn lỗ hổng rõ ràng (auth bypass, lộ dữ liệu).
- Chạy được bằng cấu hình production/biến môi trường.

**Prompt mẫu**
```
Sprint 8. Theo docs/04 (NFR) + docs/06.
Task: Thêm ChatApp.Tests với unit test cho MessageService & PermissionChecker; thêm Serilog + rate limit auth;
siết CORS theo env. Báo cáo coverage và DoD.
```

---

## Sprint 9 — Tìm kiếm & Tính năng nâng cao kiểu Telegram

**Mục tiêu**: bổ sung nhóm tính năng nâng cao để trải nghiệm tiệm cận Telegram.

**Phụ thuộc**: Sprint 3 (message), 4 (realtime), 5 (quyền), 6 (notification).

> Chia thành các **gói nhỏ độc lập** — mỗi gói = schema migration riêng + service + endpoint + (realtime nếu có). Có thể làm song song / chọn lọc theo ưu tiên.

**Gói 9A — Tìm kiếm**
- [ ] Search messages/rooms/users/attachments (giới hạn theo quyền thành viên). `ILIKE` cho MVP.
- [ ] `SearchController` theo [05 §6](05-Thiet-ke-API-va-Realtime.md#6-tìm-kiếm-search). *(Không cần schema mới.)*

**Gói 9B — Pin & Reaction & Mention**
- [ ] Bảng `pinned_messages`, `message_reactions`, `message_mentions` + config + migration.
- [ ] Endpoint pin/unpin/list ([§7]), reaction add/remove/list ([§8]), parse mention khi gửi tin ([§9]).
- [ ] Realtime: `MessagePinned/Unpinned`, `ReactionAdded/Removed`; tạo notification `MENTION`/`REACTION`.

**Gói 9C — Forward & Edit history & Soft-delete 2 chế độ**
- [ ] Cột forward + `message_histories` + `message_hidden` + cột `DeletedForEveryone/DeletedBy`.
- [ ] Endpoint forward ([§10]), history ([§11]), delete `?scope=me|everyone` ([§12]).
- [ ] Cập nhật query timeline lọc đúng (xem [03 §7.4](03-Mo-hinh-Du-lieu-va-ERD.md#74-lưu-ý-khi-mở-rộng)).

**Gói 9D — Avatar & Sessions thiết bị**
- [ ] `POST /users/me/avatar`, `Room.AvatarUrl` + `POST /rooms/{id}/avatar`.
- [ ] `Session.IpAddress/LastActiveAt`; `GET /auth/sessions` trả OS/Browser/IP/LastActive/isCurrent.

**Gói 9E — Trạng thái phòng & Quan hệ người dùng**
- [ ] `user_room_states` (archive/mute) + endpoint state/mute ([§17]).
- [ ] `blocked_users` + block/unblock/list ([§18]) + áp hệ quả nghiệp vụ.
- [ ] `reports` + report/list/handle ([§19]).
- [ ] `DELETE /users/me` (delete account, anonymize + revoke sessions) ([§20]).

**Gói 9F — Notification đa loại**
- [ ] Đổi `Notification.MessageId` nullable; thêm `RoomId/ActorId/Data`; mở rộng `NotificationType` ([05 §21]).
- [ ] Phát sinh notification đúng ngữ cảnh (invite, role changed, room updated...) + push realtime + tôn trọng mute.

**Definition of Done**
- Mỗi gói: migration áp được, endpoint hoạt động trên Swagger, realtime (nếu có) phát đúng group, kiểm quyền đầy đủ.
- Timeline vẫn đúng sau khi thêm forward/pin/reaction/soft-delete 2 chế độ.

**Prompt mẫu**
```
Sprint 9 - Gói 9B. Theo docs/03 §7 và docs/05 §7-§9.
Task: Thêm entity message_reactions + config + migration; implement ReactionService; endpoint add/remove/list;
realtime ReactionAdded/Removed; tạo Notification(REACTION). Giữ rich domain, trả DTO, kiểm member. Báo cáo DoD.
```

---

## Sprint 10 — Cache & Mở rộng quy mô

**Mục tiêu**: giảm tải DB và sẵn sàng chạy nhiều instance. Làm khi đã có tải thực / chuẩn bị production.

**Phụ thuộc**: Sprint 4 (realtime), 5 (permission), 6 (notification).

**Checklist** (chi tiết [08 §6](08-Cross-cutting-va-Phi-chuc-nang.md#6-cache-redis))
- [ ] `ICacheService` (get-or-set) trên `IDistributedCache` + Redis; fallback `MemoryCache` ở dev.
- [ ] Cache dữ liệu nóng: permission user-trong-phòng, room info, UserSetting, unread notification count.
- [ ] **Invalidation** tại mọi điểm ghi tương ứng (đổi role/permission, sửa phòng, đổi setting, đọc/ tạo notif).
- [ ] **SignalR Redis backplane** để broadcast đúng khi chạy >1 instance.
- [ ] Presence chuyển sang Redis (thay in-memory) nếu scale ngang.
- [ ] Rà soát & thêm index cho truy vấn chậm (đo bằng log/EF).

**Definition of Done**
- Truy vấn permission/room/setting/unread đọc từ cache, có invalidation đúng (không stale sau khi ghi).
- Chạy 2 instance API: tin nhắn realtime vẫn tới đủ client (backplane hoạt động).

**Prompt mẫu**
```
Sprint 10. Theo docs/08 §6.
Task: Thêm ICacheService + Redis; cache permission check theo (userId, roomId) với invalidation khi đổi role/permission;
bật SignalR Redis backplane. Đảm bảo fallback khi không có Redis ở dev. Báo cáo DoD + các key/TTL đã dùng.
```

---

## Phụ lục — Theo dõi tiến độ

| Sprint | Trạng thái | Ghi chú |
|--------|-----------|---------|
| 0 | ☐ Chưa bắt đầu | |
| 1 | ☐ | |
| 2 | ☐ | |
| 3 | ☐ | |
| 4 | ☐ | |
| 5 | ☐ | |
| 6 | ☐ | |
| 7 | ☐ | |
| 8 | ☐ | |
| 9 | ☐ | Gói 9A–9F |
| 10 | ☐ | Cache & scale |

> Cập nhật cột Trạng thái (☐ → 🟡 đang làm → ✅ xong) sau mỗi sprint.