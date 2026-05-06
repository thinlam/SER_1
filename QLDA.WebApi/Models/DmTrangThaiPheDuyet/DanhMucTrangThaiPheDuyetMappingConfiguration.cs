namespace QLDA.WebApi.Models.DmTrangThaiPheDuyet;

public static class DanhMucTrangThaiPheDuyetMappingConfiguration {
    public static DanhMucTrangThaiPheDuyetModel ToModel(this DanhMucTrangThaiPheDuyet entity)
        => new() {
            Id = entity.Id,
            Ma = entity.Ma,
            Ten = entity.Ten,
            MoTa = entity.MoTa,
            Stt = entity.Stt,
            Used = entity.Used,
            Loai = entity.Loai
        };

    public static DanhMucTrangThaiPheDuyet ToEntity(this DanhMucTrangThaiPheDuyetModel model)
        => new() {
            Id = model.GetId(),
            Ma = model.Ma,
            Ten = model.Ten,
            MoTa = model.MoTa,
            Stt = model.Stt,
            Used = model.Used,
            Loai = model.Loai
        };
}
