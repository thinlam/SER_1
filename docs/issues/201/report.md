# Issue #201 — Implementation Report: Phân quyền dữ liệu 3 Dashboard vốn / giải ngân

## Issue #201 | Trạng thái: PHÂN TÍCH (chưa implement)

## Tóm tắt

Thêm phân quyền dữ liệu cho 3 màn dashboard vốn / giải ngân:
- `trinh.vo` → toàn bộ dự án thuộc trung tâm.
- Các Giám đốc/Lãnh đạo khác → chỉ dự án có `DuAn.LanhDaoPhuTrachId == userInfo.UserID`.

Hiện tại **3 query dashboard này KHÔNG có filter phạm vi nào** — chúng trả về toàn bộ dữ liệu cho mọi user. Cần bổ sung filter ở **tầng query/backend, trước khi `Count/Sum/GroupBy`**.

## API mapping (ứng viên — chưa xác nhận FE)

> FE không nằm trong repo này. Bảng dưới là ứng viên suy ra từ tên endpoint / DTO / issue #9450. **Chưa xác nhận** cho tới khi có FE.

| Màn hình | Endpoint ứng viên | Controller | Query/Handler | Nơi query DuAn | Mức chắc chắn |
|---|---|---|---|---|---|
| Tình hình giải ngân | `GET /api/thong-ke/tien-do-giai-ngan-nguon-von` | `DashboardController.TienDoGiaiNganNguonVon` | `DashboardTienDoGiaiNganNguonVonQuery` (Dapper) | `ThanhToan→NghiemThu→HopDong→GoiThau→DuAn` (đã `JOIN d.DuAn`) | Cao — **chưa xác nhận** |
| Tình hình giải ngân (alt) | `GET /api/thong-ke/chi-tiet-giai-ngan` | `DashboardController.GetChiTietGiaiNgan` | `DashboardGetChiTietGiaiNganQuery` (Dapper) | `ThanhToan→…→DuAn` (đã `JOIN da.DuAn`) | Trung bình — **chưa xác nhận** |
| Giải ngân nguồn vốn | `GET /api/thong-ke/giai-ngan-theo-nguon-von` | `DashboardController.GetGiaiNganTheoNguonVon` | `DashboardGetGiaiNganTheoNguonVonQuery` (Dapper) | `ThanhToan→…→GoiThau` — **CHƯA JOIN `DuAn`** | Cao — **chưa xác nhận** |
| Giải ngân nguồn vốn (chi tiết) | `GET /api/thong-ke/chi-tiet-giai-ngan` | `DashboardController.GetChiTietGiaiNgan` | `DashboardGetChiTietGiaiNganQuery` (Dapper) | đã `JOIN da.DuAn` | Trung bình — **chưa xác nhận** |
| Tổng hợp vốn | `GET /api/du-an/von-giai-ngan` | `DuAnController.GetTongHopVonGiaiNgan` | `TongHopVonGiaiNganQuery` (EF Core) | `_duAn.GetQueryableSet()` (bảng `DuAn`) | Cao — **chưa xác nhận** |

## Trace chi tiết

### 1. Tình hình giải ngân — `DashboardTienDoGiaiNganNguonVonQuery` (Dapper)

- SQL: `ThanhToan tt JOIN NghiemThu nt ON nt.Id=tt.NghiemThuId JOIN HopDong hd ON hd.Id=nt.HopDongId JOIN GoiThau gt ON gt.Id=hd.GoiThauId JOIN DuAn d ON d.Id=gt.DuAnId`, group theo nguồn vốn / loại dự án / tháng-năm.
- Có sẵn `d.DuAn` → chỉ cần thêm `AND d.LanhDaoPhuTrachId = @LanhDaoPhuTrachId` trong `WHERE` (trước `GROUP BY`).
- File: `DashboardTienDoGiaiNganNguonVonQuery.cs:24-49`.

### 2. Giải ngân nguồn vốn — `DashboardGetGiaiNganTheoNguonVonQuery` (Dapper)

