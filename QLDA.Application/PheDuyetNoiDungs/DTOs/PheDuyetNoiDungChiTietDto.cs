using QLDA.Application.TepDinhKems.DTOs;

namespace QLDA.Application.PheDuyetNoiDungs.DTOs;

public class PheDuyetNoiDungChiTietDto : PheDuyetNoiDungDto {
    public string? NguoiKy { get; set; }
    public DateTimeOffset? NgayKy { get; set; }
    public long? NguoiXuLyId { get; set; }
    public List<PheDuyetNoiDungLichSuDto>? LichSu { get; set; }
}
