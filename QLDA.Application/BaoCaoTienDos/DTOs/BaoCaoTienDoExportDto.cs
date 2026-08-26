using System.Text.Json.Serialization;

namespace QLDA.Application.BaoCaoTienDos.DTOs;

/// <summary>
/// DTO dòng export Excel danh sách báo cáo tiến độ — property khớp placeholder template DanhSachBaoCaoTienDo.xlsx ($Field)
/// </summary>
public class BaoCaoTienDoExportDto
{
    public int STT { get; set; }
    public string? TenDuAn { get; set; }
    public string? TenBuoc { get; set; }

    [JsonPropertyName("ngayBaoCao")]
    public string? NgayBaoCao { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("noiDung")]
    public string? NoiDung { get; set; }
}
