namespace QLDA.Application.DuAnCongViecs.DTOs;

public class DuAnCongViecDto {
    public Guid DuAnId { get; set; }
    public string? TenDuAn { get; set; }
    public long CongViecId { get; set; }
    public string? TenCongViec { get; set; }
    public bool IsDeleted { get; set; }
    public bool? IsHoanThanh { get; set; }
    public long? NguoiPhuTrachChinhId { get; set; }
    public string? TenNguoiPhuTrachChinh { get; set; }
    public long? NguoiTaoId { get; set; }
    public string? TenNguoiTao { get; set; }
}