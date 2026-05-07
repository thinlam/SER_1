# Issue 9584 — Báo cáo tình hình phân quyền theo phòng ban

## Yêu cầu

| # | Yêu cầu | Trạng thái |
|---|---------|------------|
| 1 | Phòng KH-TC (xác định theo phòng ID) và BGĐ cho phép xem tất cả dự án | ⚠️ KH-TC: ✅ / BGĐ: chưa xác định role |
| 2 | Phòng chuyên môn chỉ xem được dự án do phòng là Phụ trách chính / Theo dõi | ✅ Đã hoàn thành |
| 3 | Tương tự cho Gói thầu, Hợp đồng, Văn bản | ✅ Đã hoàn thành |
| 4 | Phòng KH-TC và BGĐ có thể xem/sửa/xóa tất cả | ⚠️ KH-TC: ✅ / BGĐ: chưa xác định role |
| 5 | Phòng phụ trách chính và theo dõi cũng xem/sửa/xóa dự án tham gia | ⚠️ Xem: ✅ / Sửa/Xóa: chưa theo phòng cụ thể |

---

## Kiến trúc hiện tại

### Hệ thống phân quyền 2 tầng

```
Layer 1: JWT Role (Login)          Layer 2: Permission Toggle (Runtime)
┌─────────────────────┐           ┌──────────────────────────────┐
│ QLDA_TatCa (Admin)   │──────────→│ CauHinhVaiTroQuyen table     │
│ QLDA_QuanTri         │           │ (VaiTro, QuyenId, KichHoat) │
│ QLDA_LDDV            │           │                              │
│ QLDA_LD              │           │ ↓ PolicyProvider cache       │
│ QLDA_ChuyenVien      │           │ ↓ VisibilityFilterExtensions │
└─────────────────────┘           └──────────────────────────────┘
```

### Role → Permission mapping (seed data)

| Role | XemTatCa | XemTheoPhong | Tao | Sua | Xoa | PheDuyet |
|------|----------|--------------|-----|-----|-----|----------|
| `QLDA_TatCa` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `QLDA_QuanTri` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `QLDA_LDDV` | ✅ | — | — | — | — | — |
| `QLDA_LD` | ✅ | — | — | — | — | — |
| `QLDA_ChuyenVien` | — | ✅ | ✅ | ✅ | — | — |

### Visibility filter logic

```
User request → IPolicyProvider.HasPermission(roles, "DuAn.XemTatCa")
  ├─ YES → show all projects
  └─ NO → IPolicyProvider.HasPermission(roles, "DuAn.XemTheoPhong")
       ├─ YES + có PhongBanID → filter by:
       │     e.DonViPhuTrachChinhId == user.PhongBanID
       │     OR e.DuAnChiuTrachNhiemXuLys.Any(i => i.RightId == user.PhongBanID)
       └─ NO → return empty
```

**Áp dụng cho entity con** (GoiThau, HopDong, VanBan, etc.) qua `ApplyDuAnChildVisibility()` — filter theo DuAn visibility.

---

## Files liên quan

| File | Vai trò |
|------|---------|
| `Domain/Constants/RoleConstants.cs` | Định nghĩa 5 roles |
| `Domain/Constants/PermissionConstants.cs` | Định nghĩa permission keys + default mapping |
| `Domain/Entities/CauHinhVaiTroQuyen.cs` | Entity role-permission toggle |
| `Domain/Entities/DanhMuc/DanhMucQuyen.cs` | Danh mục quyền (seed data) |
| `Application/Providers/IPolicyProvider.cs` | Interface check permission |
| `Application/Providers/PolicyProvider.cs` | Implementation: load DB → cache → check |
| `Application/Common/Extensions/VisibilityFilterExtensions.cs` | IQueryable filter theo permission |
| `WebApi/Controllers/CauHinhVaiTroQuyenController.cs` | API config quyền runtime |
| `Persistence/Configurations/CauHinhVaiTroQuyenConfiguration.cs` | Seed default permissions |
| `WebApi/ConfigurationOptions/AppSettings.cs` | Config PhongKeToanID (pattern mở rộng) |
| `Application/Providers/IAppSettingsProvider.cs` | Interface đọc config phòng ID |
| `Application/Common/DTOs/UserInfo.cs` | UserInfo.PhongBanID (từ JWT claim) |

---

## Cách phòng ban được xác định

### Cơ chế: Token Claim → IOptions đối chiếu

Phòng ban được xác định bằng cách đối chiếu **PhongBanID từ JWT token** (claim của user) với **ID phòng config từ appsettings.json**.

```
JWT Token Claims                    appsettings.json
┌──────────────────┐               ┌──────────────────────────┐
│ user.PhongBanID   │─── compare ──→│ "PhongKeToanID": 219     │
│ (long?, từ claim) │               │ "PhongKhtcID": ???       │
│                   │               │ "PhongBgdID": ???        │
└──────────────────┘               └──────────────────────────┘
        │                                    │
        └──── IUserProvider.Info.PhongBanID  └──── IAppSettingsProvider (IOptions<AppSettings>)
```

### Pattern đã có sẵn (tham khảo ThanhToan)

Module ThanhToan đã implement pattern này:

