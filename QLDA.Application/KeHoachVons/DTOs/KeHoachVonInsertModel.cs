namespace QLDA.Application.KeHoachVons.DTOs;

/// <summary>
/// Input tạo mới kế hoạch vốn
/// </summary>
public class KeHoachVonInsertModel {
    public Guid? NguonVonId { get; set; }
    public int Nam { get; set; }
    public decimal SoVon { get; set; }
    public decimal? SoVonDieuChinh { get; set; }
    public string? SoQuyetDinh { get; set; }
    public DateTimeOffset? NgayKy { get; set; }
    public string? GhiChu { get; set; }
}