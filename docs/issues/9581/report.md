# Issue 9581 — FE Mapping: Kế hoạch vốn, Quyết định phê duyệt, Khái toán kinh phí

## API Endpoints

| Action | Method | Route | Body Type |
|--------|--------|-------|-----------|
| Thêm mới | `POST` | `/api/du-an/them-moi` | `DuAnInsertDto` |
| Cập nhật | `PUT` | `/api/du-an/cap-nhat` | `DuAnUpdateModel` |
| Chi tiết | `GET` | `/api/du-an/{id}/chi-tiet` | — |

Response cho cả 3 endpoint: `ResultApi<DuAnDto>`

---

## 1. Kế hoạch vốn (KeHoachVon) — MỚI

### Vị trí form: section "Kế hoạch vốn", nằm sau section "Dự toán"

### UI tham khảo
![Kế hoạch vốn UI](image-1.png)

### Input Fields (Create)

> Nested trong `DuAnInsertDto.KeHoachVons: KeHoachVonInsertModel[]`

| FE Field Label | API Field | Type | Required | Notes |
|----------------|-----------|------|----------|-------|
| Nguồn vốn | `NguonVonId` | `int?` | **Yes** | Combobox → load theo DanhMucNguonVon đã chọn ở DanhSachNguonVon |
| Năm KH | `Nam` | `int` | **Yes** | Number input, năm tài chính |
| Vốn được giao | `SoVon` | `decimal` | **Yes** | Number input, nhập số tiền |
| Vốn điều chỉnh | `SoVonDieuChinh` | `decimal?` | No | Number input, nhập số tiền |
| Số QĐ | `SoQuyetDinh` | `string?` | No | Text input |
| Ngày QĐ | `NgayKy` | `DateTimeOffset?` | No | Date picker |
| Ghi chú | `GhiChu` | `string?` | No | Textarea |
| File đính kèm | `DanhSachTepDinhKem` | `TepDinhKemInsertDto[]` | No | Upload component |

### Input Fields (Update)

> Nested trong `DuAnUpdateModel.KeHoachVons: KeHoachVonUpdateModel[]`

| FE Field Label | API Field | Type | Required | Notes |
|----------------|-----------|------|----------|-------|
| (hidden) | `Id` | `Guid?` | Auto | Null = tạo mới, có giá trị = cập nhật |
| Nguồn vốn | `NguonVonId` | `int?` | **Yes** | Combobox |
| Năm KH | `Nam` | `int` | **Yes** | Number input |
| Vốn được giao | `SoVon` | `decimal` | **Yes** | Number input |
| Vốn điều chỉnh | `SoVonDieuChinh` | `decimal?` | No | Number input |
| Số QĐ | `SoQuyetDinh` | `string?` | No | Text input |
| Ngày QĐ | `NgayKy` | `DateTimeOffset?` | No | Date picker |
| Ghi chú | `GhiChu` | `string?` | No | Textarea |
| File đính kèm | `DanhSachTepDinhKem` | `TepDinhKemInsertOrUpdateDto[]` | No | Upload component (có `Id` để update) |

### Response Fields

> Nested trong `DuAnDto.KeHoachVons: KeHoachVonDto[]`

| API Field | Type | Notes |
|-----------|------|-------|
| `Id` | `Guid` | ID kế hoạch vốn |
| `DuAnId` | `Guid` | ID dự án cha |
| `NguonVonId` | `int?` | ID danh mục nguồn vốn |
| `Nam` | `int` | Năm kế hoạch |
| `SoVon` | `decimal` | Vốn được giao |
| `SoVonDieuChinh` | `decimal?` | Vốn điều chỉnh |
| `SoQuyetDinh` | `string?` | Số quyết định |
| `NgayKy` | `DateTimeOffset?` | Ngày ký |
| `GhiChu` | `string?` | Ghi chú |
| `DanhSachTepDinhKem` | `TepDinhKemDto[]?` | File đính kèm |

### JSON Example (Create body)