- SQL: CTE `GiaiNganTheoNguonVon` join `ThanhToan→…→GoiThau`, **chưa JOIN `DuAn`**. CTE `KeHoachVonTheoNguonVon` đọc từ bảng `KeHoachVon`.
- Cần **thêm `JOIN dbo.DuAn da ON da.Id = gt.DuAnId`** + `AND da.LanhDaoPhuTrachId = @LanhDaoPhuTrachId` **trong CTE `GiaiNganTheoNguonVon`, trước `GROUP BY`** (ảnh hưởng cả `GiaTriGiaiNgan` và `GiaTriHopDong`).
- File: `DashboardGetGiaiNganTheoNguonVonQuery.cs:16-57`.

### 3. Chi tiết giải ngân — `DashboardGetChiTietGiaiNganQuery` (Dapper)

- SQL đã `JOIN DuAn da` → thêm `AND da.LanhDaoPhuTrachId = @LanhDaoPhuTrachId`.
- File: `DashboardGetChiTietGiaiNganQuery.cs:16-29`.

### 4. Tổng hợp vốn — `TongHopVonGiaiNganQuery` (EF Core)

- Query EF: `_duAn.GetQueryableSet().Include(...).Where(e => !e.IsDeleted)` sau đó `.Where(...)` + `.Select(...)` với các `Sum` con.
- Thêm `.Where(d => d.LanhDaoPhuTrachId == userId)` **trước khi `.Select`/aggregate** khi không phải `trinh.vo`.
- File: `TongHopVonGiaiNganQuery.cs:40-78`.

## Trả lời các điểm bắt buộc

1. **3 màn dùng chung Query/Handler/helper?** — KHÔNG. Mỗi query handler độc lập; chỉ giống nhau chuỗi JOIN.
2. **Query lấy danh sách dự án từ đâu?** — EF: `IRepository<DuAn,Guid>.GetQueryableSet()`. Dapper: join ngược từ `ThanhToan` lên `DuAn`.
3. **Đã có filter đơn vị/trung tâm/user/lãnh đạo/role?** — 3 query dashboard **không có filter nào**. Hệ thống ownership `DuAnAuthorizationProvider.Filter` (gồm `LanhDaoPhuTrachId==userId || CreatedBy==userId || DonViPhuTrachChinhId==phongBanId || phối hợp`, bypass KH-TC `HasKhtcBypass` & Giám đốc `GiamDocId→HasViewAll`) **không áp dụng** cho dashboard.
4. **userInfo?** — `IUserProvider.Info` → `UserInfo.UserID` (= `UserPortalId`), `UserName`, `HoTen`, `DonViID`, `PhongBanID`. JWT chỉ điền `UserID/DonViID/PhongBanID`; `UserName/HoTen` rỗng từ token.
5. **`LanhDaoPhuTrachId`?** — trực tiếp trên `DuAn` (`DuAn.cs:166`, `long?`), lưu `UserPortalId` (comment: `DuAnSearchDto.cs:49`, `DuAnGetDanhSachExportQuery.cs:104`, `QuyetDinhLapBanQldaPrintDto.cs:15`). So sánh đúng: `DuAn.LanhDaoPhuTrachId == userInfo.UserID`.
6. **Cách nhận diện `trinh.vo`?** — chưa có chỗ nào nhận diện. Đề xuất lookup `UserMaster.UserName` qua `UserPortalId == userId` (xem bên dưới).

## Nhận diện `trinh.vo`

- JWT **không mang username**. `UserMaster` (`BuildingBlocks/.../UserMaster.cs`) có `UserName` (MaxLength 50 — tên đăng nhập), `HoTen`, `UserPortalId`, `DonViId`, `PhongBanId`. Không có field `Account`/`LoginName`.
- Có mapping đang dùng khắp source: `UserMaster.UserPortalId == userInfo.UserID`.
- Pattern user-đặc-biệt hiện có: `AppSettings.GiamDocId`/`PhongKHTCId` → `AuthorizationContext` (`AuthorizationContext.cs:49-52`).

**Phương án chọn:** **lookup `UserMaster`** (vì source có sẵn cách truy account, đúng hướng dẫn task — không cần config mới):

```csharp
var isTrinhVo = _userMaster.GetQueryableSet()
    .Where(u => u.UserPortalId == userId)
    .Select(u => u.UserName)
    .FirstOrDefault() == "trinh.vo";
```

> ⚠️ Cần xác nhận field account thực tế là `UserMaster.UserName` (chỉ xác minh được entity có `UserName`/`HoTen`, chưa truy DB).

## Kiến trúc đề xuất

