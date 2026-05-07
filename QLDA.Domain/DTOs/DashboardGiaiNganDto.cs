namespace QLDA.Domain.DTOs;

/// <summary>
/// DTO thống kê giải ngân theo nguồn vốn
/// </summary>
public class DashboardGiaiNganTheoNguonVonDto {
    public int? NguonVonId { get; set; }
    public string? TenNguonVon { get; set; }
    public decimal GiaTriGiaiNgan { get; set; }
    public decimal GiaTriHopDong { get; set; }
}
