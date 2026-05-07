namespace QLDA.WebApi.Models.PheDuyetNoiDungs;

/// <summary>
/// Model cho request trả lại nội dung phê duyệt
/// </summary>
public class PheDuyetNoiDungTraLaiModel {
    /// <summary>
    /// Lý do trả lại (bắt buộc)
    /// </summary>
    public string NoiDung { get; set; } = string.Empty;
}
