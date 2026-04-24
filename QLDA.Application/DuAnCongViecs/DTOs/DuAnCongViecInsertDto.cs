namespace QLDA.Application.DuAnCongViecs.DTOs;

public class DuAnCongViecInsertDto {
    public Guid DuAnId { get; set; }
    public long CongViecId { get; set; }
    public long? NguoiPhuTrachChinhId { get; set; }
}