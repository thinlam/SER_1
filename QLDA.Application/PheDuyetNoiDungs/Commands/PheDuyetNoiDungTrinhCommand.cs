using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Application.Providers;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities;

namespace QLDA.Application.PheDuyetNoiDungs.Commands;

/// <summary>
/// Trình nội dung phê duyệt - CB.PCT, CB.PKH-TC theo phòng
/// </summary>
public record PheDuyetNoiDungTrinhCommand(
    Guid VanBanQuyetDinhId,
    Guid DuAnId,
    int? BuocId,
    string? NoiDung = null) : IRequest<int>;

internal class PheDuyetNoiDungTrinhCommandHandler : IRequestHandler<PheDuyetNoiDungTrinhCommand, int> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;
    private readonly IRepository<VanBanQuyetDinh, Guid> _vbqdRepository;
    private readonly IUserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetNoiDungTrinhCommandHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
        _vbqdRepository = serviceProvider.GetRequiredService<IRepository<VanBanQuyetDinh, Guid>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _unitOfWork = _repository.UnitOfWork;
    }

    public async Task<int> Handle(PheDuyetNoiDungTrinhCommand request, CancellationToken cancellationToken) {
        var vbqd = await _vbqdRepository.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.VanBanQuyetDinhId, cancellationToken);
        ManagedException.ThrowIfNull(vbqd, "Không tìm thấy văn bản quyết định");

        // Check not already in PheDuyetNoiDung
        var exists = await _repository.GetQueryableSet()
            .AnyAsync(e => e.VanBanQuyetDinhId == request.VanBanQuyetDinhId && !e.IsDeleted, cancellationToken);
        if (exists) {
            throw new ManagedException("Nội dung này đã được trình phê duyệt");
        }

        var entity = new PheDuyetNoiDung {
            Id = Guid.NewGuid(),
            VanBanQuyetDinhId = request.VanBanQuyetDinhId,
            DuAnId = request.DuAnId,
            BuocId = request.BuocId,
            TrangThai = TrangThaiPheDuyetCodes.NoiDung.ChoXuLy,
            NguoiXuLyId = _userProvider.Info.UserID
        };

        await _repository.AddAsync(entity, cancellationToken);

        var history = new PheDuyetNoiDungHistory {
            Id = Guid.NewGuid(),
            PheDuyetNoiDungId = entity.Id,
            DuAnId = entity.DuAnId,
            NguoiXuLyId = _userProvider.Info.UserID,
            TrangThai = TrangThaiPheDuyetCodes.NoiDung.ChoXuLy,
            NoiDung = request.NoiDung,
            NgayXuLy = DateTimeOffset.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
