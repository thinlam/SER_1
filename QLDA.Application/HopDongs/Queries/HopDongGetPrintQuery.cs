using Microsoft.EntityFrameworkCore;
using QLDA.Application.Authorization;
using QLDA.Application.Common.Mapping;
using QLDA.Application.HopDongs.DTOs;
using QLDA.Application.TepDinhKems.DTOs;

namespace QLDA.Application.HopDongs.Queries;

public record HopDongGetPrintQuery(HopDongSearchDto SearchDto) : AggregateRootPagination, IRequest<List<HopDongPrintResultDto>>
{
    public bool IsNoTracking { get; set; }
}

internal class HopDongGetPrintQueryHandler : IRequestHandler<HopDongGetPrintQuery, List<HopDongPrintResultDto>>
{
    private readonly IRepository<HopDong, Guid> HopDong;
    private readonly IRepository<DuAnBuoc, int> _duAnBuocRepo;
    private readonly IBuocAuthorizationProvider _buocAuth;
    private readonly IAuthorizationManager _authManager;
    private readonly IAuthorizationContext _authContext;

    public HopDongGetPrintQueryHandler(IServiceProvider serviceProvider)
    {
        HopDong = serviceProvider.GetRequiredService<IRepository<HopDong, Guid>>();
        _duAnBuocRepo = serviceProvider.GetRequiredService<IRepository<DuAnBuoc, int>>();
        _buocAuth = serviceProvider.GetRequiredService<IBuocAuthorizationProvider>();
        _authManager = serviceProvider.GetRequiredService<IAuthorizationManager>();
        _authContext = serviceProvider.GetRequiredService<IAuthorizationContext>();
    }

    public async Task<List<HopDongPrintResultDto>> Handle(HopDongGetPrintQuery request,
        CancellationToken cancellationToken = default)
    {
        var queryable = HopDong.GetHopDongQueryable(
                                _duAnBuocRepo,
                                _buocAuth,
                                _authManager,
                                _authContext,
                                request.SearchDto);

        return await queryable
            .Select(e => new HopDongPrintResultDto()
            {
              
                ten = e.Ten,
                soHopDong = e.SoHopDong,
                noiDung = e.NoiDung,
                ngayHopDong = e.NgayKy.HasValue? e.NgayKy.Value.ToDateOnlyVn().ToString("dd/MM/yyyy") : string.Empty,
                ngayHieuLuc = e.NgayHieuLuc.HasValue? e.NgayHieuLuc.Value.ToDateOnlyVn().ToString("dd/MM/yyyy") : string.Empty,
               ngayKetThuc = e.NgayDuKienKetThucHopDong.HasValue? e.NgayDuKienKetThucHopDong.Value.ToDateOnlyVn().ToString("dd/MM/yyyy") : string.Empty,
                giaTri = e.GiaTri,
                loaiHopDongId = e.LoaiHopDong == null ? string.Empty : e.LoaiHopDong.Ten ?? string.Empty,
                TenDuAn =  e.DuAn!.TenDuAn,
                TenBuoc = e.DuAnBuoc == null ? string.Empty : e.DuAn.TenDuAn,
                donViThucHienId = e.DonViThucHien == null ? string.Empty : e.DonViThucHien.Ten,
            }).ToListAsync(cancellationToken);
    }
}
