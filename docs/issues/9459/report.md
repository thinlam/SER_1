# UC22 - Implementation Report: Quản lý phê duyệt nội dung trình duyệt

## Issue #9459 | Branch: `feature/9459-quan-ly-phe-duyet-noi-dung-trinh-duyet`

## Summary

Triển khai module PheDuyetNoiDung - màn hình tổng hợp quản lý phê duyệt, phản hồi và ban hành nội dung trình duyệt. BGĐ duyệt/từ chối/ký số/chuyển QLVB, P.HC-TH phát hành, CB/LĐ gửi lại sau khi bị trả lại.

## Architecture

Follow PheDuyetDuToan pattern nhưng thiết kế mới: **unified approval overlay** trên VanBanQuyetDinh. Không duplicate VBQD data - reference qua FK, thêm approval-specific fields.

```
VanBanQuyetDinh (existing) ← PheDuyetNoiDung (new, approval tracking)
                                ├── TrangThaiId (int FK → DanhMucTrangThaiPheDuyet)
                                ├── PheDuyetNoiDungHistory (audit trail, TrangThaiId FK)
                                └── DanhMucTrangThaiPheDuyet (shared DanhMuc, discriminated by Loai)
```

### DanhMucTrangThaiPheDuyet (Shared Status Catalog)

Merged `DanhMucTrangThaiPheDuyetDuToan` + `DanhMucTrangThaiPheDuyetNoiDung` thành single entity `DanhMucTrangThaiPheDuyet` với `Loai` discriminator:

| Loai | Id | Ma | Ten |
|------|----|----|-----|
| DuToan | 1 | DT | Dự thảo |
| DuToan | 2 | ĐTr | Đã trình |
| DuToan | 3 | ĐD | Đã duyệt |
| DuToan | 4 | TL | Trả lại |
| DuToan | 5 | LEG | Migrated |
| NoiDung | 6 | CXL | Chờ xử lý |
| NoiDung | 7 | TC | Từ chối |
| NoiDung | 8 | DKS | Đã ký số |
| NoiDung | 9 | DQLVB | Đã chuyển QLVB |
| NoiDung | 10 | DPH | Đã phát hành |
| NoiDung | 11 | DD | Đã duyệt |
| NoiDung | 12 | TL | Trả lại |

### Constants

`TrangThaiPheDuyetCodes` - merged status codes with nested classes:
- `TrangThaiPheDuyetCodes.Loai.DuToan` / `.NoiDung` - Loai discriminator
- `TrangThaiPheDuyetCodes.DuToan.*` - DuToan status codes (DT, ĐTr, ĐD, TL, LEG)
- `TrangThaiPheDuyetCodes.NoiDung.*` - NoiDung status codes (CXL, DD, TC, TL, DKS, DQLVB, DPH)

### Status Tracking (FK-based)

PheDuyetNoiDung & PheDuyetNoiDungHistory use `int? TrangThaiId` FK → `DanhMucTrangThaiPheDuyet` (not string codes). Commands lookup status via `_statusRepository` by `Ma + Loai`, then set `TrangThaiId`. DTOs expose `TrangThaiId`, `MaTrangThai`, `TenTrangThai`.

Composite unique index on `(Ma, Loai)` allows same Ma across different Loai (e.g. "TL" for both DuToan and NoiDung).

## Files Created

### Domain Layer (3 files)
- `QLDA.Domain/Constants/TrangThaiPheDuyetCodes.cs` - Merged status codes + Loai constants
- `QLDA.Domain/Entities/PheDuyetNoiDung.cs` - Main entity (TrangThaiId FK)
- `QLDA.Domain/Entities/PheDuyetNoiDungHistory.cs` - History entity (TrangThaiId FK)

### Persistence Layer (2 files)
- `QLDA.Persistence/Configurations/PheDuyetNoiDungConfiguration.cs`
- `QLDA.Persistence/Configurations/PheDuyetNoiDungHistoryConfiguration.cs`

### Application Layer (11 files)
- `QLDA.Application/PheDuyetNoiDungs/Commands/` - 8 command handlers (all use `_statusRepository` for FK lookup)
  - Trinh, Duyet, TuChoi, TraLai, KySo, ChuyenQLVB, PhatHanh, GuiLai
- `QLDA.Application/PheDuyetNoiDungs/Queries/` - 3 query handlers (`.Include(e => e.TrangThai)`)
  - GetDanhSach (paginated + visibility filter), GetChiTiet, GetLichSu
- `QLDA.Application/PheDuyetNoiDungs/DTOs/` - 3 DTOs (TrangThaiId, MaTrangThai, TenTrangThai)
  - PheDuyetNoiDungDto, PheDuyetNoiDungChiTietDto, PheDuyetNoiDungLichSuDto

### WebApi Layer (10 files)
- `QLDA.WebApi/Controllers/PheDuyetNoiDungController.cs` - 11 endpoints
- `QLDA.WebApi/Controllers/DanhMucTrangThaiPheDuyetController.cs` - 6 CRUD endpoints
- `QLDA.WebApi/Models/PheDuyetNoiDungs/` - 6 request models
- `QLDA.WebApi/Models/DmTrangThaiPheDuyet/` - Model + Mapping (with Loai)

### Tests (1 file)
- `QLDA.Tests/Integration/PheDuyetNoiDungControllerTests.cs` - 20 tests

## Files Modified

