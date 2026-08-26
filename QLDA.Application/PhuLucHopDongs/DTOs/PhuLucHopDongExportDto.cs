using System.Text.Json.Serialization;

namespace QLDA.Application.PhuLucHopDongs.DTOs;

/// <summary>
/// DTO dòng export Excel danh sách phụ lục hợp đồng — property khớp placeholder template DanhSachPhuLucHopDong.xlsx ($Field)
/// </summary>
public class PhuLucHopDongExportDto
{
    public int STT { get; set; }
    public string? TenDuAn { get; set; }
    public string? TenBuoc { get; set; }

    [JsonPropertyName("soPhuLucHopDong")]
    public string? SoPhuLucHopDong { get; set; }

    [JsonPropertyName("ten")]
    public string? Ten { get; set; }

    [JsonPropertyName("hopDongId")]
    public string? HopDongId { get; set; }

    [JsonPropertyName("giaTri")]
    public long? GiaTri { get; set; }

    [JsonPropertyName("ngay")]
    public string? Ngay { get; set; }

    [JsonPropertyName("ngayDuKienKetThuc")]
    public string? NgayDuKienKetThuc { get; set; }

    [JsonPropertyName("noiDung")]
    public string? NoiDung { get; set; }
}
