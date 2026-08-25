namespace QLDA.Application.DuAns.DTOs;

public class DuAnTreHanChiTietDto {
    public Guid DuAnId { get; set; }
    public string? TenDuAn { get; set; }
    public long? DonViPhuTrachChinhId { get; set; }
    public string? TenDonViPhuTrachChinh { get; set; }
    public int? BuocId { get; set; }
    public string? TenBuoc { get; set; }
    public DateOnly? NgayDuKienBatDau { get; set; }
    public DateOnly? NgayDuKienKetThuc { get; set; }
    public DateOnly? NgayThucTeBatDau { get; set; }

    public DateOnly? NgayThucTeKetThuc { get; set; }
}
