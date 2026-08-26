using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common.Mapping;
using QLDA.Application.TepDinhKems.DTOs;
using QLDA.Application.BaoCaoTienDos.DTOs;
using QLDA.Application.Common.Interfaces;

namespace QLDA.Application.BaoCaoTienDos.Queries;

public record BaoCaoTienDoGetDanhSachQuery : AggregateRootPagination, IMayHaveGlobalFilter, IFromDateToDate, IRequest<PaginatedList<BaoCaoTienDoDto>> {
    public int? BuocId { get; set; }
    public Guid? DuAnId { get; set; }
    public bool IsNoTracking { get; set; }
    public string? GlobalFilter { get; set; }

    public string? NoiDung { get; set; }
    public DateOnly? TuNgay { get; set; }
    public DateOnly? DenNgay { get; set; }
    /// <summary>
    /// Loại dự án theo năm - tài chính
    /// </summary>
    /// <remarks>PMIS #9609</remarks>
    public int? LoaiDuAnTheoNamId { get; set; }
}

internal class
    BaoCaoTienDoGetDanhSachQueryHandler(IServiceProvider ServiceProvider)
    : IRequestHandler<BaoCaoTienDoGetDanhSachQuery,
        PaginatedList<BaoCaoTienDoDto>> {
    private readonly IRepository<BaoCaoTienDo, Guid> BaoCaoTienDo =
        ServiceProvider.GetRequiredService<IRepository<BaoCaoTienDo, Guid>>();

    private readonly IRepository<Attachment, Guid> TepDinhKem =
        ServiceProvider.GetRequiredService<IRepository<Attachment, Guid>>();

    private readonly IRepository<UserMaster, long> _userMaster =
        ServiceProvider.GetRequiredService<IRepository<UserMaster, long>>();

    private readonly IUserProvider User = ServiceProvider.GetRequiredService<IUserProvider>();

    public async Task<PaginatedList<BaoCaoTienDoDto>> Handle(BaoCaoTienDoGetDanhSachQuery request,
        CancellationToken cancellationToken = default) {
        bool dieuKienThayTatCa = false;
        var userMasterQuery = _userMaster.GetQueryableSet().AsNoTracking();

        var queryable = BaoCaoTienDo.GetQueryableSet().AsNoTracking()
            .Where(e => !e.DuAn!.IsDeleted)
            .WhereIf(User.Id > 0 && !dieuKienThayTatCa, e => e.CreatedBy == User.Id.ToString(), e => dieuKienThayTatCa)
            .WhereIf(request.DuAnId != null, e => e.DuAnId == request.DuAnId)
            .WhereIf(request.LoaiDuAnTheoNamId > 0, e => e.DuAn!.LoaiDuAnTheoNamId == request.LoaiDuAnTheoNamId)
            .WhereIf(request.NoiDung.IsNotNullOrWhitespace(),
                e => e.NoiDung!.ToLower().Contains(request.NoiDung!.ToLower()))
            .WhereIf(request.BuocId > 0, e => e.BuocId == request.BuocId)
            .WhereIf(request.TuNgay.HasValue, e => e.Ngay.HasValue && e.Ngay.Value >= request.TuNgay!.Value.ToStartOfDayUtc())
            .WhereIf(request.DenNgay.HasValue, e => e.Ngay.HasValue && e.Ngay.Value <= request.DenNgay!.Value.ToEndOfDayUtc())
            .WhereGlobalFilter(
                request,
                e => e.NoiDung
            );

        return await queryable
            .Select(e => new BaoCaoTienDoDto() {
                Id = e.Id,
                DuAnId = e.DuAnId,
                TenDuAn = e.DuAn != null ? e.DuAn.TenDuAn : null,
                BuocId = e.BuocId,
                TenBuoc = e.DuAnBuoc != null ? e.DuAnBuoc.TenBuoc : null,
                NoiDung = e.NoiDung,
                Ngay = e.Ngay,
                NguoiBaoCaoId = long.Parse(e.CreatedBy),
                TenNguoiBaoCao = userMasterQuery
                    .Where(u => u.UserPortalId.ToString() == e.CreatedBy || u.Id.ToString() == e.CreatedBy)
                    .Select(u => u.HoTen)
                    .FirstOrDefault(),
                DanhSachTepDinhKem = TepDinhKem.GetQueryableSet()
                    .Where(i => i.GroupId == e.Id.ToString())
                    .Select(i => i.ToDto()).ToList(),
            })
            .PaginatedListAsync(request.Skip(), request.Take(), cancellationToken: cancellationToken);
    }
}