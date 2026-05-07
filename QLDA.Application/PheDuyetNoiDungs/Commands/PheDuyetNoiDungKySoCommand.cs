using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities;
using QLDA.Domain.Entities.DanhMuc;
using QLDARoleConstants = QLDA.Domain.Constants.RoleConstants;

namespace QLDA.Application.PheDuyetNoiDungs.Commands;

/// <summary>
/// Ký số nội dung - BGĐ, phải đã duyệt
/// </summary>
public record PheDuyetNoiDungKySoCommand(Guid Id) : IRequest<int>;

internal class PheDuyetNoiDungKySoCommandHandler : IRequestHandler<PheDuyetNoiDungKySoCommand, int> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;
    private readonly IRepository<DanhMucTrangThaiPheDuyet, int> _statusRepository;
    private readonly IUserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetNoiDungKySoCommandHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
        _statusRepository = serviceProvider.GetRequiredService<IRepository<DanhMucTrangThaiPheDuyet, int>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _unitOfWork = _repository.UnitOfWork;
    }

    public async Task<int> Handle(PheDuyetNoiDungKySoCommand request, CancellationToken cancellationToken) {
        if (!_userProvider.AuthInfo.Roles.Contains(QLDARoleConstants.QLDA_LDDV)) {
            throw new ManagedException("Chỉ Ban Giám đốc có quyền ký số");
        }

        var trangThaiDaDuyet = await _statusRepository.GetQueryableSet(OnlyUsed: true, OnlyNotDeleted: true, OrderByIndex: false)
            .FirstOrDefaultAsync(s => s.Ma == TrangThaiPheDuyetCodes.NoiDung.DaDuyet && s.Loai == TrangThaiPheDuyetCodes.Loai.NoiDung, cancellationToken);
        var trangThaiDaKySo = await _statusRepository.GetQueryableSet(OnlyUsed: true, OnlyNotDeleted: true, OrderByIndex: false)
            .FirstOrDefaultAsync(s => s.Ma == TrangThaiPheDuyetCodes.NoiDung.DaKySo && s.Loai == TrangThaiPheDuyetCodes.Loai.NoiDung, cancellationToken);
        ManagedException.ThrowIfNull(trangThaiDaKySo, "Không tìm thấy trạng thái 'Đã ký số'");

        var entity = await _repository.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity, "Không tìm thấy nội dung phê duyệt");

        if (entity.TrangThaiId != trangThaiDaDuyet?.Id) {
            throw new ManagedException("Chỉ có thể ký số khi trạng thái là Đã duyệt");
        }

        entity.TrangThaiId = trangThaiDaKySo.Id;
        entity.NguoiXuLyId = _userProvider.Info.UserID;

        var history = new PheDuyetNoiDungHistory {
            Id = Guid.NewGuid(),
            PheDuyetNoiDungId = entity.Id,
            DuAnId = entity.DuAnId,
            NguoiXuLyId = _userProvider.Info.UserID,
            TrangThaiId = trangThaiDaKySo.Id,
            NgayXuLy = DateTimeOffset.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
