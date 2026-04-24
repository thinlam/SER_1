using Microsoft.EntityFrameworkCore;
using QLDA.Application.DuAnCongViecs.DTOs;

namespace QLDA.Application.DuAnCongViecs.Queries;

public record DuAnCongViecGetChiTietQuery(Guid DuAnId, long CongViecId) : IRequest<DuAnCongViecDto>;

internal class DuAnCongViecGetChiTietQueryHandler : IRequestHandler<DuAnCongViecGetChiTietQuery, DuAnCongViecDto> {
    private readonly IRepository<DuAn, Guid> DuAn;

    public DuAnCongViecGetChiTietQueryHandler(IServiceProvider serviceProvider) {
        DuAn = serviceProvider.GetRequiredService<IRepository<DuAn, Guid>>();
    }

    public async Task<DuAnCongViecDto> Handle(DuAnCongViecGetChiTietQuery request, CancellationToken cancellationToken = default) {
        var duAnEntity = await DuAn.GetQueryableSet()
            .AsNoTracking()
            .Include(e => e.DuAnCongViecs)
            .FirstOrDefaultAsync(e => e.Id == request.DuAnId, cancellationToken)
            ?? throw new ManagedException("Dự án không tồn tại");

        var entity = duAnEntity.DuAnCongViecs!
            .FirstOrDefault(e => e.RightId == request.CongViecId && !e.IsDeleted)
            ?? throw new ManagedException("Liên kết không tồn tại");

        return new DuAnCongViecDto {
            DuAnId = entity.LeftId,
            TenDuAn = duAnEntity.TenDuAn,
            CongViecId = entity.RightId,
            IsDeleted = entity.IsDeleted,
            IsHoanThanh = entity.IsHoanThanh,
            NguoiPhuTrachChinhId = entity.NguoiPhuTrachChinhId,
            NguoiTaoId = entity.NguoiTaoId
        };
    }
}