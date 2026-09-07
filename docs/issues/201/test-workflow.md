# Issue #201 — Test Workflow: Phân quyền dữ liệu 3 Dashboard vốn / giải ngân

## Thông tin chung

- **Issue**: #201
- **Trạng thái**: implement (filter backend), chưa tự động test được trên SQLite
- **Entity liên quan**: `DuAn.LanhDaoPhuTrachId` (long?, lưu UserPortalId), `UserMaster.UserName` (account)
- **Current user**: JWT claim `UserId` → `UserInfo.UserID` (= `UserPortalId`)

## ⚠️ Giới hạn test tự động

Các endpoint của 3 màn này **KHÔNG thể integration-test trên SQLite** của test infra:

1. **Các query Dapper** (`tien-do-giai-ngan-nguon-von`, `giai-ngan-theo-nguon-von`, `chi-tiet-giai-ngan`) dùng SQL Server-specific:
   - `dbo.` schema, hàm `YEAR()`, `FULL OUTER JOIN`, `ISNULL()` → SQLite không hỗ trợ.
   - `IDapperRepository` mở connection theo `DefaultConnection` — không dùng SQLite.
2. **Query EF `TongHopVonGiaiNgan`** có sẵn `t.NgayHoaDon.Value.Year` — EF **không translate** `DateTimeOffset.Year` trên SQLite (chỉ SQL Server).
   → Lỗi này có **sẵn từ trước** (không do issue #201), query chưa từng có test.

Do đó không có test file tự động cho issue này (phù hợp hiện trạng repo — các dashboard này chưa từng có test). **Phải verify thủ công trên SQL Server.**

## Chạy build

```bash
# Build toàn solution (0 error / 0 warning)
dotnet build SER.sln -c Debug
```

## Verify thủ công trên SQL Server

### Chuẩn bị dữ liệu

Tạo ít nhất 2 dự án với `LanhDaoPhuTrachId` khác nhau:

| Dự án | LanhDaoPhuTrachId (= UserPortalId) |
|-------|-------------------------------------|
| DA_X | = UserId của lãnh đạo test (vd 100) |
| DA_Y | = UserId khác (vd 200) |

Và đảm bảo `UserMaster` có dòng:

```sql
SELECT User_MasterID, User_PortalID, UserName FROM USER_MASTER
WHERE UserName = 'trinh.vo';  -- đối chiếu UserPortalId của tài khoản trinh.vo
```

### Case A — `trinh.vo` (toàn bộ dự án)

- Đăng nhập bằng tài khoản `trinh.vo`.
- Gọi 3 (4) endpoint, kỳ vọng **có cả DA_X và DA_Y** trong mọi số liệu:
  - `GET /api/thong-ke/tien-do-giai-ngan-nguon-von?nam={year}`
  - `GET /api/thong-ke/giai-ngan-theo-nguon-von?nam={year}`
  - `GET /api/thong-ke/chi-tiet-giai-ngan?nam={year}`
  - `GET /api/du-an/von-giai-ngan?Nam={year}`

### Case B — lãnh đạo khác (UserId = X)

- Đăng nhập tài khoản có `UserID = X`.
- Kỳ vọng **chỉ DA_X** (`LanhDaoPhuTrachId == X`), **không có DA_Y** trong:
  - danh sách / tổng vốn / giải ngân / tỷ lệ / biểu đồ theo nguồn vốn.
- Xác nhận filter nằm **trước** `Sum/GroupBy` (số liệu đúng theo phạm vi).

### Case C — lãnh đạo không phụ trách dự án nào

- Đăng nhập tài khoản `UserID = Z` (không phải trinh.vo, không phụ trách dự án).
- Kỳ vọng: **không lỗi**, trả đúng structure; list rỗng / số liệu 0.

## Lưu ý khi verify

1. JWT không mang username → hệ thống nhận diện `trinh.vo` bằng lookup `UserMaster.UserName` qua `UserPortalId == userID`.
2. Nếu `UserMaster` chưa có dòng `UserName='trinh.vo'` cho đúng `UserPortalId`, user sẽ bị xem là lãnh đạo thường (filter theo `LanhDaoPhuTrachId`) — đúng logic.
3. Không tạo migration / DB change cho task này.
