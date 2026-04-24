using QLDA.Domain.Entities;

namespace QLDA.Application.KeHoachVons.DTOs;

public static class KeHoachVonMapping {

    public static KeHoachVon ToEntity(this KeHoachVonInsertModel model, Guid duAnId) {
        return new KeHoachVon {
            Id = GuidExtensions.GetSequentialGuidId(),
            DuAnId = duAnId,
            NguonVonId = model.NguonVonId,
            Nam = model.Nam,
            SoVon = model.SoVon,
            SoVonDieuChinh = model.SoVonDieuChinh,
            SoQuyetDinh = model.SoQuyetDinh,
            NgayKy = model.NgayKy,
            GhiChu = model.GhiChu
        };
    }

    public static KeHoachVon ToEntity(this KeHoachVonUpdateModel model, Guid duAnId) {
        return new KeHoachVon {
            Id = model.Id.GetId(),
            DuAnId = duAnId,
            NguonVonId = model.NguonVonId,
            Nam = model.Nam,
            SoVon = model.SoVon,
            SoVonDieuChinh = model.SoVonDieuChinh,
            SoQuyetDinh = model.SoQuyetDinh,
            NgayKy = model.NgayKy,
            GhiChu = model.GhiChu
        };
    }

    public static KeHoachVonDto ToDto(this KeHoachVon entity) {
        return new KeHoachVonDto {
            Id = entity.Id,
            DuAnId = entity.DuAnId,
            NguonVonId = entity.NguonVonId,
            Nam = entity.Nam,
            SoVon = entity.SoVon,
            SoVonDieuChinh = entity.SoVonDieuChinh,
            SoQuyetDinh = entity.SoQuyetDinh,
            NgayKy = entity.NgayKy,
            GhiChu = entity.GhiChu
        };
    }
}