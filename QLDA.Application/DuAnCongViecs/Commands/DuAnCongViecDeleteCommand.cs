using Microsoft.EntityFrameworkCore;

namespace QLDA.Application.DuAnCongViecs.Commands;

public record DuAnCongViecDeleteCommand(Guid DuAnId, long CongViecId) : IRequest<bool>;

internal class DuAnCongViecDeleteCommandHandler : IRequestHandler<DuAnCongViecDeleteCommand, bool> {
    private readonly IRepository<DuAn, Guid> DuAn;
    private readonly IUnitOfWork _unitOfWork;

    public DuAnCongViecDeleteCommandHandler(IServiceProvider serviceProvider) {
        DuAn = serviceProvider.GetRequiredService<IRepository<DuAn, Guid>>();
        _unitOfWork = DuAn.UnitOfWork;
    }

    public async Task<bool> Handle(DuAnCongViecDeleteCommand request, CancellationToken cancellationToken = default) {
        var duAnEntity = await DuAn.GetQueryableSet()
            .Include(e => e.DuAnCongViecs)
            .FirstOrDefaultAsync(e => e.Id == request.DuAnId, cancellationToken)
            ?? throw new ManagedException("Dự án không tồn tại");

        var entity = duAnEntity.DuAnCongViecs!
            .FirstOrDefault(e => e.RightId == request.CongViecId && !e.IsDeleted)
            ?? throw new ManagedException("Liên kết không tồn tại");

        entity.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}