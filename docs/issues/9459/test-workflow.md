# UC22 - Test Workflow: Quản lý phê duyệt nội dung trình duyệt

## Thông tin chung

- **Issue**: #9459
- **Branch**: `feature/9459-quan-ly-phe-duyet-noi-dung-trinh-duyet`
- **Test file**: `QLDA.Tests/Integration/PheDuyetNoiDungControllerTests.cs`
- **Total tests**: 20 (all passing)
- **Status tracking**: `int? TrangThaiId` FK → `DanhMucTrangThaiPheDuyet` (FK-based, not string codes)

## Chạy test

```bash
# Toàn bộ tests PheDuyetNoiDung
dotnet test QLDA.Tests/QLDA.Tests.csproj --filter "FullyQualifiedName~PheDuyetNoiDung"

# Test cụ thể
dotnet test QLDA.Tests/QLDA.Tests.csproj --filter "FullyQualifiedName~PheDuyetNoiDungControllerTests.FullWorkflow_TrinhToPhatHanh"

# Toàn bộ tests PheDuyet (cả DuToan + NoiDung)
dotnet test QLDA.Tests/QLDA.Tests.csproj --filter "FullyQualifiedName~PheDuyet"
```

## Flow trạng thái (NoiDung)

```
CXL (id:6) ──Duyet──→ DD (id:11) ──KySo──→ DKS (id:8) ──ChuyenQLVB──→ DQLVB (id:9) ──PhatHanh──→ DPH (id:10)
 │                       │
 ├──TuChoi──→ TC (id:7)   └──ChuyenQLVB──→ DQLVB (id:9)
 │
 └──TraLai──→ TL (id:12) ──GuiLai──→ CXL (id:6) (loop)
```

## Constants Reference

```csharp
// Loai discriminator
TrangThaiPheDuyetCodes.Loai.DuToan  // "DuToan"
TrangThaiPheDuyetCodes.Loai.NoiDung // "NoiDung"

// NoiDung status codes
TrangThaiPheDuyetCodes.NoiDung.ChoXuLy      // "CXL"
TrangThaiPheDuyetCodes.NoiDung.DaDuyet       // "DD"
TrangThaiPheDuyetCodes.NoiDung.TuChoi        // "TC"
TrangThaiPheDuyetCodes.NoiDung.TraLai        // "TL"
TrangThaiPheDuyetCodes.NoiDung.DaKySo        // "DKS"
TrangThaiPheDuyetCodes.NoiDung.DaChuyenQLVB  // "DQLVB"
TrangThaiPheDuyetCodes.NoiDung.DaPhatHanh    // "DPH"

// DuToan status codes
TrangThaiPheDuyetCodes.DuToan.DuThao   // "DT"
TrangThaiPheDuyetCodes.DuToan.DaTrinh  // "ĐTr"
TrangThaiPheDuyetCodes.DuToan.DaDuyet  // "ĐD"
TrangThaiPheDuyetCodes.DuToan.TraLai   // "TL"
TrangThaiPheDuyetCodes.DuToan.Legacy   // "LEG"
```

## Seeded DanhMucTrangThaiPheDuyet IDs

| Id | Ma | Ten | Loai | Used in tests |
|----|----|-----|------|---------------|
| 1 | DT | Dự thảo | DuToan | - |
| 2 | ĐTr | Đã trình | DuToan | - |
| 3 | ĐD | Đã duyệt | DuToan | - |
| 4 | TL | Trả lại | DuToan | - |
| 5 | LEG | Migrated | DuToan | - |
| 6 | CXL | Chờ xử lý | NoiDung | `TrangThaiCXL` |
| 7 | TC | Từ chối | NoiDung | - |
| 8 | DKS | Đã ký số | NoiDung | `TrangThaiDKS` |
| 9 | DQLVB | Đã chuyển QLVB | NoiDung | `TrangThaiDQLVB` |
| 10 | DPH | Đã phát hành | NoiDung | - |
| 11 | DD | Đã duyệt | NoiDung | `TrangThaiDD` |
| 12 | TL | Trả lại | NoiDung | `TrangThaiTL` |

