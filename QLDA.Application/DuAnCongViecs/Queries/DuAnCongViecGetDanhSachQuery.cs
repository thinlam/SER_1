using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common.Mapping;
using QLDA.Application.DuAnCongViecs.DTOs;

namespace QLDA.Application.DuAnCongViecs.Queries;

public record DuAnCongViecGetDanhSachQuery(DuAnCongViecSearchDto SearchDto) : IRequest<PaginatedList<DuAnCongViecDto>>;

internal class DuAnCongViecGetDanhSachQueryHandler : IRequestHandler<DuAnCongViecGetDanhSachQuery, PaginatedList<DuAnCongViecDto>> {
    private readonly IRepository<DuAn, Guid> DuAn;

    public DuAnCongViecGetDanhSachQueryHandler(IServiceProvider serviceProvider) {
        DuAn = serviceProvider.GetRequiredService<IRepository<DuAn, Guid>>();
    }

    public async Task<PaginatedList<DuAnCongViecDto>> Handle(DuAnCongViecGetDanhSachQuery request, CancellationToken cancellationToken = default) {
        var queryable = DuAn.GetQueryableSet()
            .AsNoTracking()
            .Include(e => e.DuAnCongViecs)
            .Where(e => e.DuAnCongViecs!.Any(x => !x.IsDeleted))
            .WhereIf(request.SearchDto.DuAnId.HasValue, e => e.Id == request.SearchDto.DuAnId)
            .SelectMany(e => e.DuAnCongViecs!.Where(x => !x.IsDeleted), (duAn, link) => new { DuAn = duAn, Link = link });

        if (request.SearchDto.CongViecId.HasValue) {
            queryable = queryable.Where(x => x.Link.RightId == request.SearchDto.CongViecId);
        }
        if (request.SearchDto.IsHoanThanh.HasValue) {
            queryable = queryable.Where(x => x.Link.IsHoanThanh == request.SearchDto.IsHoanThanh);
        }

        return await queryable
            .Select(x => new DuAnCongViecDto {
                DuAnId = x.Link.LeftId,
                TenDuAn = x.DuAn.TenDuAn,
                CongViecId = x.Link.RightId,
                IsDeleted = x.Link.IsDeleted,
                IsHoanThanh = x.Link.IsHoanThanh,
                NguoiPhuTrachChinhId = x.Link.NguoiPhuTrachChinhId,
                NguoiTaoId = x.Link.NguoiTaoId
            })
            .PaginatedListAsync(request.SearchDto.Skip(), request.SearchDto.Take(), cancellationToken: cancellationToken);
    }
}