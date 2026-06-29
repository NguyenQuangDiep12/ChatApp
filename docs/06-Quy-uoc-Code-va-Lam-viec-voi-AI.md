# 06 — Quy ước code & Làm việc với AI

## 1. Quy ước đặt tên & cấu trúc

- **Namespace** theo project: `ChatApp.Core.*`, `ChatApp.Repository.*`, `ChatApp.Service.*`, `ChatApp.API.*`.
  > Dọn namespace `NLayerArchitecture.*` còn sót (xem nợ kỹ thuật 02).
- Interface tiền tố `I` (`IUserService`). Async method hậu tố `Async`.
- 1 file = 1 type công khai. Tên file = tên type.
- DTO đặt ở `ChatApp.Service/DTOs` (record, immutable). Request hậu tố `Request`, Response/đầu ra hậu tố `Dto`/`Response`.

## 2. Quy tắc theo tầng (RẤT QUAN TRỌNG)

| Quy tắc | Bắt buộc |
|---------|----------|
| API Controller chỉ gọi Service, **không** chạm DbContext/Repository | ✅ |
| Service trả **DTO**, không trả entity domain ra ngoài | ✅ |
| Thay đổi state entity chỉ qua **hành vi domain** (vd `message.Delete()`), không gán private setter từ ngoài | ✅ |
| Ghi DB luôn kết thúc bằng `IUnitOfWork.CommitAsync()` | ✅ |
| Thao tác nhiều bảng/nhiều bước → dùng transaction | ✅ |
| Read-only query dùng `AsNoTracking()` | ✅ |
| Định danh dùng `Guid` xuyên suốt (sau Sprint 0) | ✅ |
| Không hard-code secret/connection string — đọc từ config/env | ✅ |
| Không log token, mật khẩu, hash | ✅ |
| Mọi endpoint thay đổi dữ liệu phải kiểm tra quyền ở Service | ✅ |

## 3. Xử lý lỗi

- Service ném **exception nghiệp vụ** (`NotFoundException`, `ForbiddenException`, `ConflictException`, `ValidationException`...) định nghĩa ở `ChatApp.Service/Exceptions`.
- API có **ExceptionHandlingMiddleware** ánh xạ exception → mã HTTP + body lỗi thống nhất (xem [05](05-Thiet-ke-API-va-Realtime.md#1-định-dạng-phản-hồi--lỗi)).
- Không nuốt exception (no empty catch). Không trả thông tin nhạy cảm trong `detail`.

## 4. Validation

- Validate input ở biên API (FluentValidation hoặc DataAnnotations) trước khi xuống Service.
- Ràng buộc nghiệp vụ (vd "user phải là member") validate trong Service.

## 5. Git & commit

- Nhánh hiện tại: `DomainDevelopment`. Nhánh chính: `main`.
- Quy ước commit (Conventional Commits): `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`.
  - Ví dụ: `feat(service): implement MessageService.SendMessageAsync`.
- **1 task → 1 (vài) commit nhỏ, build xanh**. Không commit code không build được.
- Chỉ commit/push khi người dùng yêu cầu.

## 6. Definition of Done (DoD) chung cho mọi task

Một task được coi là **xong** khi:
1. ✅ Code build thành công (`dotnet build`).
2. ✅ Không phá vỡ hành vi/tầng (tuân thủ mục 2).
3. ✅ Có DTO ở biên (không lộ entity).
4. ✅ Đã đăng ký DI nếu thêm service/repository mới.
5. ✅ Endpoint mới xuất hiện trên Swagger và gọi thử được.
6. ✅ Có kiểm thử cho logic nghiệp vụ quan trọng (từ Sprint có test trở đi).
7. ✅ Cập nhật tài liệu nếu thay đổi hợp đồng API/model.

## 7. Lệnh hay dùng

```bash
# Build toàn solution
dotnet build ChatApp.sln

# Chạy API
dotnet run --project ChatApp.API

# Tạo migration mới
dotnet ef migrations add <Tên> --project ChatApp.Repository --startup-project ChatApp.API

# Áp migration
dotnet ef database update --project ChatApp.Repository --startup-project ChatApp.API

# Chạy test (khi đã có project test)
dotnet test
```

## 8. Làm việc với AI — quy trình đề xuất

**Bối cảnh khởi tạo phiên** (dán cho AI ở đầu):
> "Đọc `docs/02-Kien-truc-He-thong.md`, `docs/03-Mo-hinh-Du-lieu-va-ERD.md`, `docs/06-Quy-uoc-Code-va-Lam-viec-voi-AI.md`. Tuân thủ quy tắc theo tầng và Definition of Done. Ta đang làm **Sprint X**."

**Vòng lặp mỗi task**:
1. Yêu cầu AI **đọc file liên quan** trước khi sửa.
2. Giao **một task nhỏ** kèm tiêu chí chấp nhận (lấy từ checklist sprint).
3. AI implement → `dotnet build` → sửa lỗi.
4. Review: kiểm DoD (mục 6) → commit.
5. Sang task kế.

**Nguyên tắc vàng khi prompt AI**:
- Nêu rõ **file đích** và **interface/contract** phải tôn trọng.
- Yêu cầu **không tự đổi public contract** (interface ở Core) nếu không được duyệt.
- Yêu cầu **giữ phong cách rich domain** (qua hành vi, không gán private setter).
- Sau khi xong, yêu cầu AI **tự liệt kê đã thỏa DoD nào**.

## 9. Mẫu prompt tái sử dụng

**Mẫu — implement một Service**
```
Ngữ cảnh: Sprint <N>. Tuân thủ docs/06.
Nhiệm vụ: Implement <IXxxService> tại ChatApp.Service/Services/XxxService.cs.
Ràng buộc:
- Dùng các repository qua DI + IUnitOfWork.CommitAsync().
- Trả DTO (định nghĩa/đặt ở ChatApp.Service/DTOs), không trả entity.
- Kiểm tra quyền/nghiệp vụ, ném exception nghiệp vụ phù hợp.
- Đăng ký DI trong ServiceCollectionExtensions.
Hoàn tất: dotnet build xanh; liệt kê DoD đã đạt.
```

**Mẫu — thêm một Controller + endpoint**
```
Nhiệm vụ: Thêm <Xxx>Controller với các endpoint <liệt kê> theo docs/05.
Ràng buộc: controller mỏng, chỉ gọi Service; dùng DTO; có [Authorize] khi cần;
trả mã HTTP đúng (201 cho create...). Kiểm tra Swagger hiển thị.
```