| File | Change |
|------|--------|
| `QLDA.Domain/Constants/RoleConstants.cs` | Added `QLDA_HC_TH` |
| `QLDA.Domain/Constants/PermissionConstants.cs` | Added 7 PheDuyet permissions + ByNhom + RolePermissions |
| `QLDA.Domain/Entities/PheDuyetDuToan.cs` | TrangThai navigation → DanhMucTrangThaiPheDuyet (was DanhMucTrangThaiPheDuyetDuToan) |
| `QLDA.Domain/Entities/PheDuyetDuToanHistory.cs` | TrangThai navigation → DanhMucTrangThaiPheDuyet |
| `QLDA.Domain/Entities/HoSoDeXuatCapDoCntt.cs` | TrangThai navigation → DanhMucTrangThaiPheDuyet |
| `QLDA.Domain/Entities/DanhMuc/DanhMucTrangThaiPheDuyet.cs` | Removed PheDuyetDuToans collection nav |
| `QLDA.Application/Common/Enums/EDanhMuc.cs` | Added `DanhMucTrangThaiPheDuyet` |
| `QLDA.Application/Providers/IAppSettingsProvider.cs` | Added `PhongHCTHID` |
| `QLDA.Application/Common/Queries/DanhMucGetQuery.cs` | Added DanhMucTrangThaiPheDuyet handler |
| `QLDA.Application/Common/Queries/DanhMucGetDanhSachQuery.cs` | Added DanhMucTrangThaiPheDuyet handler |
| `QLDA.Application/Common/Commands/DanhMucInsertOrUpdateCommand.cs` | Added DanhMucTrangThaiPheDuyet handler |
| `QLDA.Application/PheDuyetDuToans/Commands/*` | Updated status repo + codes references |
| `QLDA.Application/PheDuyetDuToans/DTOs/PheDuyetDuToanDto.cs` | Updated codes reference |
| `QLDA.Persistence/Configurations/DanhMuc/DanhMucTrangThaiPheDuyetConfiguration.cs` | Composite unique (Ma+Loai), seed 12 entries, removed old nav |
| `QLDA.Persistence/Configurations/DanhMuc/DanhMucQuyenConfiguration.cs` | Formatting, Ma max length |
| `QLDA.Persistence/Configurations/PheDuyetDuToanConfiguration.cs` | TrangThai nav → WithMany() |
| `QLDA.Persistence/Configurations/PheDuyetNoiDungConfiguration.cs` | Added TrangThai FK |
| `QLDA.Persistence/Configurations/PheDuyetNoiDungHistoryConfiguration.cs` | Added TrangThai FK |
| `QLDA.WebApi/ConfigurationOptions/AppSettings.cs` | Added `PhongHCTHID` |
| `QLDA.WebApi/ConfigurationOptions/AppSettingsProvider.cs` | Added `PhongHCTHID` |
| `QLDA.WebApi/appsettings.json` | Added `PhongHCTHID: 0` |
| `QLDA.Tests/Fixtures/WebApiFixture.cs` | CreatePheDuyetNoiDungAsync takes int? TrangThaiId |
| `QLDA.Tests/Integration/PheDuyetNoiDungControllerTests.cs` | Uses int TrangThaiId constants |

## Files Deleted (refactored away)

| File | Reason |
|------|--------|
| `DanhMucTrangThaiPheDuyetDuToan.cs` | Merged into shared `DanhMucTrangThaiPheDuyet` |
| `DanhMucTrangThaiPheDuyetDuToanConfiguration.cs` | Replaced by shared `DanhMucTrangThaiPheDuyetConfiguration` |
| `TrangThaiPheDuyetDuToanCodes.cs` | Merged into `TrangThaiPheDuyetCodes.cs` |
| `TrangThaiPheDuyetNoiDungCodes.cs` | Merged into `TrangThaiPheDuyetCodes.cs` |

## Build & Test Results

- **Build**: 0 errors, 0 warnings
- **Tests**: 20/20 PheDuyetNoiDung passed, 29/30 total (1 pre-existing PheDuyetDuToan failure)
- **Existing tests**: Not affected (fixture change backward-compatible)

## Design Decisions

1. **FK-based status** (`int? TrangThaiId` → `DanhMucTrangThaiPheDuyet`) - consistent với PheDuyetDuToan, DB-enforced referential integrity
2. **Separate entity** không inherit VanBanQuyetDinh - approval layer trên VBQD có sẵn
3. **History tracks TrangThaiId FK** - join DanhMuc để display Ma/Ten trong DTOs
4. **Visibility filter** reuse `ApplyDuAnChildVisibility` - consistent với existing modules
5. **Role checks active** (không comment out như PheDuyetDuToan) - enforce security
6. **Shared DanhMucTrangThaiPheDuyet** with `Loai` discriminator - DRY, single CRUD
7. **Merged constants** `TrangThaiPheDuyetCodes` with nested DuToan/NoiDung/Loai classes
8. **Composite unique index** on (Ma, Loai) - allows same Ma across different Loai types

## DanhMucTrangThaiPheDuyet CRUD Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `api/danh-muc-trang-thai-phe-duyet/{id}` | GET | Get by ID |
| `api/danh-muc-trang-thai-phe-duyet/danh-sach` | GET | List (active only) |
| `api/danh-muc-trang-thai-phe-duyet/danh-sach-day-du` | GET | Full paginated list |
| `api/danh-muc-trang-thai-phe-duyet/them-moi` | POST | Create |
| `api/danh-muc-trang-thai-phe-duyet/cap-nhat` | PUT | Update |
| `api/danh-muc-trang-thai-phe-duyet/xoa-tam` | DELETE | Soft delete |

## Unresolved Questions

1. `PhongHCTHID` trong `appsettings.json` đang = 0, cần cấu hình ID phòng HC-TH thực tế
2. Role `QLDA_HC_TH` cần được thêm vào hệ thống authentication/authorization
3. Chưa có notification/gửi thông báo kết quả xử lý đến đơn vị trình duyệt (UC22 bước 7)
