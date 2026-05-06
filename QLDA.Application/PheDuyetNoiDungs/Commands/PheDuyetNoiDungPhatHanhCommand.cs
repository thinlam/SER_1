using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities;
using QLDARoleConstants = QLDA.Domain.Constants.RoleConstants;

namespace QLDA.Application.PheDuyetNoiDungs.Commands;

/// <summary>
/// Phát hành - P.HC-TH, phải đã chuyển QLVB
/// </summary>
public record PheDuyetNoiDungPhatHanhCommand(
    Guid Id,
    string? SoPhatHanh = null,
    DateTimeOffset? NgayPhatHanh = null) : IRequest<int>;

internal class PheDuyetNoiDungPhatHanhCommandHandler : IRequestHandler<PheDuyetNoiDungPhatHanhCommand, int> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;
    private readonly IUserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetNoiDungPhatHanhCommandHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _unitOfWork = _repository.UnitOfWork;
    }

    public async Task<int> Handle(PheDuyetNoiDungPhatHanhCommand request, CancellationToken cancellationToken) {
        if (!_userProvider.AuthInfo.Roles.Contains(QLDARoleConstants.QLDA_HC_TH)) {
            throw new ManagedException("Chỉ P.HC-Tổng hợp có quyền phát hành");
        }

        var entity = await _repository.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity, "Không tìm thấy nội dung phê duyệt");

        if (entity.TrangThai != TrangThaiPheDuyetCodes.NoiDung.DaChuyenQLVB) {
            throw new ManagedException("Chỉ có thể phát hành khi trạng thái là Đã chuyển QLVB");
        }

        entity.TrangThai = TrangThaiPheDuyetCodes.NoiDung.DaPhatHanh;
        entity.NguoiXuLyId = _userProvider.Info.UserID;
        entity.SoPhatHanh = request.SoPhatHanh;
        entity.NgayPhatHanh = request.NgayPhatHanh ?? DateTimeOffset.UtcNow;

        var history = new PheDuyetNoiDungHistory {
            Id = Guid.NewGuid(),
            PheDuyetNoiDungId = entity.Id,
            DuAnId = entity.DuAnId,
            NguoiXuLyId = _userProvider.Info.UserID,
            TrangThai = TrangThaiPheDuyetCodes.NoiDung.DaPhatHanh,
            NgayXuLy = DateTimeOffset.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
