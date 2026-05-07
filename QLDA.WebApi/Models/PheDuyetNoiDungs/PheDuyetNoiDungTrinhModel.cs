namespace QLDA.WebApi.Models.PheDuyetNoiDungs;

/// <summary>
/// Model cho request trình nội dung phê duyệt từ tiến độ
/// </summary>
public class PheDuyetNoiDungTrinhModel {
    public Guid DuAnId { get; set; }
    public int? BuocId { get; set; }
    public string? NoiDung { get; set; }
}
