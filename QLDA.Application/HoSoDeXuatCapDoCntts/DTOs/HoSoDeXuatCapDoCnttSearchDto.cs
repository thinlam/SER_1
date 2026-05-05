namespace QLDA.Application.HoSoDeXuatCapDoCntts.DTOs;

public record HoSoDeXuatCapDoCnttSearchDto : AggregateRootPagination, IMayHaveGlobalFilter {
    public Guid? DuAnId { get; set; }
    public int? BuocId { get; set; }
    public string? GlobalFilter { get; set; }

}