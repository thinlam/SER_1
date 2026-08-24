using System.Text.Json.Serialization;

namespace QLDA.Application.HopDongs.DTOs;

/// <summary>
/// DTO dòng export Excel danh sách hợp đồng — property khớp placeholder template DanhSachHopDong.xlsx ($Field)
/// </summary>
public class HopDongExportDto
{
    public int STT { get; set; }
    public string? TenDuAn { get; set; }
    public string? TenBuoc { get; set; }

    [JsonPropertyName("soHopDong")]
    public string? SoHopDong { get; set; }

    [JsonPropertyName("ten")]
    public string? Ten { get; set; }

    [JsonPropertyName("noiDung")]
    public string? NoiDung { get; set; }

    [JsonPropertyName("donViThucHienId")]
    public string? DonViThucHienId { get; set; }

    [JsonPropertyName("giaTri")]
    public long? GiaTri { get; set; }

    [JsonPropertyName("loaiHopDongId")]
    public string? LoaiHopDongId { get; set; }

    [JsonPropertyName("ngayHopDong")]
    public string? NgayHopDong { get; set; }

    [JsonPropertyName("ngayHieuLuc")]
    public string? NgayHieuLuc { get; set; }

    [JsonPropertyName("ngayKetThuc")]
    public string? NgayKetThuc { get; set; }
}
