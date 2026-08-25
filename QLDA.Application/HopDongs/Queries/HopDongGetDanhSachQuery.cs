using QLDA.Application.Common.Mapping;
using QLDA.Application.TepDinhKems.DTOs;
using QLDA.Application.HopDongs.DTOs;
using QLDA.Application.Authorization;

namespace QLDA.Application.HopDongs.Queries;

public record HopDongGetDanhSachQuery(HopDongSearchDto SearchDto) : AggregateRootPagination, IRequest<PaginatedList<HopDongDto>>
{
    public bool IsNoTracking { get; set; }
}

internal class
    HopDongGetDanhSachQueryHandler : IRequestHandler<HopDongGetDanhSachQuery,
    PaginatedList<HopDongDto>>
{
    private readonly IRepository<HopDong, Guid> HopDong;
    private readonly IRepository<Attachment, Guid> TepDinhKem;
    private readonly IRepository<DuAnBuoc, int> _duAnBuocRepo;
    private readonly IBuocAuthorizationProvider _buocAuth;
    private readonly IAuthorizationManager _authManager;
    private readonly IAuthorizationContext _authContext;

    public HopDongGetDanhSachQueryHandler(IServiceProvider serviceProvider)
    {
        HopDong = serviceProvider.GetRequiredService<IRepository<HopDong, Guid>>();
        TepDinhKem = serviceProvider.GetRequiredService<IRepository<Attachment, Guid>>();
        _duAnBuocRepo = serviceProvider.GetRequiredService<IRepository<DuAnBuoc, int>>();
        _buocAuth = serviceProvider.GetRequiredService<IBuocAuthorizationProvider>();
        _authManager = serviceProvider.GetRequiredService<IAuthorizationManager>();
        _authContext = serviceProvider.GetRequiredService<IAuthorizationContext>();
    }

    public async Task<PaginatedList<HopDongDto>> Handle(HopDongGetDanhSachQuery request,
        CancellationToken cancellationToken = default)
    {
        var queryable = HopDong.GetHopDongQueryable(
                                _duAnBuocRepo,
                                _buocAuth,
                                _authManager,
                                _authContext,
                                request.SearchDto);

        return await queryable
            .Select(e => new HopDongDto()
            {
                Id = e.Id,
                DuAnId = e.DuAnId,
                BuocId = e.BuocId,
                GoiThauId = e.GoiThauId,
                Ten = e.Ten,
                SoHopDong = e.SoHopDong,
                NoiDung = e.NoiDung,
                NgayKy = e.NgayKy,
                GiaTri = e.GiaTri,
                NgayHieuLuc = e.NgayHieuLuc.ToDateOnlyVn(),
                NgayDuKienKetThucHopDong = e.NgayDuKienKetThucHopDong.ToDateOnlyVn(),
                NgayDuKienKetThucGoiThau = e.NgayDuKienKetThucGoiThau.ToDateOnlyVn(),
                LoaiHopDongId = e.LoaiHopDongId,
                DonViThucHienId = e.DonViThucHienId,
                IsBienBan = e.IsBienBan,
                DanhSachTepDinhKem = TepDinhKem.GetQueryableSet()
                    .Where(i => i.GroupId == e.Id.ToString())
                    .Select(i => i.ToDto()).ToList(),

                ThanhToanIds = e.NghiemThus == null ? null : e.NghiemThus!.Where(nt => !nt.IsDeleted && nt.ThanhToan != null && !nt.ThanhToan.IsDeleted).Select(i => i.ThanhToan!.Id).ToList(),
                TamUngId = e.TamUng == null ? null : e.TamUng.IsDeleted ? null : e.TamUng.Id,
                SoPhieuChi = e.TamUng == null ? null : e.TamUng.IsDeleted ? null : e.TamUng.SoPhieuChi,
            })
            .PaginatedListAsync(request.SearchDto.Skip(), request.SearchDto.Take(), cancellationToken: cancellationToken);
    }
}