**`AppSettings.cs`:**
```csharp
public long PhongKeToanID { get; set; }  // = 219 trong appsettings.json
```

**`IAppSettingsProvider.cs`:**
```csharp
long PhongKeToanID { get; }
```

**`ThanhToanInsertCommandHandler.cs`:**
```csharp
private void ValidatePhongKeToanPermission() {
    ManagedException.ThrowIf(
        _userProvider.Info.PhongBanID != _settings.PhongKeToanID,
        "Chỉ phòng kế toán có quyền thực hiện thao tác này"
    );
}
```

### Cần mở rộng cho issue 9584

Thêm config vào `AppSettings`:
```json
{
    "PhongKeToanID": 219,
    "PhongKhtcID": <id phòng KH-TC>,    // MỚI - cho xem/sửa/xóa tất cả dự án
    "PhongBgdID": <id phòng BGĐ>         // MỚI - cho xem/sửa/xóa tất cả dự án
}
```

Thêm vào `IAppSettingsProvider`:
```csharp
long PhongKhtcID { get; }
long PhongBgdID { get; }
```

Check logic:
```csharp
bool IsPhongKhtcOrBgd = user.Info.PhongBanID == settings.PhongKhtcID
                      || user.Info.PhongBanID == settings.PhongBgdID;
```

### Phòng KH-TC → xác định theo phòng ID

Phòng KH-TC được xác định bằng `IUserProvider.Info.PhongBanID` so khớp với `IAppSettingsProvider.PhongKhtcID`.

Hiện tại: User phòng KH-TC được gán role `QLDA_QuanTri` → có permission `DuAn.XemTatCa` → `ApplyDuAnVisibility()` trả tất cả.

Cần: Thêm `PhongKhtcID` vào appsettings + đối chiếu trực tiếp PhongBanID.

### BGĐ (Ban Giám đốc) → chưa xác định role + ID

BGĐ chưa được map. Cần xác định:
- **ID phòng BGĐ** — giá trị cụ thể để thêm vào appsettings.json
- **Role** — BGĐ dùng role nào? `QLDA_LD`? `QLDA_LDDV`? Hoặc role mới?
- Nếu dùng `QLDA_LD` → hiện chỉ có `XemTatCa` (read-only), chưa có Tao/Sua/Xoa

Pattern: `user.Info.PhongBanID == settings.PhongBgdID` → cho phép CRUD tất cả.

### Phòng chuyên môn → xem theo phòng

User được gán role `QLDA_ChuyenVien` → có permission `DuAn.XemTheoPhong` → filter:
- `DonViPhuTrachChinhId == user.PhongBanID` (phòng phụ trách chính)
- `DuAnChiuTrachNhiemXuLys.Any(i => i.RightId == user.PhongBanID)` (phòng phối hợp/theo dõi)

Entity `DuAnChiuTrachNhiemXuLy` liên kết DuAn với phòng ban, có loại:
- `DonViPhoiHop` — Đơn vị phối hợp
- `DonViTheoDoi` — Đơn vị theo dõi

---

## Điều chỉnh runtime (không deploy)

Admin dùng API để bật/tắt permission:

```
GET  /api/cau-hinh-vai-tro-quyen/danh-sach?vaiTro=QLDA_ChuyenVien&nhomQuyen=DuAn
PUT  /api/cau-hinh-vai-tro-quyen/cap-nhat
```

Ví dụ: tắt `DuAn.XemTatCa` cho `QLDA_LDDV`:
```json
{ "Items": [{ "Id": <row_id>, "KichHoat": false }] }
```

---

## Gaps cần xử lý

| # | Gap | Mô tả | Ưu tiên |
|---|-----|-------|---------|
| 1 | CUD theo phòng | Create/Update/Delete chỉ check role (`[Authorize(Roles=...)]`), **không** check permission toggle. Chuyên viên phòng A có thể sửa dự án phòng B nếu có role `QLDA_ChuyenVien` | High |
| 2 | DonViPhuTrachChinhId type | `DuAn.DonViPhuTrachChinhId` là `long?`, `user.Info.PhongBanID` cũng `long?` — đúng kiểu, OK | — |
| 3 | Theo dõi filter | `ApplyDuAnVisibility` filter theo `DuAnChiuTrachNhiemXuLys` (phối hợp + theo dõi) — cả 2 loại đều match, đúng theo yêu cầu | — |
| 4 | Xem/Sửa/Xóa cho phòng tham gia | Yêu cầu #5 nói phòng phụ trách + theo dõi cũng sửa/xóa được. Hiện tại visibility filter chỉ áp cho **đọc** (list queries), chưa áp cho CUD operations | Medium |

### Unresolved questions
- **BGĐ dùng role nào?** — Cần xác định role cho Ban Giám đốc (QLDA_LD / QLDA_LDDV / role mới?)
- **ID phòng KH-TC là gì?** — Cần giá trị `PhongKhtcID` để thêm vào appsettings.json
- **ID phòng BGĐ là gì?** — Cần giá trị `PhongBgdID` để thêm vào appsettings.json
- Có cần thêm permission riêng cho "sửa/xóa theo phòng" hay dùng chung `XemTheoPhong`?
- Có cần separate `DonViPhoiHop` vs `DonViTheoDoi` trong filter (hiện gộp chung)?
