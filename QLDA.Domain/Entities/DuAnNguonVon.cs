using QLDA.Domain.Entities.DanhMuc;

namespace QLDA.Domain.Entities;

public class DuAnNguonVon : IJunctionEntity<Guid, int>, IAggregateRoot {
    public Guid LeftId { get; set; }
    public int RightId { get; set; }

    #region Navigation Properties

    public DuAn? DuAn { get; set; }
    public DanhMucNguonVon? NguonVon { get; set; }

    #endregion
}
