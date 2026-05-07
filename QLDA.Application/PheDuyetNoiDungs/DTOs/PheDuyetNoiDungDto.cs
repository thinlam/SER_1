using QLDA.Application.TepDinhKems.DTOs;

namespace QLDA.Application.PheDuyetNoiDungs.DTOs;

public class PheDuyetNoiDungDto {
    public Guid Id { get; set; }
    public Guid VanBanQuyetDinhId { get; set; }
    public Guid DuAnId { get; set; }
    public int? BuocId { get; set; }

    // From VanBanQuyetDinh
    public string? TenDuAn { get; set; }
    public string? So { get; set; }
    public DateTimeOffset? Ngay { get; set; }
    public string? TrichYeu { get; set; }
    public string? LoaiVanBan { get; set; }
    public string? TenLoaiVanBan { get; set; }

    // From PheDuyetNoiDung
    public int? TrangThaiId { get; set; }
    public string? MaTrangThai { get; set; }
    public string? TenTrangThai { get; set; }
    public string? NoiDungPhanHoi { get; set; }
    public bool DaChuyenQLVB { get; set; }
    public string? SoPhatHanh { get; set; }
    public DateTimeOffset? NgayPhatHanh { get; set; }

    // Attachments
    public List<TepDinhKemDto>? DanhSachTepDinhKem { get; set; }
}
