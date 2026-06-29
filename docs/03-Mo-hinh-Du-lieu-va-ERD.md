# 03 — Mô hình dữ liệu & ERD

> Nguồn sự thật: `ChatApp.Core/Models/*` và `ChatApp.Repository/Configurations/*`.
> Mọi khóa chính hiện là `Guid` (xem nợ kỹ thuật về việc interface còn dùng `int`).

## 1. Sơ đồ quan hệ (ERD dạng văn bản)

```
User 1───1 UserSetting
User 1───* Session
User 1───* Notification
User 1───* MessageRead
User 1───* UserSystemRole *───1 Role        (vai trò hệ thống của user)
User 1───* InviteToken (Creator)
User 1───* Message (Sender)
User 1───* RoomMember *───1 Room            (tư cách thành viên trong phòng)
User 1───* Room (Creator)

Room 1───* RoomMember
Room 1───* Message
Room 1───* Role          (room-scoped roles)
Room 1───* InviteToken

RoomMember *───1 Role
RoomMember *───0..1 InviteToken             (gia nhập bằng link nào)

Message 1───* Attachment
Message 1───* MessageRead
Message 1───* Notification
Message 0..1───* Message (ReplyTo / Replies) (tự tham chiếu)

Role 1───* RolePermission *───1 Permission
Role 1───* UserSystemRole
Role 1───* RoomMember
```

## 2. Danh sách Enum

| Enum | Giá trị | Ý nghĩa |
|------|---------|---------|
| `UserStatus` | `UNKNOWN`, `ONLINE`, `OFFLINE` | Trạng thái hiện diện người dùng |
| `RoomType` | `DIRECT`, `GROUP`, `CHANEL` | 1-1 / nhóm / kênh broadcast (chỉ người được ủy quyền gửi) |
| `PrivacyType` | `PUBLIC`, `PRIVATE`, `PASSWORD` | Tự do vào / mời qua link / cần mật khẩu |
| `MessageType` | `Text`, `Image`, `File` | Loại nội dung tin nhắn |
| `RoleScope` | `System`, `Room` | Phạm vi vai trò |
| `NotificationType` | `Info`, `Warning`, `Error`, `Update` | Loại thông báo |

> Lưu ý: enum lưu xuống DB dạng `int` (`HasConversion<int>()`). `RoomType.CHANEL` viết thiếu chữ N — giữ nguyên để khớp code, hoặc đưa vào nợ kỹ thuật nếu muốn đổi tên.

## 3. Chi tiết Entity

### User (`users`)
| Thuộc tính | Kiểu | Ràng buộc / ghi chú |
|-----------|------|---------------------|
| Id | Guid | PK, `ValueGeneratedNever` |
| UserName | string(100) | required |
| Email | string(255) | required, **unique index** |
| PasswordHash | string(255) | required |
| AvatarUrl | string(255) | |
| UserStatus | enum int | required, default `UNKNOWN` |
| LastSeen | DateTime | cập nhật khi `SetStatus(OFFLINE)` |
| CreatedAt | DateTime | required |
| UserSettings | 1-1 | |
| Sessions, Notifications, MessageReads, UserSystemRoles, InviteTokens, Messages, RoomMembers | navigation | |

**Hành vi**: `SetUserName`, `SetAvatar`, `SetPassword`, `SetStatus` (tự set `LastSeen` khi offline).
**Ghi chú**: `User.Rooms` bị `Ignore` trong config — truy cập phòng qua `RoomMembers`.

### UserSetting (`settings`)
| Id (=UserId) Guid PK/FK 1-1 | ShowOnlineStatus | ShowLastSeen | SendReadReceipt | CreatedAt |
Mặc định tất cả cờ = `false` (`CreateDefault`). Các cờ `public set` (sửa trực tiếp).

### Session (`sessions`)
| Id Guid PK | UserId FK | Token | DeviceInfo | IsActive | ExpiresAt (mặc định +7 ngày) | CreatedAt |
**Hành vi**: `IsExpired()`, `RevokeToken()`, `RenewToken(newToken)` (gia hạn +7 ngày).

### Room (`rooms`)
| Id Guid PK | Name | Description | PrivacyType | RoomType | PasswordHash? | CreatedBy (FK User) | CreatedAt |
**Hành vi**: `Rename`, `ChangeDescription`, `ChangePrivacy` (xóa password nếu khác `PASSWORD`), `SetPassword`, `AddMember`, `RemoveMember`.

