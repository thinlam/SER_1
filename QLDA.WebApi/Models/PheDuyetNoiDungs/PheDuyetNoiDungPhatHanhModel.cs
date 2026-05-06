namespace QLDA.WebApi.Models.PheDuyetNoiDungs;

/// <summary>
/// Model cho request phát hành nội dung (P.HC-TH)
/// </summary>
public class PheDuyetNoiDungPhatHanhModel {
    public string? SoPhatHanh { get; set; }
    public DateTimeOffset? NgayPhatHanh { get; set; }
}
