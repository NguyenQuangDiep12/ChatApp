# Tài liệu thiết kế phần mềm — ChatApp (Hệ thống Chat đa nền tảng)

> Bộ tài liệu này mô tả thiết kế phần mềm của dự án **ChatApp** và chia công việc thành các **Sprint** để dễ phối hợp triển khai cùng AI (Claude Code / trợ lý lập trình).

## Mục lục

| # | Tài liệu | Nội dung |
|---|----------|----------|
| 01 | [Tổng quan & Phạm vi](01-Tong-quan-va-Pham-vi.md) | Tầm nhìn, mục tiêu, đối tượng, phạm vi MVP, công nghệ |
| 02 | [Kiến trúc hệ thống](02-Kien-truc-He-thong.md) | N-Layer, luồng phụ thuộc, trạng thái hiện tại, **nợ kỹ thuật** |
| 03 | [Mô hình dữ liệu & ERD](03-Mo-hinh-Du-lieu-va-ERD.md) | Toàn bộ entity, quan hệ, enum, ràng buộc, index |
| 04 | [Yêu cầu chức năng](04-Yeu-cau-Chuc-nang.md) | Use case, yêu cầu chức năng & phi chức năng |
| 05 | [Thiết kế API & Realtime](05-Thiet-ke-API-va-Realtime.md) | REST endpoints, DTO, SignalR Hub |
| 06 | [Quy ước code & Làm việc với AI](06-Quy-uoc-Code-va-Lam-viec-voi-AI.md) | Coding convention, quy trình, mẫu prompt cho AI |
| 07 | [Lộ trình Sprint](07-Lo-trinh-Sprint.md) | Sprint 0 → 10: mục tiêu, checklist, Definition of Done, prompt mẫu |
| 08 | [Cross-cutting & Phi chức năng](08-Cross-cutting-va-Phi-chuc-nang.md) | Cursor pagination, rate limit, audit, jobs, storage, cache, logging, health, validation, OpenAPI |

## Cách dùng tài liệu này với AI

1. **Trước mỗi phiên làm việc**: cung cấp cho AI các file nền: `02-Kien-truc`, `03-Mo-hinh-Du-lieu`, `06-Quy-uoc-Code` (+ `08-Cross-cutting` khi đụng tới phân trang/upload/cache/log...).
2. **Khi bắt đầu một Sprint**: mở [07-Lo-trinh-Sprint](07-Lo-trinh-Sprint.md), copy phần Sprint tương ứng (mục tiêu + checklist + DoD + prompt mẫu) vào prompt.
3. **Mỗi task = một đơn vị công việc nhỏ, kiểm chứng được**: yêu cầu AI làm 1 task → build → test → commit, rồi mới sang task kế.
4. **Luôn yêu cầu AI tôn trọng `Definition of Done`** ở mỗi sprint và quy ước ở file 06.

## Trạng thái dự án (tóm tắt nhanh)

- ✅ **Core**: domain models, enums, interface Repository/Service/UoW — đã định nghĩa.
- 🟡 **Repository**: DbContext, EF Configurations, migration `InitialCreate`, đa số repository — đã có nhưng còn lỗi build & lệch kiểu `int`/`Guid`.
- ❌ **Service**: chưa có implementation (project rỗng).
- ❌ **API**: chưa có Controller / DI / Auth / SignalR.
- ❌ **Frontend (React)**: chưa khởi tạo.

> Chi tiết & danh sách nợ kỹ thuật: xem [02-Kien-truc-He-thong.md](02-Kien-truc-He-thong.md#5-nợ-kỹ-thuật-cần-xử-lý-trước).