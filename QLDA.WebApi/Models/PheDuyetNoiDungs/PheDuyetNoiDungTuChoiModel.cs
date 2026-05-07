namespace QLDA.WebApi.Models.PheDuyetNoiDungs;

/// <summary>
/// Model cho request từ chối nội dung phê duyệt
/// </summary>
public class PheDuyetNoiDungTuChoiModel {
    /// <summary>
    /// Lý do từ chối (bắt buộc)
    /// </summary>
    public string NoiDung { get; set; } = string.Empty;
}
