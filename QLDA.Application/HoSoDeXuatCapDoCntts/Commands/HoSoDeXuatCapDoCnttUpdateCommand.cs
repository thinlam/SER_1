using System.Data;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.HoSoDeXuatCapDoCntts.DTOs;

namespace QLDA.Application.HoSoDeXuatCapDoCntts.Commands;

public record HoSoDeXuatCapDoCnttUpdateCommand(HoSoDeXuatCapDoCnttUpdateModel Model) 
    : IRequest<HoSoDeXuatCapDoCntt>;

internal class HoSoDeXuatCapDoCnttUpdateCommandHandler : IRequestHandler<HoSoDeXuatCapDoCnttUpdateCommand, HoSoDeXuatCapDoCntt> {
    private readonly IRepository<HoSoDeXuatCapDoCntt, Guid> HoSoDeXuatCapDoCntt;
    private readonly IUnitOfWork _unitOfWork;

    public HoSoDeXuatCapDoCnttUpdateCommandHandler(IServiceProvider serviceProvider) {
        HoSoDeXuatCapDoCntt = serviceProvider.GetRequiredService<IRepository<HoSoDeXuatCapDoCntt, Guid>>();
        _unitOfWork = HoSoDeXuatCapDoCntt.UnitOfWork;
    }

    public async Task<HoSoDeXuatCapDoCntt> Handle(HoSoDeXuatCapDoCnttUpdateCommand request, CancellationToken cancellationToken = default) {
        var entity = await HoSoDeXuatCapDoCntt.GetQueryableSet()
            .FirstOrDefaultAsync(e => e.Id == request.Model.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity);

        entity.Update(request.Model);

        using var tx = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        await HoSoDeXuatCapDoCntt.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        return entity;
    }
}