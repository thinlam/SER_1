# Journal — #201 Phân quyền dữ liệu 3 Dashboard vốn / giải ngân

## 07/09 — Khảo sát source & lập plan (chưa implement)

**Công việc:**
- Trace 3 màn hình dashboard xuống backend:
  - FE route → endpoint → controller → query/handler → repository/EF → entity `DuAn`.
- Xác minh:
  - `DuAn.LanhDaoPhuTrachId` (`long?`) nằm trực tiếp trên `DuAn`, lưu **`UserPortalId`**.
  - Current user: `IUserProvider.Info.UserID` (= `UserPortalId`); JWT **không** mang username.
  - 3 query dashboard hiện **không có filter phạm vi** nào — trả toàn bộ dữ liệu.
  - Hệ thống ownership `DuAnAuthorizationProvider.Filter` / `HasKhtcBypass` / `HasViewAll` (GiamDocId) hiện không áp cho dashboard.
- Nhận diện `trinh.vo`: đề xuất lookup `UserMaster.UserName` qua `UserPortalId == userId`.

**Quyết định:**
- Rule: `trinh.vo` → toàn bộ dự án; user khác → `DuAn.LanhDaoPhuTrachId == userInfo.UserID`.
- KHÔNG dùng `FilterVisible` (rộng hơn requirement).
- Filter đặt **trước** `Count/Sum/GroupBy`.
- KHÔNG migration / DB change — chỉ filter query.
- Không sửa FE; không hardcode UserId/username nhiều nơi.

**Còn chờ xác nhận:**
- FE→API mapping (FE không nằm trong repo).
- Field account `trinh.vo` thực tế là `UserMaster.UserName`.

**Docs:** tạo `docs/issues/201/` (index, report, test-workflow, journal).

---

## 07/09 — Implement filter backend (issue #201)

**Thay đổi code:**
- Tạo `QLDA.Application/Common/DashboardDataPermission.cs` — helper resolve `(IsTrinhVo, UserId)`:
  - `UserId = IUserProvider.Info.UserID` (= UserPortalId)
  - `IsTrinhVo = UserMaster.UserName == "trinh.vo"` (lookup qua `UserPortalId`)
- 3 query Dapper (Dashboard) + 1 query EF (DuAns) áp rule:
  - `trinh.vo` → không filter
  - user khác → `DuAn.LanhDaoPhuTrachId == userId` (trước `Count/Sum/GroupBy`)

**Files:**
- `DashboardTienDoGiaiNganNguonVonQuery.cs` — `AND d.LanhDaoPhuTrachId = @...`
- `DashboardGetGiaiNganTheoNguonVonQuery.cs` — thêm `JOIN DuAn` + filter trong CTE
- `DashboardGetChiTietGiaiNganQuery.cs` — `AND da.LanhDaoPhuTrachId = @...`
- `TongHopVonGiaiNganQuery.cs` — EF `.Where(d => d.LanhDaoPhuTrachId == userId)`

**Build:** toàn solution 0 error / 0 warning.

**Test tự động:** KHÔNG khả thi trên SQLite:
- Query Dapper dùng SQL Server-specific (`dbo.`, `YEAR()`, `FULL OUTER JOIN`).
- Query EF `TongHopVonGiaiNgan` có sẵn `t.NgayHoaDon.Value.Year` không translate trên SQLite (lỗi có sẵn, không do #201).
→ Đã thử viết integration test (Case A/B/C) nhưng endpoint không chạy được trên SQLite → bỏ, chuyển sang **verify thủ công trên SQL Server** (ghi trong `test-workflow.md`).

**Còn chờ:** xác nhận FE→API mapping; verify Case A/B/C trên SQL Server.
