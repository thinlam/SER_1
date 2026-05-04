using System.Data;
using Microsoft.EntityFrameworkCore;
using QLDA.Application.HoSoDeXuatCapDoCntts.DTOs;

namespace QLDA.Application.HoSoDeXuatCapDoCntts.Commands;

public record HoSoDeXuatCapDoCnttInsertCommand(HoSoDeXuatCapDoCnttInsertDto Dto) 
    : IRequest<HoSoDeXuatCapDoCntt>;

internal class HoSoDeXuatCapDoCnttInsertCommandHandler : IRequestHandler<HoSoDeXuatCapDoCnttInsertCommand, HoSoDeXuatCapDoCntt> {
    private readonly IRepository<HoSoDeXuatCapDoCntt, Guid> HoSoDeXuatCapDoCntt;
    private readonly IUnitOfWork _unitOfWork;

    public HoSoDeXuatCapDoCnttInsertCommandHandler(IServiceProvider serviceProvider) {
        HoSoDeXuatCapDoCntt = serviceProvider.GetRequiredService<IRepository<HoSoDeXuatCapDoCntt, Guid>>();
        _unitOfWork = HoSoDeXuatCapDoCntt.UnitOfWork;
    }

    public async Task<HoSoDeXuatCapDoCntt> Handle(HoSoDeXuatCapDoCnttInsertCommand request, CancellationToken cancellationToken = default) {
        var entity = request.Dto.ToEntity();

        using var tx = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        await HoSoDeXuatCapDoCntt.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        return entity;
    }
}