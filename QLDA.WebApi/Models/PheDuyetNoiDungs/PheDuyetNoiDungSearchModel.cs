namespace QLDA.WebApi.Models.PheDuyetNoiDungs;

public class PheDuyetNoiDungSearchModel {
    public Guid? DuAnId { get; set; }
    public int? BuocId { get; set; }
    public string? TrangThai { get; set; }
    public string? LoaiVanBan { get; set; }
    public string? GlobalFilter { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