```json
{
  "TenDuAn": "Dự án ABC",
  "...": "(các field khác)",
  "KeHoachVons": [
    {
      "NguonVonId": 1,
      "Nam": 2026,
      "SoVon": 5000000000,
      "SoVonDieuChinh": null,
      "SoQuyetDinh": "123/QĐ-UBND",
      "NgayKy": "2026-01-15T00:00:00+07:00",
      "GhiChu": "Kế hoạch vốn năm 2026",
      "DanhSachTepDinhKem": [
        {
          "Type": "pdf",
          "FileName": "khv-2026.pdf",
          "OriginalName": "Kế hoạch vốn 2026.pdf",
          "Path": "/uploads/khv-2026.pdf",
          "Size": 102400
        }
      ]
    }
  ]
}
```

### JSON Example (Update body)

```json
{
  "Id": "guid-du-an",
  "KeHoachVons": [
    {
      "Id": "guid-khv-1",
      "NguonVonId": 1,
      "Nam": 2026,
      "SoVon": 5000000000,
      "SoVonDieuChinh": 5500000000,
      "SoQuyetDinh": "456/QĐ-UBND",
      "NgayKy": "2026-03-20T00:00:00+07:00",
      "GhiChu": "Đã điều chỉnh",
      "DanhSachTepDinhKem": [
        {
          "Id": "guid-file-1",
          "Type": "pdf",
          "FileName": "khv-2026-v2.pdf",
          "OriginalName": "Kế hoạch vốn 2026 v2.pdf",
          "Path": "/uploads/khv-2026-v2.pdf",
          "Size": 204800
        }
      ]
    },
    {
      "Id": null,
      "Nam": 2027,
      "SoVon": 3000000000,
      "NguonVonId": 2
    }
  ]
}
```

---

## 2. Quyết định phê duyệt — MỚI

### Vị trí form: section riêng trong form Dự án

### UI tham khảo
![Quyết định phê duyệt](image-3.png)

### Input Fields (Create)

> Trực tiếp trên `DuAnInsertDto`

| FE Field Label | API Field | Type | Required | Notes |
|----------------|-----------|------|----------|-------|
| Số quyết định phê duyệt | `SoQuyetDinhPheDuyet` | `string?` | No | Text input |
| Ngày quyết định | `NgayQuyetDinhPheDuyet` | `DateTimeOffset?` | No | Date picker |
| File đính kèm QĐ | `DanhSachTepQuyetDinh` | `TepDinhKemInsertDto[]` | No | Upload component |

### Input Fields (Update)

> Trực tiếp trên `DuAnUpdateModel`

| FE Field Label | API Field | Type | Required | Notes |
|----------------|-----------|------|----------|-------|
| Số quyết định phê duyệt | `SoQuyetDinhPheDuyet` | `string?` | No | Text input |
| Ngày quyết định | `NgayQuyetDinhPheDuyet` | `DateTimeOffset?` | No | Date picker |
| File đính kèm QĐ | `DanhSachTepQuyetDinh` | `TepDinhKemInsertOrUpdateDto[]` | No | Upload component |

### Response Fields

> Trực tiếp trên `DuAnDto`

| API Field | Type | Notes |
|-----------|------|-------|
| `SoQuyetDinhPheDuyet` | `string?` | Số QĐ phê duyệt |
| `NgayQuyetDinhPheDuyet` | `DateTimeOffset?` | Ngày QĐ |
| `DanhSachTepQuyetDinh` | `TepDinhKemDto[]?` | File QĐ phê duyệt |

---

## 3. Khái toán kinh phí — MỚI

### Vị trí form: cùng row với "Tổng mức đầu tư", nhập giống format TongMucDauTu

### UI tham khảo
![Khái toán kinh phí](image-4.png)

### Input Fields

| FE Field Label | API Field | Type | Required | Notes |
|----------------|-----------|------|----------|-------|
| Khái toán kinh phí | `KhaiToanKinhPhi` | `decimal?` | No | Number input, giống format TongMucDauTu |

> Có trên cả `DuAnInsertDto` và `DuAnUpdateModel`

### Response Fields

| API Field | Type | Notes |
|-----------|------|-------|
| `KhaiToanKinhPhi` | `decimal?` | Khái toán kinh phí |

---

## 4. File đính kèm (TepDinhKem) — Shared Structure

### TepDinhKemInsertDto (Create)

| API Field | Type | Required | Notes |
|-----------|------|----------|-------|
| `Type` | `string?` | No | Loại tệp (pdf, docx, ...) |
| `FileName` | `string?` | No | Tên tệp lưu trên server |
| `OriginalName` | `string?` | No | Tên tệp gốc khi upload |
| `Path` | `string?` | No | Đường dẫn tệp |
| `Size` | `long` | No | Kích thước (bytes) |
| `ParentId` | `Guid?` | No | File gốc (dùng cho ký số) |

