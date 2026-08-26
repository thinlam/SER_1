using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common.Mapping;
using QLDA.Application.DuAns.DTOs;
using BuildingBlocks.CrossCutting.DateTimes;
using QLDA.WebApi.Models.DuAns;

namespace QLDA.Application.DuAns.Queries;

public record DuAnGetDanhSachTreHanExcell(DuAnSearchOverdueDto SearchDto) : AggregateRootPagination, IRequest<List<DuAnTreHanDto>>;

public record DuAnGetDanhSachTreHanExcellHandler(IServiceProvider serviceProvider)
    : IRequestHandler<DuAnGetDanhSachTreHanExcell, List<DuAnTreHanDto>> {
    private readonly IRepository<DuAnBuoc, int> DuAnBuoc = serviceProvider.GetRequiredService<IRepository<DuAnBuoc, int>>();
    private readonly IRepository<DmDonVi, long> DanhMucDonVi =  serviceProvider.GetRequiredService<IRepository<DmDonVi, long>>();
    private readonly IDateTimeProvider _dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();
    public async Task<List<DuAnTreHanDto>> Handle( DuAnGetDanhSachTreHanExcell request,   CancellationToken cancellationToken) {
        var query = DuAnBuoc.GetDanhSachTreHanQueryable(  request.SearchDto);

        if (request.SearchDto.IsChiTiet) {
            return await query .Select(e => new DuAnTreHanDto {
                    DuAnId = e.DuAnId,
                    TenDuAn = e.DuAn!.TenDuAn,
                    DonViPhuTrachChinhId =   e.DuAn.DonViPhuTrachChinhId,
                    TenBuoc = e.TenBuoc,
                    BuocId = e.BuocId,
                    TenDonViPhuTrachChinh = DanhMucDonVi.GetQueryableSet()
                            .Where(dv => dv.Id == e.DuAn!.DonViPhuTrachChinhId)
                            .Select(dv => dv.TenDonVi ?? "Không rõ")
                            .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        // Không chi tiết
        return await query
            .GroupBy(e => new {
                e.BuocId,
                e.TenBuoc,
                e.DuAn!.DonViPhuTrachChinhId
            })
            .Select(g => new DuAnTreHanDto {
                BuocId = g.Key.BuocId,
                TenBuoc = g.Key.TenBuoc,
                DonViPhuTrachChinhId = g.Key.DonViPhuTrachChinhId,
                SoLuong = g.Count(),
                TenDonViPhuTrachChinh = DanhMucDonVi.GetQueryableSet()
                        .Where(dv => dv.Id == g.Key.DonViPhuTrachChinhId)
                        .Select(dv => dv.TenDonVi ?? "Không rõ")
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
    }

}
