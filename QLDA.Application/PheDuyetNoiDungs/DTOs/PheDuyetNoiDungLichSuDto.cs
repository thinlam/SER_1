namespace QLDA.Application.PheDuyetNoiDungs.DTOs;

public class PheDuyetNoiDungLichSuDto {
    public Guid Id { get; set; }
    public long? NguoiXuLyId { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public DateTimeOffset NgayXuLy { get; set; }
}
