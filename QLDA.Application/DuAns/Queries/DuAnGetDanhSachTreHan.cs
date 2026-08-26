using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common.Mapping;
using QLDA.Application.DuAns.DTOs;
using BuildingBlocks.CrossCutting.DateTimes;
using QLDA.WebApi.Models.DuAns;

namespace QLDA.Application.DuAns.Queries;

public record DuAnGetDanhSachTreHan(DuAnSearchOverdueDto SearchDto) : AggregateRootPagination, IRequest<PaginatedList<DuAnTreHanChiTietDto>>;

public record DuAnGetDanhSachTreHanHandler(IServiceProvider serviceProvider)
    : IRequestHandler<DuAnGetDanhSachTreHan, PaginatedList<DuAnTreHanChiTietDto>> {
    private readonly IRepository<DuAnBuoc, int> DuAnBuoc =
        serviceProvider.GetRequiredService<IRepository<DuAnBuoc, int>>();

    private readonly IRepository<DmDonVi, long> DanhMucDonVi =
        serviceProvider.GetRequiredService<IRepository<DmDonVi, long>>();

    private readonly IDateTimeProvider _dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

    public async Task<PaginatedList<DuAnTreHanChiTietDto>> Handle(DuAnGetDanhSachTreHan request,
        CancellationToken cancellationToken) {
        // var now = _dateTimeProvider.OffsetUtcNow;
        var query = DuAnBuoc.GetDanhSachTreHanQueryable(
           request.SearchDto);
        return await query
           .Select(e => new DuAnTreHanChiTietDto {
               DuAnId = e.DuAnId,
               TenDuAn = e.DuAn!.TenDuAn,
               DonViPhuTrachChinhId =  e.DuAn.DonViPhuTrachChinhId,
               NgayDuKienBatDau =      e.NgayDuKienBatDau.ToDateOnlyVn(),
               NgayDuKienKetThuc =  e.NgayDuKienKetThuc.ToDateOnlyVn(),
               NgayThucTeBatDau = e.NgayThucTeBatDau.ToDateOnlyVn(),
               NgayThucTeKetThuc =   e.NgayThucTeKetThuc.ToDateOnlyVn(),
               TenBuoc = e.TenBuoc,
               BuocId = e.BuocId,
               TenDonViPhuTrachChinh =  DanhMucDonVi.GetQueryableSet()
                       .Where(dv => dv.Id ==   e.DuAn!.DonViPhuTrachChinhId)
                       .Select(dv => dv.TenDonVi ?? "Không rõ").FirstOrDefault()
           })
        .PaginatedListAsync(request.SearchDto.Skip(), request.SearchDto.Take(), cancellationToken);
    }
}
