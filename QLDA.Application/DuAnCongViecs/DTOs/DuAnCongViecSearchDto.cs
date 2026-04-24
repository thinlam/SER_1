using QLDA.Application.Common.Interfaces;

namespace QLDA.Application.DuAnCongViecs.DTOs;

public record DuAnCongViecSearchDto : CommonSearchDto {
    public long? CongViecId { get; set; }
    public bool? IsHoanThanh { get; set; }
}