### RoomMember (`room_members`)
| Id Guid PK | RoomId FK | UserId FK | RoleId FK | InviteTokenId? FK | JoinedAt | LastReadAt |
**Ràng buộc**: unique `(RoomId, UserId)` — 1 user 1 bản ghi/phòng. Cascade từ Room & User.
**Hành vi**: `UpdateLastReadAt()`, `ChangeRole(newRoleId)`.

### Message (`messages`)
| Id Guid PK | SenderId FK (Restrict) | RoomId FK (Cascade) | ReplyToId? FK self (Restrict) | Content(4000) | Type enum | IsEdited | IsDeleted | CreatedAt | UpdatedAt |
**Index**: `RoomId`; `(RoomId, CreatedAt)`; `SenderId`; `ReplyToId`.
**Hành vi**: `Edit(newContent)` (set `IsEdited`, `UpdatedAt`), `Delete()` (soft delete: `IsDeleted=true`, xóa nội dung).

### Attachment (`attachments`)
| Id Guid PK | MessageId FK | FileUrl | FileName | FileType | FileSize (long) | CreatedAt |

### MessageRead (`message_reads`)
| Id Guid PK | UserId FK | MessageId FK | ReadAt | — đánh dấu user đã đọc message. |

### Notification (`notifications`)
| Id Guid PK | UserId FK | MessageId FK | Type enum | IsRead | CreatedAt |
**Hành vi**: `MarkAsRead()`.

### InviteToken (`invite_tokens`)
| Id Guid PK | RoomId FK | CreatedBy FK User | Token | Note | IsActive | MaxUsage (byte, mặc định 10) | UseCount (byte) | ExpireAt (mặc định +45 phút) | CreatedAt |
**Hành vi**: `IsExpired()`, `CanBeUsed()` (active && chưa hết hạn && còn lượt), `IncreaseUsage()` (tự tắt khi hết lượt), `Deactivate()`.

### Role (`roles`)
| Id Guid PK | Name(100) | Description(500) | Scope enum | RoomId? FK | IsCustom | IsSystemDefault | Priority (byte) | Color(20) | CreatedAt |
**Constructor**: 1 cho System role (`RoomId=null`), 1 cho Room role (`RoomId` bắt buộc).
**Ràng buộc**:
- Index `RoomId`; unique `(Name, RoomId, Scope)`.
- **Check constraint** `CK_Role_Scope_Room`: `(Scope=0 AND RoomId IS NULL) OR (Scope=1 AND RoomId IS NOT NULL)`.
**Hành vi**: `Rename`, `SetDescription`, `SetColor`, `SetPriority`.

### Permission (`permissions`)
| Id Guid PK | PermissionCode | Description | Scope enum |
Quyền được định danh bằng `PermissionCode` (ví dụ: `room.message.delete`).

### RolePermission (`role_permissions`)
| Id Guid PK | RoleId FK | PermissionId FK | — bảng nối Role ⇄ Permission. |

### UserSystemRole (`system_roles`)
| Id Guid PK | UserId FK | RoleId FK | AssignedBy (Guid) | CreatedAt | — gán vai trò **hệ thống** cho user. |

## 4. Gợi ý dữ liệu hạt giống (seed) — dùng từ Sprint 5

**System roles**: `SuperAdmin`, `Moderator`, `User` (mặc định cho mọi tài khoản mới).
**Room roles** (tạo theo từng phòng): `Owner`, `Admin`, `Member`.
**Permission codes (gợi ý)**:
```
# System scope
system.user.ban, system.user.manageRoles, system.room.manage
# Room scope
room.update, room.delete,
room.member.invite, room.member.kick, room.member.changeRole,
room.message.send, room.message.delete.any, room.message.pin,
room.role.manage, room.invite.manage
```

## 5. Quy ước thiết kế dữ liệu
- Khóa chính `Guid`, sinh trong constructor domain (`ValueGeneratedNever`) — **không** để DB tự sinh.
- Mốc thời gian lưu **UTC** (`DateTime.UtcNow`).
- Xóa tin nhắn = **xóa mềm** (`IsDeleted`), không xóa cứng.
- Enum lưu dạng `int`.
- Mật khẩu (user & room) chỉ lưu **hash**, không bao giờ lưu plaintext.

## 7. Đề xuất mở rộng mô hình dữ liệu (Telegram-like)

