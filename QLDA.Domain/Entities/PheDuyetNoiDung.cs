using QLDA.Domain.Constants;

namespace QLDA.Domain.Entities;

/// <summary>
/// Phê duyệt nội dung trình duyệt - approval tracking overlay on VanBanQuyetDinh
/// </summary>
public class PheDuyetNoiDung : Entity<Guid>, IAggregateRoot {
    public Guid VanBanQuyetDinhId { get; set; }
    public Guid DuAnId { get; set; }
    public int? BuocId { get; set; }
    public string TrangThai { get; set; } = TrangThaiPheDuyetCodes.NoiDung.ChoXuLy;

    /// <summary>
    /// USER_MASTER.UserPortalId
    /// </summary>
    public long? NguoiXuLyId { get; set; }

    public string? NoiDungPhanHoi { get; set; }
    public string? SoPhatHanh { get; set; }
    public DateTimeOffset? NgayPhatHanh { get; set; }
    public bool DaChuyenQLVB { get; set; }

    #region Navigation Properties

    public VanBanQuyetDinh? VanBanQuyetDinh { get; set; }
    public DuAn? DuAn { get; set; }
    public DuAnBuoc? DuAnBuoc { get; set; }
    public ICollection<PheDuyetNoiDungHistory>? Histories { get; set; } = [];

    #endregion
}
