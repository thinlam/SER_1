using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities;
using QLDARoleConstants = QLDA.Domain.Constants.RoleConstants;

namespace QLDA.Application.PheDuyetNoiDungs.Commands;

/// <summary>
/// Chuyển QLVB - BGĐ, phải đã duyệt hoặc đã ký số
/// </summary>
public record PheDuyetNoiDungChuyenQLVBCommand(Guid Id) : IRequest<int>;

internal class PheDuyetNoiDungChuyenQLVBCommandHandler : IRequestHandler<PheDuyetNoiDungChuyenQLVBCommand, int> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;
    private readonly IUserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetNoiDungChuyenQLVBCommandHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _unitOfWork = _repository.UnitOfWork;
    }

    public async Task<int> Handle(PheDuyetNoiDungChuyenQLVBCommand request, CancellationToken cancellationToken) {
        if (!_userProvider.AuthInfo.Roles.Contains(QLDARoleConstants.QLDA_LDDV)) {
            throw new ManagedException("Chỉ Ban Giám đốc có quyền chuyển QLVB");
        }

        var entity = await _repository.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity, "Không tìm thấy nội dung phê duyệt");

        if (entity.TrangThai != TrangThaiPheDuyetCodes.NoiDung.DaDuyet &&
            entity.TrangThai != TrangThaiPheDuyetCodes.NoiDung.DaKySo) {
            throw new ManagedException("Chỉ có thể chuyển QLVB khi trạng thái là Đã duyệt hoặc Đã ký số");
        }

        entity.TrangThai = TrangThaiPheDuyetCodes.NoiDung.DaChuyenQLVB;
        entity.DaChuyenQLVB = true;
        entity.NguoiXuLyId = _userProvider.Info.UserID;

        var history = new PheDuyetNoiDungHistory {
            Id = Guid.NewGuid(),
            PheDuyetNoiDungId = entity.Id,
            DuAnId = entity.DuAnId,
            NguoiXuLyId = _userProvider.Info.UserID,
            TrangThai = TrangThaiPheDuyetCodes.NoiDung.DaChuyenQLVB,
            NgayXuLy = DateTimeOffset.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