> Các thay đổi này phục vụ tính năng ở [05 Phần II](05-Thiet-ke-API-va-Realtime.md#phần-ii--tính-năng-mở-rộng-kiểu-telegram-đề-xuất). Triển khai dần theo [Sprint 9](07-Lo-trinh-Sprint.md#sprint-9--tìm-kiếm--tính-năng-nâng-cao-kiểu-telegram). Mỗi nhóm = 1 migration riêng.

### 7.1. Cột thêm vào entity hiện có
| Entity | Cột thêm | Mục đích |
|--------|----------|----------|
| `Message` | `ForwardFromMessageId? (Guid)`, `ForwardFromRoomId? (Guid)`, `ForwardFromUserId? (Guid)` | Forward (khác Reply) |
| `Message` | `DeletedForEveryone (bool)`, `DeletedBy? (Guid)` | Phân biệt xóa-2-chế-độ |
| `Message` | *(tùy chọn)* `IsPinned`, `PinnedBy?`, `PinnedAt?` | Pin đơn giản (nếu không dùng bảng `pinned_messages`) |
| `Room` | `AvatarUrl (string)` | Avatar nhóm |
| `Session` | `IpAddress (string)`, `LastActiveAt (DateTime)` | Active sessions |
| `User` | *(tùy chọn)* `IsDeleted (bool)`, `DeletedAt? (DateTime)` | Delete account |
| `Notification` | `MessageId` → **nullable**; thêm `RoomId? (Guid)`, `ActorId? (Guid)`, `Data (string/jsonb)` | Đa loại notification |

### 7.2. Bảng (entity) mới
| Bảng | Cột chính | Ràng buộc |
|------|-----------|-----------|
| `pinned_messages` | `Id, RoomId, MessageId, PinnedBy, PinnedAt` | unique `(RoomId, MessageId)` |
| `message_reactions` | `Id, MessageId, UserId, Emoji, CreatedAt` | unique `(MessageId, UserId, Emoji)` |
| `message_mentions` | `Id, MessageId, UserId` | unique `(MessageId, UserId)`; index `UserId` |
| `message_histories` | `Id, MessageId, OldContent, EditedAt` | index `MessageId` |
| `message_hidden` | `Id, UserId, MessageId` | unique `(UserId, MessageId)` — "delete for me" |
| `user_room_states` | `Id, UserId, RoomId, IsArchived, IsMuted, MutedUntil?, UpdatedAt` | unique `(UserId, RoomId)` |
| `blocked_users` | `Id, UserId, BlockedUserId, CreatedAt` | unique `(UserId, BlockedUserId)` |
| `reports` | `Id, ReporterId, TargetUserId?, TargetMessageId?, Reason, Status, CreatedAt, HandledBy?, HandledAt?` | index `Status` |
| `audit_logs` | `Id, ActorId, Action, EntityType, EntityId, RoomId?, Data (jsonb), IpAddress?, CreatedAt` | index `(EntityType, EntityId)`, `RoomId`, `CreatedAt` — xem [08 §3](08-Cross-cutting-va-Phi-chuc-nang.md#3-audit-log) |

### 7.3. Enum cập nhật
- **`NotificationType`** (đề xuất thay/bổ sung): `NEW_MESSAGE`, `MENTION`, `REACTION`, `ROOM_INVITE`, `ROLE_CHANGED`, `ROOM_UPDATED` (giữ `Info/Warning/Error/Update` cho thông báo hệ thống nếu cần). Xem bảng ngữ cảnh ở [05 §21](05-Thiet-ke-API-va-Realtime.md#21-mở-rộng-loại-notification).
- *(tùy chọn)* `ReportStatus`: `Pending`, `Reviewing`, `Resolved`, `Rejected`.

### 7.4. Lưu ý khi mở rộng
- Giữ **rich domain**: thêm hành vi (vd `Message.Pin(by)`, `Message.Forward(...)`, `Message.DeleteForEveryone(by)`) thay vì set thuộc tính từ ngoài.
- Truy vấn timeline phải lọc đồng thời: `IsDeleted`/`DeletedForEveryone` **và** `message_hidden` của user hiện tại.
- Reaction/mention/pin nên có index theo cột tra cứu nóng (`MessageId`, `UserId`).
- Mọi bảng mới đăng ký `IEntityTypeConfiguration<T>` + `DbSet` + migration riêng (xem quy ước [06](06-Quy-uoc-Code-va-Lam-viec-voi-AI.md)).