### TepDinhKemInsertOrUpdateDto (Update)

| API Field | Type | Required | Notes |
|-----------|------|----------|-------|
| `Id` | `Guid?` | **Key** | Null = tạo mới, có giá trị = cập nhật |
| *(các field còn lại giống Insert)* | | | |

### TepDinhKemDto (Response)

| API Field | Type | Notes |
|-----------|------|-------|
| `Id` | `Guid?` | ID tệp |
| `GroupId` | `string?` | Nhóm (auto, BE set) |
| `GroupType` | `string?` | Loại nhóm (auto, BE set) |
| `Type` | `string?` | Loại tệp |
| `FileName` | `string?` | Tên tệp server |
| `OriginalName` | `string?` | Tên tệp gốc |
| `Path` | `string?` | Đường dẫn |
| `Size` | `long` | Kích thước |
| `ParentId` | `Guid?` | File gốc (ký số) |

---

## 5. Full Response Example (GET chi-tiet)

```json
{
  "isSuccess": true,
  "data": {
    "id": "guid-du-an",
    "tenDuAn": "Dự án ABC",
    "khaiToanKinhPhi": 10000000000,
    "soQuyetDinhPheDuyet": "123/QĐ-UBND",
    "ngayQuyetDinhPheDuyet": "2026-01-15T00:00:00+07:00",
    "danhSachTepQuyetDinh": [
      {
        "id": "guid-file-qd",
        "groupId": "guid-du-an",
        "groupType": "QuyetDinhPheDuyetNhiemVu",
        "type": "pdf",
        "fileName": "qd-phe-duyet.pdf",
        "originalName": "Quyết định phê duyệt.pdf",
        "path": "/uploads/qd-phe-duyet.pdf",
        "size": 51200
      }
    ],
    "keHoachVons": [
      {
        "id": "guid-khv-1",
        "duAnId": "guid-du-an",
        "nguonVonId": 1,
        "nam": 2026,
        "soVon": 5000000000,
        "soVonDieuChinh": null,
        "soQuyetDinh": "456/QĐ-UBND",
        "ngayKy": "2026-02-10T00:00:00+07:00",
        "ghiChu": null,
        "danhSachTepDinhKem": [
          {
            "id": "guid-file-khv",
            "groupId": "guid-khv-1",
            "groupType": "KeHoachVon",
            "fileName": "khv-2026.pdf",
            "originalName": "Kế hoạch vốn 2026.pdf",
            "size": 102400
          }
        ]
      }
    ],
    "duToans": [
      {
        "id": "guid-dt-1",
        "duAnId": "guid-du-an",
        "soDuToan": 8000000000,
        "namDuToan": 2026,
        "soQuyetDinhDuToan": "789/QĐ-UBND",
        "ngayKyDuToan": "2026-01-05T00:00:00+07:00",
        "ghiChu": null,
        "danhSachTepDinhKem": []
      }
    ]
  }
}
```

---

## 6. Tóm tắt thay đổi FE

| # | Thay đổi | Loại |
|---|----------|------|
| 1 | Thêm section "Kế hoạch vốn" vào form Thêm mới/Cập nhật Dự án | **New section** |
| 2 | Thêm fields `SoQuyetDinhPheDuyet`, `NgayQuyetDinhPheDuyet` + upload `DanhSachTepQuyetDinh` | **New fields** |
| 3 | Thêm field `KhaiToanKinhPhi` (decimal, giống format TongMucDauTu) | **New field** |
| 4 | Response Create/Update trả về `DuAnDto` đầy đủ (bao gồm files) thay vì chỉ `{ Id }` | **Breaking change** |

### Lưu ý quan trọng
- `KeHoachVons` là array → cho phép thêm nhiều kế hoạch vốn, có nút "Thêm dòng"
- Update KeHoachVon: `Id = null` → tạo mới, `Id = guid` → cập nhật bản cũ
- `NguonVonId` (KHV) là `int` (không phải Guid) — load từ combobox DanhMucNguonVon
- File đính kèm KeHoachVon: `GroupType = "KeHoachVon"` (BE tự set)
- File QĐ phê duyệt: `GroupType = "QuyetDinhPheDuyetNhiemVu"` (BE tự set)
