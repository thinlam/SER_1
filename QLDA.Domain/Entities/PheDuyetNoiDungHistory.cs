namespace QLDA.Domain.Entities;

/// <summary>
/// Lịch sử phê duyệt nội dung trình duyệt
/// </summary>
public class PheDuyetNoiDungHistory : Entity<Guid>, IAggregateRoot {
    public Guid PheDuyetNoiDungId { get; set; }
    public Guid DuAnId { get; set; }

    /// <summary>
    /// USER_MASTER.UserPortalId
    /// </summary>
    public long? NguoiXuLyId { get; set; }

    public string TrangThai { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public DateTimeOffset NgayXuLy { get; set; }

    #region Navigation Properties

    public PheDuyetNoiDung? PheDuyetNoiDung { get; set; }
    public DuAn? DuAn { get; set; }

    #endregion
}
