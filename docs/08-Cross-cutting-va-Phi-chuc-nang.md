# 08 — Cross-cutting concerns & Chất lượng backend

> Các mối quan tâm **xuyên suốt** mọi tính năng. Đây là "tư duy backend" nền tảng — nên được thiết kế **một lần, dùng mọi nơi**, không rải rác trong từng controller/service.
> Triển khai theo [07 §Sprint 0 & §Sprint 8 & §Sprint 10](07-Lo-trinh-Sprint.md).

## Bảng tổng quan

| # | Concern | Thư viện đề xuất (.NET 8) | Áp dụng từ |
|---|---------|--------------------------|-----------|
| 1 | Cursor pagination | (keyset thủ công) | Sprint 3 |
| 2 | Rate limiting | `Microsoft.AspNetCore.RateLimiting` (built-in) | Sprint 8 |
| 3 | Audit log | Custom + EF interceptor | Sprint 8 |
| 4 | Background jobs | `BackgroundService` / Hangfire / Quartz | Sprint 8/10 |
| 5 | File storage abstraction | `IFileStorage` (Local / MinIO / S3 / Azure Blob) | Sprint 3 |
| 6 | Cache | `IDistributedCache` + Redis (`StackExchange.Redis`) | Sprint 10 |
| 7 | Logging | Serilog (`Serilog.AspNetCore`) | Sprint 0 |
| 8 | Health check | `AspNetCore.HealthChecks.*` | Sprint 0 |
| 9 | Global validation | FluentValidation | Sprint 0/1 |
| 10 | OpenAPI | Swashbuckle (Swagger) | Sprint 0 |

---

## 1. Cursor Pagination (keyset)

**Vấn đề**: `Skip()/Take()` (offset) chậm dần và sai lệch khi dữ liệu lớn / có chèn tin mới (drift). Chat luôn cuộn ngược thời gian → hợp với **keyset pagination**.

**Hợp đồng API** (thay cho `skip/take` ở message/search):
```
GET /api/rooms/{roomId}/messages?cursor={lastMessageId}&limit=30   # tải trang cũ hơn
→ { items: [...], nextCursor: "<id-cũ-nhất-trang-này>", hasMore: true }
```
- `cursor` rỗng = trang mới nhất. Mỗi lần cuộn lên truyền `nextCursor` của trang trước.
- Có thể dùng `(CreatedAt, Id)` làm khóa keyset để ổn định khi trùng thời điểm.

**Truy vấn (mẫu)**:
```csharp
var q = _ctx.Messages.Where(m => m.RoomId == roomId && !m.IsDeleted);
if (cursor is Guid c) {
    var anchor = await _ctx.Messages.Where(m => m.Id == c)
                    .Select(m => new { m.CreatedAt, m.Id }).FirstAsync();
    q = q.Where(m => m.CreatedAt < anchor.CreatedAt
                  || (m.CreatedAt == anchor.CreatedAt && m.Id.CompareTo(anchor.Id) < 0));
}
var items = await q.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id)
                   .Take(limit + 1).AsNoTracking().ToListAsync();
var hasMore = items.Count > limit;   // lấy dư 1 để biết còn trang
```
**Thay đổi cần làm**: đổi `IMessageService.GetMessagesByRoomIdAsync(roomId, skip, take)` → `(...roomId, Guid? cursor, int limit)`. Tựa nguyên tắc cho search & danh sách dài khác. Dựa index `(RoomId, CreatedAt)` đã có.

**DoD**: endpoint message dùng cursor; không còn `Skip()` trên bảng lớn; tải trang ổn định khi có tin mới chèn vào.

## 2. Rate Limiting

**Mục tiêu**: chống brute-force & spam. .NET 8 có rate limiter tích hợp (`builder.Services.AddRateLimiter(...)` + `app.UseRateLimiter()`).

**Chính sách đề xuất (policy đặt tên, gắn `[EnableRateLimiting("...")]`)**:
| Policy | Phạm vi | Giới hạn gợi ý |
|--------|---------|----------------|
| `auth` | login + register (theo IP) | 5–10 req / phút |
| `send-message` | gửi tin (theo userId) | ~20 req / 10 giây |
| `upload` | upload file (theo userId) | ~10 req / phút |
| `global` | mặc định toàn API (theo IP/user) | hợp lý, tránh DoS |