## DanhMucTrangThaiPheDuyet CRUD

| Endpoint | Method | Role | Description |
|----------|--------|------|-------------|
| `api/danh-muc-trang-thai-phe-duyet/{id}` | GET | Admin | Get by ID (includes Loai) |
| `api/danh-muc-trang-thai-phe-duyet/danh-sach` | GET | Admin | List active |
| `api/danh-muc-trang-thai-phe-duyet/danh-sach-day-du` | GET | Admin | Full paginated list |
| `api/danh-muc-trang-thai-phe-duyet/them-moi` | POST | Admin | Create |
| `api/danh-muc-trang-thai-phe-duyet/cap-nhat` | PUT | Admin | Update |
| `api/danh-muc-trang-thai-phe-duyet/xoa-tam` | DELETE | Admin | Soft delete |

## Ma trận test case (PheDuyetNoiDung)

| # | Endpoint | Method | Role | Test case | Seed Status |
|---|----------|--------|------|-----------|-------------|
| 1 | `/api/phe-duyet-noi-dung/danh-sach` | GET | Any | GetDanhSach_ReturnsOk | - |
| 2 | `/{id}/chi-tiet` | GET | Any | GetChiTiet_ExistingId_ReturnsOk | - |
| 3 | `/{id}/chi-tiet` | GET | Any | GetChiTiet_NonExistentId_ReturnsFailure | - |
| 4 | `/{vbqdId}/trinh` | POST | Any | Trinh_CreatesPheDuyetNoiDung | - |
| 5 | `/{vbqdId}/trinh` | POST | Any | Trinh_DuplicateVBQD_ReturnsFailure | - |
| 6 | `/{id}/duyet` | POST | BGĐ (LDDV) | Duyet_AsBgdUser_ReturnsOk | CXL (id:6) |
| 7 | `/{id}/duyet` | POST | Non-BGĐ | Duyet_AsNonBgdUser_ReturnsFailure | CXL (id:6) |
| 8 | `/{id}/duyet` | POST | BGĐ | Duyet_WhenNotChoXuLy_ReturnsFailure | DD (id:11) |
| 9 | `/{id}/tu-choi` | POST | BGĐ | TuChoi_AsBgdUser_WithReason_ReturnsOk | CXL (id:6) |
| 10 | `/{id}/tu-choi` | POST | BGĐ | TuChoi_WithoutReason_ReturnsFailure | CXL (id:6) |
| 11 | `/{id}/tra-lai` | POST | BGĐ | TraLai_AsBgdUser_WithReason_ReturnsOk | CXL (id:6) |
| 12 | `/{id}/ky-so` | POST | BGĐ | KySo_WhenDaDuyet_ReturnsOk | DD (id:11) |
| 13 | `/{id}/ky-so` | POST | BGĐ | KySo_WhenNotDaDuyet_ReturnsFailure | CXL (id:6) |
| 14 | `/{id}/chuyen-qlvb` | POST | BGĐ | ChuyenQLVB_WhenDaDuyet_ReturnsOk | DD (id:11) |
| 15 | `/{id}/chuyen-qlvb` | POST | BGĐ | ChuyenQLVB_WhenDaKySo_ReturnsOk | DKS (id:8) |
| 16 | `/{id}/phat-hanh` | POST | HC-TH | PhatHanh_AsHcthUser_ReturnsOk | DQLVB (id:9) |
| 17 | `/{id}/phat-hanh` | POST | Non-HC-TH | PhatHanh_AsNonHcthUser_ReturnsFailure | DQLVB (id:9) |
| 18 | `/{id}/gui-lai` | POST | Any | GuiLai_WhenTraLai_ReturnsOk | TL (id:12) |
| 19 | `/{id}/gui-lai` | POST | Any | GuiLai_WhenNotTraLai_ReturnsFailure | CXL (id:6) |
| 20 | Full flow | - | Mixed | FullWorkflow_TrinhToPhatHanh | - |

