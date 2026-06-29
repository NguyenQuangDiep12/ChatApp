# 02 — Kiến trúc hệ thống

## 1. Tổng quan kiến trúc N-Layer

Dự án theo **N-Layer / Clean Architecture** (cảm hứng từ [NLayerArchitecture](https://github.com/BerkayKulak/NLayerArchitecture)), gồm 4 project:

```
┌──────────────────────────────────────────────────────────┐
│  ChatApp.API            (Presentation)                     │
│  Controllers, SignalR Hubs, DI, Middleware, Swagger, Auth  │
└───────────────┬──────────────────────────────────────────┘
                │ depends on
┌───────────────▼──────────────────────────────────────────┐
│  ChatApp.Service        (Business Logic)                   │
│  Service implementations, DTO, Mapping, Validation         │
└───────────────┬──────────────────────────────────────────┘
                │ depends on
┌───────────────▼──────────────────────────────────────────┐
│  ChatApp.Repository     (Data Access)                      │
│  ApplicationDbContext, EF Configurations, Repositories,    │
│  UnitOfWork, Migrations                                     │
└───────────────┬──────────────────────────────────────────┘
                │ depends on
┌───────────────▼──────────────────────────────────────────┐
│  ChatApp.Core           (Domain — trung tâm)               │
│  Models (entities), Enums, Interfaces (IRepository,        │
│  IService, IUnitOfWork)                                     │
└──────────────────────────────────────────────────────────┘
```

### Nguyên tắc phụ thuộc
- **Core không phụ thuộc bất kỳ tầng nào** — là trung tâm. Mọi tầng khác hướng về Core (Dependency Inversion).
- Mọi tầng giao tiếp qua **interface** định nghĩa ở Core.
- API **không** truy cập trực tiếp Repository/DbContext — phải qua Service.
- Service **không** trả entity thô ra ngoài — trả về **DTO**.

## 2. Trách nhiệm từng tầng

### Core (`ChatApp.Core`)
- `Models/` — entity domain (rich domain: private setter, hàm hành vi như `Edit()`, `Delete()`, `SetStatus()`...).
- `Models/Enums/` — `MessageType`, `RoomType`, `PrivacyType`, `RoleScope`, `UserStatus`, `NotificationType`.
- `Repositories/` — interface repository (`IGenericRepository<T>` + interface theo entity).
- `Services/` — interface service (`IService<T>` + interface theo entity).
- `UnitOfWorks/` — `IUnitOfWork`.

### Repository (`ChatApp.Repository`)
- `ApplicationDbContext` — khai báo `DbSet`, nạp cấu hình từ assembly.
- `Configurations/` — `IEntityTypeConfiguration<T>` cho từng entity (mapping bảng, FK, index, constraint).
- `Repositories/` — implement repository (kế thừa `GenericRepository<T>`).
- `UnitOfWorks/UnitOfWork` — bọc `SaveChanges`.
- `Migrations/` — migration EF Core.

### Service (`ChatApp.Service`) — *chưa triển khai*
- Implement các interface `IXxxService`.
- Chứa **logic nghiệp vụ**: kiểm tra quyền, ràng buộc, điều phối nhiều repository, gọi `UnitOfWork.CommitAsync()`.
- Mapping Entity ⇄ DTO.
- Ném exception nghiệp vụ (NotFound, Forbidden, Conflict...).

### API (`ChatApp.API`)
- `Controllers/` — REST endpoints (mỏng, chỉ điều phối → Service).
- `Hubs/` — SignalR hubs (realtime).
- `Program.cs` — cấu hình DI, DbContext, Auth, CORS, Swagger, middleware.

## 3. Luồng xử lý điển hình (ví dụ: gửi tin nhắn)

```
Client → POST /api/rooms/{roomId}/messages
  → MessagesController.Send(dto)
    → IMessageService.SendMessageAsync(...)
        - kiểm tra user là thành viên phòng (IRoomMemberService/repo)
        - tạo entity Message (new Message(...))
        - _messageRepository.AddAsync(message)
        - _unitOfWork.CommitAsync()
        - map → MessageDto
    ← MessageDto
  → IChatHub.Clients.Group(roomId).SendAsync("ReceiveMessage", dto)  // realtime
← 201 Created + MessageDto
```

## 4. Trạng thái hiện tại (tính đến tài liệu này)

| Thành phần | Trạng thái | Ghi chú |
|------------|-----------|---------|
| Core — Models | ✅ Hoàn chỉnh | Rich domain, 14 entity + 6 enum |
| Core — Interfaces Repository | ✅ | `IGenericRepository<T>` + 14 interface |
| Core — Interfaces Service | ✅ | `IService<T>` + 14 interface |
| Core — IUnitOfWork | 🟡 | Chỉ có `Commit`/`CommitAsync`, **chưa expose repository** |
| Repository — DbContext | ✅ | 14 `DbSet`, nạp config tự động |
| Repository — Configurations | ✅ | Đủ 14 config (table, FK, index, check constraint) |
| Repository — Migrations | ✅ | `InitialCreate` |
| Repository — Repositories | 🟡 | Có implement nhưng **lệch kiểu `int`/`Guid`** + lỗi build |
| Repository — UnitOfWork | 🟡 | Chỉ `SaveChanges` |
| Service | ❌ | Project rỗng (chỉ `.csproj`) |
| API — Controllers | ❌ | Chưa có |
| API — DI registration | ❌ | Chưa đăng ký Repository/Service/UoW |
| API — Auth / JWT | ❌ | Chưa có |
| API — SignalR | ❌ | Chưa có |
| Frontend (React) | ❌ | Chưa khởi tạo |

## 5. Nợ kỹ thuật cần xử lý trước

> Đây là danh sách **bắt buộc** giải quyết ở **Sprint 0** trước khi xây tính năng mới.

1. **Lệch kiểu khóa `int` vs `Guid`** (nghiêm trọng, ưu tiên cao nhất)
   - Entity dùng `Guid Id`, nhưng `IGenericRepository<T>.GetByIdAsync(int id)`, `IService<T>` và toàn bộ interface service (`GetMessagesByRoomIdAsync(int roomId,...)`, v.v.) dùng `int`.
   - `GenericRepository.GetByIdAsync(int)` gọi `FindAsync(id)` → sai kiểu khóa khi chạy.
   - `UserRepository` có cả `GetByIdWithSettingsAsync(Guid)` lẫn `GetByIdWithSettingsAsync(int)` (bản `int` `throw NotImplementedException`).
   - **Hành động**: chuẩn hóa toàn bộ định danh sang `Guid` (đề xuất), cập nhật mọi interface Repository + Service.

2. **`GenericRepository.RemoveRange` sai** — gọi `_dbSet.RemoveRange()` (không truyền `entities`). Sửa thành `_dbSet.RemoveRange(entities)`.

3. **`MessageRepository.SoftDeleteAsync` không build được** — gán trực tiếp `message.IsDeleted = true; message.UpdatedAt = ...` trong khi setter là `private`. Phải gọi hành vi domain `message.Delete()`.

4. **`IUnitOfWork` chưa expose repository** — pattern Unit of Work nên cung cấp truy cập tới các repository (hoặc dùng DI repository riêng + 1 `IUnitOfWork.CommitAsync()`). Cần chốt cách dùng và áp dụng nhất quán ở Service.

5. **Namespace lẫn lộn template** — `IService<T>`, `GenericRepository<T>` còn nằm trong `NLayerArchitecture.*`. Đổi về `ChatApp.*` cho nhất quán.

6. **DI chưa đăng ký** — `Program.cs` mới chỉ có `DbContext`. Cần đăng ký toàn bộ Repository, Service, UnitOfWork (scoped).

7. **`UserConfiguration` Ignore quan hệ `Rooms`** — `builder.Ignore(u => u.Rooms)`. Cần xác nhận chủ đích (quan hệ user↔room đi qua `RoomMembers`, nên `User.Rooms` là điều hướng phụ — chấp nhận được, nhưng ghi chú rõ).

8. **Rủi ro multiple cascade path (PostgreSQL/SQL)** — `RoomMember` cascade từ cả `Room` và `User`; `Role` cascade từ `Room` còn `RoomMember→Role` restrict. Cần kiểm tra migration áp được, tránh vòng cascade.

## 6. Cấu trúc thư mục đề xuất sau khi hoàn thiện

```
ChatApp.API/
  Controllers/        # AuthController, UsersController, RoomsController, MessagesController, ...
  Hubs/               # ChatHub, PresenceHub
  Extensions/         # ServiceCollectionExtensions (DI), AuthExtensions
  Middlewares/        # ExceptionHandlingMiddleware
  DTOs/  (hoặc ở Service)
  Program.cs
ChatApp.Service/
  Services/           # UserService, MessageService, RoomService, ...
  DTOs/               # Request/Response DTO
  Mappings/           # AutoMapper profiles (hoặc mapping thủ công)
  Exceptions/         # AppException, NotFoundException, ForbiddenException, ...
ChatApp.Repository/   # (đã có)
ChatApp.Core/         # (đã có)
```