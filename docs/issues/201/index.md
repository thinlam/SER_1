# Issue #201 — Phân quyền dữ liệu 3 màn hình Dashboard vốn / giải ngân

## Mô tả nghiệp vụ

Cần chỉnh **phân quyền dữ liệu** cho 3 màn hình dashboard liên quan vốn / giải ngân:

1. **Tình hình giải ngân** — FE route `/quan-ly-du-an/dashboard/tinh-hinh-giai-ngan`
2. **Giải ngân nguồn vốn** — FE route `/quan-ly-du-an/dashboard/giai-ngan-nguon-von`
3. **Tổng hợp vốn** — FE route `/quan-ly-du-an/dashboard/tong-hop-von`

> ⚠️ **Lưu ý:** FE source không nằm trong repo này (`E:\SER` là backend QLDA). Việc mapping route FE → endpoint backend chỉ là **ứng viên suy ra từ tên endpoint / DTO / issue #9450**, chưa được xác nhận bằng FE. Phải xác nhận lại trước khi coi là fact.

## Actors

| Tác nhân | Quyền phạm vi dữ liệu |
|----------|------------------------|
| `trinh.vo` | Xem **toàn bộ** dự án thuộc trung tâm — không filter theo `LanhDaoPhuTrachId` |
| Giám đốc / Lãnh đạo khác | Chỉ xem dự án do chính mình phụ trách |

## Rule phân quyền

```csharp
if (isTrinhVo)
{
    // toàn bộ dự án thuộc trung tâm (giữ phạm vi hiện tại, không thêm filter)
}
else
{
    query = query.Where(x => x.LanhDaoPhuTrachId == userInfo.UserID);
}
```

### Chi tiết

- **Trường hợp 1 — `trinh.vo`:** không filter theo `LanhDaoPhuTrachId` → giữ toàn bộ phạm vi dự án.
- **Trường hợp 2 — các Giám đốc/Lãnh đạo khác:** chỉ tính dữ liệu các dự án có `DuAn.LanhDaoPhuTrachId == userInfo.UserID`.

### Điều kiện cần xác minh

- Property user hiện tại trong source là **`UserID`** (`UserInfo.UserID`), không phải `UserId`.
- `LanhDaoPhuTrachId` nằm **trực tiếp trên `DuAn`** (`DuAn.cs:166`, kiểu `long?`), lưu giá trị **`UserPortalId`** của `UserMaster`.
- Không đổi tên field nếu source hiện tại khác requirement.

## UI notes

- 3 màn là các dashboard thống kê vốn / giải ngân theo năm.
- Phân quyền áp dụng cho **toàn bộ số liệu** của màn: danh sách, tổng vốn, giải ngân, tỷ lệ, biểu đồ/thống kê theo nguồn vốn.
- **Không sửa FE để che dữ liệu** thay cho phân quyền backend.

## Related issues

- #9450 — Dashboard giải ngân theo nguồn vốn & theo năm (nguồn gốc các endpoint `thong-ke/*`).

## Files / source liên quan (trace ban đầu)

| Loại | File |
|------|------|
| Controller | `QLDA.WebApi/Controllers/DashboardController.cs`, `QLDA.WebApi/Controllers/DuAnController.cs` |
| Query (Dapper) | `QLDA.Application/Dashboard/Queries/DashboardTienDoGiaiNganNguonVonQuery.cs`, `DashboardGetGiaiNganTheoNguonVonQuery.cs`, `DashboardGetChiTietGiaiNganQuery.cs` |
| Query (EF) | `QLDA.Application/DuAns/Queries/TongHopVonGiaiNganQuery.cs` |
| Entity | `QLDA.Domain/Entities/DuAn.cs` (`LanhDaoPhuTrachId`) |
| User | `BuildingBlocks/.../UserMaster.cs`, `BuildingBlocks/.../DTOs/UserInfo.cs`, `UserProvider.cs`, `IUserProvider.cs` |
| Auth (hiện hữu, không dùng cho dashboard) | `QLDA.Application/Authorization/DuAnAuthorizationProvider.cs`, `AuthorizationContext.cs` |
