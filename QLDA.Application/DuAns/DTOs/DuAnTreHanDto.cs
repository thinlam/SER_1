namespace QLDA.Application.DuAns.DTOs;

public class DuAnTreHanDto {
    public Guid DuAnId { get; set; }
    public string? TenDuAn { get; set; }
    public long? DonViPhuTrachChinhId { get; set; }
    public string? TenDonViPhuTrachChinh { get; set; }
    public int? BuocId { get; set; }
    public string? TenBuoc { get; set; }
    public int? SoLuong { get; set; }
}
