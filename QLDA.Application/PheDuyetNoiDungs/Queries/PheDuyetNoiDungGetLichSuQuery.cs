using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Application.PheDuyetNoiDungs.DTOs;

namespace QLDA.Application.PheDuyetNoiDungs.Queries;

public record PheDuyetNoiDungGetLichSuQuery(Guid PheDuyetNoiDungId) : IRequest<List<PheDuyetNoiDungLichSuDto>>;

internal class PheDuyetNoiDungGetLichSuQueryHandler : IRequestHandler<PheDuyetNoiDungGetLichSuQuery, List<PheDuyetNoiDungLichSuDto>> {
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _repository;

    public PheDuyetNoiDungGetLichSuQueryHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
    }

    public async Task<List<PheDuyetNoiDungLichSuDto>> Handle(PheDuyetNoiDungGetLichSuQuery request, CancellationToken cancellationToken) {
        var results = await _repository.GetQueryableSet().AsNoTracking()
            .Where(h => h.PheDuyetNoiDungId == request.PheDuyetNoiDungId)
            .Select(h => new PheDuyetNoiDungLichSuDto {
                Id = h.Id,
                NguoiXuLyId = h.NguoiXuLyId,
                TrangThai = h.TrangThai,
                NoiDung = h.NoiDung,
                NgayXuLy = h.NgayXuLy
            })
            .ToListAsync(cancellationToken);

        return results.OrderByDescending(h => h.NgayXuLy).ToList();
    }
}
