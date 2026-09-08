using Microsoft.EntityFrameworkCore;
using QLDA.Application.Authorization;

namespace QLDA.Application.Dashboard.Queries;

/// <summary>
/// Query chi tiết giải ngân theo năm và nguồn vốn
/// </summary>
public record DashboardGetChiTietGiaiNganQuery(int Nam, int? NguonVonId = null) : IRequest<List<DashboardChiTietGiaiNganDto>>;

internal class DashboardGetChiTietGiaiNganQueryHandler(IServiceProvider serviceProvider)
    : IRequestHandler<DashboardGetChiTietGiaiNganQuery, List<DashboardChiTietGiaiNganDto>> {

    private readonly IDapperRepository _dapper = serviceProvider.GetRequiredService<IDapperRepository>();

    private readonly IRepository<ThanhToan, Guid> _thanhToan = serviceProvider.GetRequiredService<IRepository<ThanhToan, Guid>>();
    private readonly IAuthorizationManager _authManager = serviceProvider.GetRequiredService<IAuthorizationManager>();

    public async Task<List<DashboardChiTietGiaiNganDto>> Handle(
        DashboardGetChiTietGiaiNganQuery request, CancellationToken cancellationToken) {
     

        var queryable = _authManager.FilterVisible(_thanhToan.GetQueryableSet(), AuthorizationResourceKeys.DuAn)
            .Include(e => e.DuAn).ThenInclude(x => x!.DuAnNguonVons)
              .Include(e => e.NghiemThu).ThenInclude(x => x!.HopDong)
                    .Where(e => !e.DuAn!.IsDeleted).Where(e => !e!.IsDeleted)
                    .WhereIf(request.NguonVonId > 0, e => e.DuAn!.DuAnNguonVons!.Select(i => i.RightId).Contains(request.NguonVonId ?? 0))
                    ;
        var result = await queryable
       .Select(e => new DashboardChiTietGiaiNganDto {
           TenDuAn = e.DuAn!.TenDuAn,
           GiaTriGiaiNgan = e.GiaTri,
           GiaTriHopDong = e.NghiemThu!.HopDong!.GiaTri,
           Ngay = e.NgayHoaDon,
           TrangThaiGiaiNgan = e.GiaTri > 0 ? "Đã giải ngân" : "Chưa giải ngân"
       })
       .ToListAsync();

        return result;

        /*
        var sql = """
            SELECT da.TenDuAn,
                tt.GiaTri AS GiaTriGiaiNgan,
                hd.GiaTri AS GiaTriHopDong,
                tt.NgayHoaDon AS Ngay,
                CASE WHEN tt.GiaTri > 0 THEN N'Đã giải ngân' ELSE N'Chưa giải ngân' END AS TrangThaiGiaiNgan
            FROM dbo.ThanhToan tt
            JOIN dbo.NghiemThu nt ON nt.Id = tt.NghiemThuId
            JOIN dbo.HopDong hd ON hd.Id = nt.HopDongId
            JOIN dbo.GoiThau gt ON gt.Id = hd.GoiThauId
            JOIN dbo.DuAn da ON da.Id = gt.DuAnId
            WHERE tt.IsDeleted = 0 AND hd.IsDeleted = 0
            AND YEAR(tt.NgayHoaDon) = @Nam
            """;

        if (request.NguonVonId.HasValue) {
            sql += " AND gt.NguonVonId = @NguonVonId";
        }

        var result = await _dapper.QueryAsync<DashboardChiTietGiaiNganDto>(sql, new { request.Nam, request.NguonVonId });
        return [.. result];
*/
    }
}
