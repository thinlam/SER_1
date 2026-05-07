namespace QLDA.Application.PheDuyetNoiDungs.DTOs;

public class PheDuyetNoiDungLichSuDto {
    public Guid Id { get; set; }
    public long? NguoiXuLyId { get; set; }
    public int? TrangThaiId { get; set; }
    public string? MaTrangThai { get; set; }
    public string? TenTrangThai { get; set; }
    public string? NoiDung { get; set; }
    public DateTimeOffset NgayXuLy { get; set; }
}
