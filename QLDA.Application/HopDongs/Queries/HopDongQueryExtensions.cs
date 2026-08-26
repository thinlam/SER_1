using QLDA.Application.Authorization;
using QLDA.Application.HopDongs.DTOs;
namespace QLDA.Application.HopDongs.Queries;

public static class HopDongQueryExtensions {
    public static IQueryable<HopDong> GetHopDongQueryable(
        this IRepository<HopDong, Guid> hopDongRepo,  IRepository<DuAnBuoc, int> duAnBuocRepo,
        IBuocAuthorizationProvider buocAuth,  IAuthorizationManager authManager,
        IAuthorizationContext authContext,  HopDongSearchDto searchDto)
     {
            return buocAuth
                .FilterVisibleChildEntities(authManager.FilterVisible( hopDongRepo.GetQueryableSet(),
                        AuthorizationResourceKeys.DuAn), duAnBuocRepo, authContext, e => e.BuocId)
                .Where(e => !e.DuAn!.IsDeleted)
                .Where(e => !e.GoiThau!.IsDeleted)
                .WhereIf( searchDto.IsBienBan.HasValue,   e => e.IsBienBan == searchDto.IsBienBan)
                .WhereIf( searchDto.DuAnId != null,   e => e.DuAnId == searchDto.DuAnId)
                .WhereIf( searchDto.LoaiDuAnTheoNamId > 0, e => e.DuAn!.LoaiDuAnTheoNamId == searchDto.LoaiDuAnTheoNamId)
                .WhereIf( searchDto.DonViThucHienId != null,  e => e.DonViThucHienId == searchDto.DonViThucHienId)
                .WhereIf( searchDto.TamUngId != null,  e => e.TamUng!.Id == searchDto.TamUngId)
                .WhereIf( searchDto.GoiThauId != null, e => e.GoiThauId == searchDto.GoiThauId)
                .WhereIf( searchDto.KeHoachLuaChonNhaThauId != null,
                    e => e.GoiThau!.KeHoachLuaChonNhaThauId == searchDto.KeHoachLuaChonNhaThauId)
                .WhereIf( searchDto.BuocId > 0,  e => e.BuocId == searchDto.BuocId)
                .WhereIf( searchDto.Ten.IsNotNullOrWhitespace(), e => e.Ten!.ToLower().Contains(searchDto.Ten!.ToLower()))
                .WhereIf( searchDto.SoHopDong.IsNotNullOrWhitespace(),
                    e => e.SoHopDong!.ToLower().Contains(searchDto.SoHopDong!.ToLower()))
                .WhereIf( searchDto.NoiDung.IsNotNullOrWhitespace(),
                    e => e.NoiDung!.ToLower().Contains(searchDto.NoiDung!.ToLower()))
                .WhereIf( searchDto.LoaiHopDongId > 0, e => e.LoaiHopDongId == searchDto.LoaiHopDongId)
                .WhereGlobalFilter(
                    searchDto,
                    e => e.Ten,
                    e => e.NoiDung,
                    e => e.SoHopDong,
                    e => e.GoiThau!.Ten,
                    e => e.DonViThucHien!.Ten,
                    e => e.LoaiHopDong!.Ten);
    }
}