Để tránh copy logic 4 chỗ, thêm **1 helper dùng chung** (trong `QLDA.Application` — khối Dashboard hoặc Common) trả `(bool IsTrinhVo, long UserId)` từ `IUserProvider` + `IRepository<UserMaster,long>`:

```
IUserProvider.Info.UserID ──┐
                           ├─→ resolve helper → (IsTrinhVo, UserId)
UserMaster.UserName ────────┘        (UserPortalId == UserID)
```

Sau đó mỗi handler dùng:

```csharp
if (!scope.IsTrinhVo)
{
    // Dapper: thêm tham số LanhDaoPhuTrachId = scope.UserId
    // EF:      query = query.Where(d => d.LanhDaoPhuTrachId == scope.UserId)
}
```

**KHÔNG sửa `FilterVisible`/`DuAnAuthorizationProvider`** dùng chung — rule dashboard khác (chỉ `LanhDaoPhuTrachId`) và hiện nó không áp cho dashboard.

## Files đã thay đổi

| File | Thay đổi |
|---|---|
| `QLDA.Application/Common/DashboardDataPermission.cs` | **Helper mới**: resolve `(IsTrinhVo, UserId)` từ `IUserProvider.Info.UserID` + `UserMaster.UserName` (qua `UserPortalId`), dùng chung 4 handler |
| `QLDA.Application/Dashboard/Queries/DashboardTienDoGiaiNganNguonVonQuery.cs` | Thêm `AND d.LanhDaoPhuTrachId = @LanhDaoPhuTrachId` trước `GROUP BY` khi không phải trinh.vo |
| `QLDA.Application/Dashboard/Queries/DashboardGetGiaiNganTheoNguonVonQuery.cs` | Thêm `JOIN dbo.DuAn da` + `AND da.LanhDaoPhuTrachId = @LanhDaoPhuTrachId` trong CTE `GiaiNganTheoNguonVon` trước `GROUP BY` |
| `QLDA.Application/Dashboard/Queries/DashboardGetChiTietGiaiNganQuery.cs` | Thêm `AND da.LanhDaoPhuTrachId = @LanhDaoPhuTrachId` |
| `QLDA.Application/DuAns/Queries/TongHopVonGiaiNganQuery.cs` (EF) | Thêm `.Where(d => d.LanhDaoPhuTrachId == userId)` trước aggregate (khi không phải trinh.vo) |

## Xác nhận kỹ thuật

- **Migration/DB change:** KHÔNG — chỉ filter query, không đụng entity/schema/migration/snapshot.
- **Filter trước `Count/Sum/GroupBy`:** đúng — đặt trong `WHERE`/trước `GROUP BY`, không lọc sau aggregate.
- **Không sửa FE** để che dữ liệu.
- **Không hardcode UserId / chuỗi username nhiều nơi.**

## Cases phải đạt

- **Case A — đăng nhập `trinh.vo`:** 3 dashboard thấy toàn bộ dự án thuộc phạm vi trung tâm, không bị giới hạn `LanhDaoPhuTrachId == userId`.
- **Case B — Giám đốc/Lãnh đạo khác (`UserId=X`):** 3 dashboard chỉ tính dự án `LanhDaoPhuTrachId == X` (danh sách, tổng vốn, giải ngân, tỷ lệ, biểu đồ/thống kê). Dự án người khác phụ trách không tính.
- **Case C — lãnh đạo không phụ trách dự án nào:** không lỗi; API trả đúng structure; số liệu/list rỗng hoặc 0 theo behavior hiện có.

## Kiểm thử

- **Build toàn solution:** 0 error / 0 warning.
- **Integration test tự động:** KHÔNG khả thi trên SQLite (xem `test-workflow.md`) — do query Dapper dùng SQL Server-specific và query EF `TongHopVonGiaiNgan` có sẵn `t.NgayHoaDon.Value.Year` không translate trên SQLite (lỗi có sẵn, không do #201). → **Verify thủ công trên SQL Server** theo 3 case.

## Trạng thái

- [x] Xác nhận field account `trinh.vo` = `UserMaster.UserName` (lookup qua `UserPortalId`)
- [x] Implement helper + 4 handler (filter trước aggregate)
- [x] Build pass (0/0)
- [x] Không migration / DB change
- [ ] Xác nhận FE→API mapping (FE không nằm trong repo)
- [ ] Verify thủ công Case A/B/C trên SQL Server
- [ ] MERGED
