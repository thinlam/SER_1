using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common.Extensions;
using QLDA.Application.Common.Mapping;
using QLDA.Application.PheDuyetNoiDungs.DTOs;
using QLDA.Application.Providers;
using QLDA.Application.TepDinhKems.DTOs;
using QLDA.Domain.Entities;
using QLDA.Domain.Enums;
using PermissionConstants = QLDA.Domain.Constants.PermissionConstants;

namespace QLDA.Application.PheDuyetNoiDungs.Queries;

public record PheDuyetNoiDungGetDanhSachQuery : AggregateRootPagination, IMayHaveGlobalFilter, IRequest<PaginatedList<PheDuyetNoiDungDto>> {
    public Guid? DuAnId { get; set; }
    public int? BuocId { get; set; }
    public string? TrangThai { get; set; }
    public string? LoaiVanBan { get; set; }
    public string? GlobalFilter { get; set; }
}

internal class PheDuyetNoiDungGetDanhSachQueryHandler : IRequestHandler<PheDuyetNoiDungGetDanhSachQuery, PaginatedList<PheDuyetNoiDungDto>> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<TepDinhKem, Guid> _tepDinhKem;
    private readonly IRepository<DuAn, Guid> _duAn;
    private readonly IUserProvider _userProvider;
    private readonly IPolicyProvider _policyProvider;

    public PheDuyetNoiDungGetDanhSachQueryHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _tepDinhKem = serviceProvider.GetRequiredService<IRepository<TepDinhKem, Guid>>();
        _duAn = serviceProvider.GetRequiredService<IRepository<DuAn, Guid>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _policyProvider = serviceProvider.GetRequiredService<IPolicyProvider>();
    }

    public async Task<PaginatedList<PheDuyetNoiDungDto>> Handle(PheDuyetNoiDungGetDanhSachQuery request, CancellationToken cancellationToken) {
        var query = _repository.GetQueryableSet().AsNoTracking()
            .Include(e => e.VanBanQuyetDinh)
            .Include(e => e.DuAn)
            .WhereIf(request.DuAnId.HasValue, e => e.DuAnId == request.DuAnId)
            .WhereIf(request.BuocId.HasValue, e => e.BuocId == request.BuocId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.TrangThai), e => e.TrangThai == request.TrangThai)
            .WhereIf(!string.IsNullOrWhiteSpace(request.LoaiVanBan), e => e.VanBanQuyetDinh != null && e.VanBanQuyetDinh.Loai == request.LoaiVanBan)
            .WhereIf(!string.IsNullOrWhiteSpace(request.GlobalFilter),
                e => (e.VanBanQuyetDinh != null && e.VanBanQuyetDinh.So != null && e.VanBanQuyetDinh.So.Contains(request.GlobalFilter!))
                  || (e.VanBanQuyetDinh != null && e.VanBanQuyetDinh.TrichYeu != null && e.VanBanQuyetDinh.TrichYeu.Contains(request.GlobalFilter!))
                  || (e.DuAn != null && e.DuAn.TenDuAn != null && e.DuAn.TenDuAn.Contains(request.GlobalFilter!)));

        // Apply visibility filter
        query = query.ApplyDuAnChildVisibility(_duAn, _userProvider, _policyProvider, e => e.DuAnId);

        return await query
            .Select(e => new PheDuyetNoiDungDto {
                Id = e.Id,
                VanBanQuyetDinhId = e.VanBanQuyetDinhId,
                DuAnId = e.DuAnId,
                BuocId = e.BuocId,
                TenDuAn = e.DuAn != null ? e.DuAn.TenDuAn : null,
                So = e.VanBanQuyetDinh != null ? e.VanBanQuyetDinh.So : null,
                Ngay = e.VanBanQuyetDinh != null ? e.VanBanQuyetDinh.Ngay : null,
                TrichYeu = e.VanBanQuyetDinh != null ? e.VanBanQuyetDinh.TrichYeu : null,
                LoaiVanBan = e.VanBanQuyetDinh != null ? e.VanBanQuyetDinh.Loai : null,
                TrangThai = e.TrangThai,
                NoiDungPhanHoi = e.NoiDungPhanHoi,
                DaChuyenQLVB = e.DaChuyenQLVB,
                SoPhatHanh = e.SoPhatHanh,
                NgayPhatHanh = e.NgayPhatHanh,
            })
            .PaginatedListAsync(request.Skip(), request.Take(), cancellationToken: cancellationToken);
    }
}
