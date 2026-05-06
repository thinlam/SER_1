using BuildingBlocks.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities;

namespace QLDA.Application.PheDuyetNoiDungs.Commands;

/// <summary>
/// Gửi lại sau khi bị trả lại - CB.PCT, LĐ.PCT
/// </summary>
public record PheDuyetNoiDungGuiLaiCommand(Guid Id, string? NoiDung = null) : IRequest<int>;

internal class PheDuyetNoiDungGuiLaiCommandHandler : IRequestHandler<PheDuyetNoiDungGuiLaiCommand, int> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;
    private readonly IUserProvider _userProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetNoiDungGuiLaiCommandHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
        _userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        _unitOfWork = _repository.UnitOfWork;
    }

    public async Task<int> Handle(PheDuyetNoiDungGuiLaiCommand request, CancellationToken cancellationToken) {
        var entity = await _repository.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity, "Không tìm thấy nội dung phê duyệt");

        if (entity.TrangThai != TrangThaiPheDuyetCodes.NoiDung.TraLai) {
            throw new ManagedException("Chỉ có thể gửi lại khi trạng thái là Trả lại");
        }

        entity.TrangThai = TrangThaiPheDuyetCodes.NoiDung.ChoXuLy;
        entity.NguoiXuLyId = _userProvider.Info.UserID;
        entity.NoiDungPhanHoi = null;

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
