namespace QLDA.Domain.Entities.DanhMuc;

/// <summary>
/// Danh mục trạng thái phê duyệt (dùng chung cho dự toán & nội dung trình duyệt)
/// </summary>
public class DanhMucTrangThaiPheDuyet : DanhMuc<int>, IAggregateRoot, IMayHaveStt {
    public int? Stt { get; set; }

    /// <summary>
    /// Phân loại: "DuToan" hoặc "NoiDung"
    /// </summary>
    public string? Loai { get; set; }

}