- Trả `429 Too Many Requests` + header `Retry-After`.
- Khóa theo `userId` (đã auth) hoặc IP (chưa auth). Cấu hình ngưỡng qua `appsettings`.

**DoD**: vượt ngưỡng login/send/upload → 429; ngưỡng đọc từ config.

## 3. Audit Log

**Mục tiêu**: ghi lại hành động nhạy cảm để admin truy vết. Khác `message_histories` (chỉ cho nội dung tin).

**Sự kiện cần ghi**: `ROOM_DELETED`, `ROOM_UPDATED`, `ROLE_CHANGED`, `PERMISSION_CHANGED`, `MEMBER_KICKED`, `MEMBER_ROLE_CHANGED`, `INVITE_CREATED/REVOKED`, `USER_BANNED`, `REPORT_HANDLED`, `LOGIN`/`LOGIN_FAILED` (tùy chọn).

**Bảng** `audit_logs` (xem [03 §7.2](03-Mo-hinh-Du-lieu-va-ERD.md#72-bảng-entity-mới)):
```
Id, ActorId, Action, EntityType, EntityId,
RoomId?, Data (jsonb: before/after), IpAddress?, CreatedAt
```
**Cách ghi**: `IAuditLogger.LogAsync(actorId, action, entityType, entityId, data)` gọi từ Service tại điểm thực thi (đáng tin hơn EF interceptor cho hành động nghiệp vụ). Ghi **bất đồng bộ**/qua background queue để không chặn request.

**API**: `GET /api/audit-logs?action=&roomId=&actorId=&from=&to=` (chỉ admin/Owner phòng tương ứng).

**DoD**: mỗi hành động nhạy cảm tạo 1 bản ghi audit; admin truy vấn & lọc được.

## 4. Background Jobs

**Mục tiêu**: việc tốn thời gian/định kỳ **không chạy trong request**.

**Công việc**:
| Job | Loại | Tần suất |
|-----|------|----------|
| Dọn `InviteToken` hết hạn (`Deactivate`) | định kỳ | mỗi 5–15 phút |
| Dọn `Session` hết hạn / inactive | định kỳ | mỗi giờ |
| Dọn attachment "mồ côi"/cũ | định kỳ | hằng ngày |
| Gửi notification (fan-out) / email | hàng đợi | khi có sự kiện |
| Đánh chỉ mục search (nếu dùng tsvector) | hàng đợi | khi có sự kiện |

**Lựa chọn**:
- Đơn giản & in-process: `BackgroundService`/`IHostedService` + `PeriodicTimer`.
- Cần lịch + bền + dashboard: **Hangfire** (lưu PostgreSQL) hoặc **Quartz.NET**.
- Hàng đợi nhẹ in-process: `Channel<T>` (producer/consumer).

**DoD**: không có thao tác chậm/định kỳ nào nằm trong luồng HTTP; job chạy & log được; idempotent.

## 5. File Storage Abstraction

**Vấn đề**: không hard-code `wwwroot/uploads` khắp nơi → khó đổi sang cloud.

**Abstraction** (ở `ChatApp.Service`):
```csharp
public interface IFileStorage {
    Task<StoredFile> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> OpenAsync(string key, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);
    string GetPublicUrl(string key); // hoặc presigned URL
}
public record StoredFile(string Key, string Url, long Size, string ContentType);
```
**Triển khai**: `LocalFileStorage` (dev), `MinioFileStorage`, `S3FileStorage` (`AWSSDK.S3`), `AzureBlobFileStorage` (`Azure.Storage.Blobs`). Chọn impl qua config `Storage:Provider`; đăng ký DI tương ứng.

**Quy ước**: validate **loại & dung lượng** file trước khi lưu; sinh `key` không đoán được; lưu `key` (không lưu URL tuyệt đối) vào `Attachment`/avatar; với private file dùng **presigned URL** có hạn.

**DoD**: đổi provider chỉ qua config, không sửa code service; upload/avatar dùng `IFileStorage`.

## 6. Cache (Redis)

**Mục tiêu**: giảm tải DB cho dữ liệu đọc nhiều / đổi ít.

**Đối tượng & TTL gợi ý**:
| Dữ liệu | Key | TTL | Vô hiệu khi |
|---------|-----|-----|-------------|
| Quyền của user trong phòng | `perm:{userId}:{roomId}` | 5–10 phút | đổi role/permission |
| Thông tin phòng | `room:{roomId}` | 10 phút | sửa/đổi privacy/avatar |
| UserSetting | `uset:{userId}` | 30 phút | user đổi setting |
| Đếm notification chưa đọc | `notif:unread:{userId}` | ngắn / incr-decr | có notif mới / đọc |
| Presence | `presence:{userId}` | theo kết nối | connect/disconnect |

**Cách dùng**: `IDistributedCache` + `StackExchange.Redis`, bọc `ICacheService` (get-or-set). **Bắt buộc invalidation** tại điểm ghi. Redis cũng làm **backplane SignalR** khi chạy nhiều instance (`AddStackExchangeRedis`).

**DoD**: các truy vấn nóng (permission/room/setting/unread) đọc từ cache; có invalidation đúng; hoạt động khi không có Redis (fallback `MemoryCache` ở dev).

## 7. Logging (Serilog)

**Mục tiêu**: log có cấu trúc, truy vết được.
- `Serilog.AspNetCore` + sinks (Console dev; File/Seq/Elastic prod).
- Bật **request logging** + middleware gắn `TraceId`/correlation id (đưa vào response lỗi — xem [05 §1](05-Thiet-ke-API-va-Realtime.md#1-định-dạng-phản-hồi--lỗi)).
- **Cấm log** mật khẩu, hash, token, nội dung nhạy cảm. Cấu hình mức log qua `appsettings`.

**DoD**: mọi request có log structured + traceId; lỗi 500 log đầy đủ stack; không lộ secret.

## 8. Health Check

**Endpoints**:
| Path | Kiểm tra |
|------|----------|
| `GET /health` | Liveness (app sống) |
| `GET /health/db` | Kết nối PostgreSQL (`AddNpgSql`) |
| `GET /health/storage` | `IFileStorage` (và Redis nếu có) |
| `GET /health/ready` | Readiness tổng hợp (DB + storage + cache) |

Dùng `builder.Services.AddHealthChecks()....`; `app.MapHealthChecks("/health/db", new(){ Predicate = c => c.Tags.Contains("db") })`. Trả JSON trạng thái từng dependency.

**DoD**: `/health` 200 khi app sống; `/health/db` phản ánh đúng khi DB down.

## 9. Global Validation (FluentValidation)

- Validator cho mỗi Request DTO (`RegisterRequestValidator`, `SendMessageRequestValidator`...), auto-register từ assembly.
- Tích hợp pipeline để **tự động trả 400** với danh sách lỗi theo định dạng thống nhất (gộp vào ExceptionHandlingMiddleware / `ValidationException`).
- Validate **định dạng/độ dài/bắt buộc** ở biên; ràng buộc **nghiệp vụ** vẫn ở Service.

**DoD**: input sai → 400 với chi tiết field lỗi; validator có unit test cơ bản.

## 10. OpenAPI (Swagger)

- Swashbuckle (đã bật). Bổ sung: **JWT Bearer security scheme** (nút Authorize), mô tả response code, ví dụ DTO, nhóm theo tag.
- Tùy chọn: API versioning (`Asp.Versioning`) khi cần phá vỡ hợp đồng.
- Chỉ phơi Swagger ở Development (hoặc bảo vệ ở Production).

**DoD**: Swagger hiển thị Authorize JWT, gọi thử endpoint có auth được; mọi endpoint có mô tả.

---

## Phụ lục — Ánh xạ vào kiến trúc

- Các abstraction (`IFileStorage`, `ICacheService`, `IAuditLogger`, `IJwtTokenGenerator`, `IPasswordHasher`) khai báo **interface ở Core hoặc Service**, **impl ở Service/Infrastructure**, **đăng ký DI ở API** — đúng nguyên tắc Dependency Inversion ([02](02-Kien-truc-He-thong.md)).
- Middleware/cross-cutting (logging, rate limit, exception, health) cấu hình tập trung ở `Program.cs` + `Extensions/`.
- Nếu cross-cutting phình to, cân nhắc tách project **`ChatApp.Infrastructure`** (storage, cache, email, jobs) tham chiếu Core, để Service không phụ thuộc SDK hạ tầng cụ thể.