### Full workflow test (end-to-end)

Test `FullWorkflow_TrinhToPhatHanh` chạy toàn bộ flow:
1. Tạo VBQD mới
2. **Trinh** → tạo PheDuyetNoiDung (CXL)
3. **Duyet** (BGĐ) → CXL → DD
4. **KySo** (BGĐ) → DD → DKS
5. **ChuyenQLVB** (BGĐ) → DKS → DQLVB
6. **PhatHanh** (HC-TH) → DQLVB → DPH
7. Kiểm tra **LichSu** có đầy đủ history

## Role mapping trong test

| Client | Roles | Mô tả |
|--------|-------|-------|
| AuthedClient | QLDA_QuanTri, QLDA_TatCa | Admin mặc định |
| BgdClient | QLDA_QuanTri, QLDA_LDDV | BGĐ (Duyet/TuChoi/TraLai/KySo/ChuyenQLVB) |
| HcthClient | QLDA_HC_TH | P.HC-TH (PhatHanh) |

## Test Status ID Constants

```csharp
private const int TrangThaiCXL = 6;   // Chờ xử lý
private const int TrangThaiDD = 11;    // Đã duyệt
private const int TrangThaiTL = 12;    // Trả lại
private const int TrangThaiDKS = 8;    // Đã ký số
private const int TrangThaiDQLVB = 9;  // Đã chuyển QLVB
```

Tests use `fixture.CreatePheDuyetNoiDungAsync(int? trangThaiId)` to seed entities with FK-based status.

## Refactoring Notes

- `DanhMucTrangThaiPheDuyetDuToan` → deleted, merged into `DanhMucTrangThaiPheDuyet` (shared entity, table `DmTrangThaiPheDuyet`)
- `DanhMucTrangThaiPheDuyetDuToanConfiguration` → deleted, replaced by shared `DanhMucTrangThaiPheDuyetConfiguration`
- `TrangThaiPheDuyetDuToanCodes` + `TrangThaiPheDuyetNoiDungCodes` → `TrangThaiPheDuyetCodes` (nested classes)
- Added `Loai` discriminator (constants: `TrangThaiPheDuyetCodes.Loai.DuToan` / `.NoiDung`)
- All PheDuyetDuToan commands updated to use `TrangThaiPheDuyetCodes.DuToan.*` + `DanhMucTrangThaiPheDuyet`
- All PheDuyetNoiDung commands use `_statusRepository` to lookup status by Ma+Loai, set `TrangThaiId` (FK)
- PheDuyetNoiDung/PheDuyetNoiDungHistory: `string TrangThai` → `int? TrangThaiId` (FK to DanhMucTrangThaiPheDuyet)
- DTOs: `string TrangThai` → `int? TrangThaiId` + `string? MaTrangThai` + `string? TenTrangThai`
- Queries use `.Include(e => e.TrangThai)` to load navigation for Ma/Ten mapping
- Composite unique index on (Ma, Loai) in DanhMucTrangThaiPheDuyetConfiguration
- CRUD wired into shared `DanhMucGetQuery`/`DanhMucGetDanhSachQuery`/`DanhMucInsertOrUpdateCommand` infrastructure

## Lưu ý khi test trên server thật

1. Migration đã được tạo, chứa seed data `DmTrangThaiPheDuyet` (12 statuses with Loai) tự động qua migration
2. Cấu hình `PhongHCTHID` trong `appsettings.json` (ID phòng HC-TH thực tế)
3. Role `QLDA_HC_TH` cần được thêm vào hệ thống role
4. DTO responses sử dụng `TrangThaiId` (int), `MaTrangThai`, `TenTrangThai` thay vì `TrangThai` string cũ
