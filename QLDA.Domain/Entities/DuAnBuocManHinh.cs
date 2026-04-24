using QLDA.Domain.Entities.DanhMuc;

namespace QLDA.Domain.Entities;

/// <summary>
/// Cân lưu lại id để load màn hình đã chọn
/// </summary>
public class DuAnBuocManHinh : IJunctionEntity<int, int>, IAggregateRoot, IMayHaveStt {
    public int LeftId { get; set; }
    public int RightId { get; set; }
    public int? Stt { get; set; }

    #region Navigation Properties

    public DuAnBuoc? DuAnBuoc { get; set; }
    public DanhMucManHinh? ManHinh { get; set; }

    #endregion
}
