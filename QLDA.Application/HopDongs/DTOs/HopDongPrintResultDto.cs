namespace QLDA.Application.HopDongs.DTOs;

/// <summary>
/// Search DTO cho print/export hợp đồng — không phân trang
/// </summary>
public record HopDongPrintResultDto {
 
    public int? Stt { get; set; }
    public string? TenDuAn { get; set; }
    public string? TenBuoc { get; set; }
    public string? ten { get; set; }
    public string? soHopDong { get; set; }
    public string? noiDung { get; set; }
    public string? ngayHopDong { get; set; }
    public string? donViThucHienId { get; set; }
    public string? ngayHieuLuc { get; set; }
    public string? ngayKetThuc { get; set; }
    public long? giaTri { get; set; }
    public string? loaiHopDongId { get; set; }
}
