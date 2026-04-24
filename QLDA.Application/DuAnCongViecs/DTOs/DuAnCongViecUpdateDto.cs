namespace QLDA.Application.DuAnCongViecs.DTOs;

public class DuAnCongViecUpdateDto {
    public Guid DuAnId { get; set; }
    public long CongViecId { get; set; }
    public bool? IsHoanThanh { get; set; }
    public long? NguoiPhuTrachChinhId { get; set; }
}