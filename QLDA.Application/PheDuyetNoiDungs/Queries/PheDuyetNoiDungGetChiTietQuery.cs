using Microsoft.EntityFrameworkCore;
using QLDA.Application.Common;
using QLDA.Application.PheDuyetNoiDungs.DTOs;
using QLDA.Application.TepDinhKems.DTOs;

namespace QLDA.Application.PheDuyetNoiDungs.Queries;

public record PheDuyetNoiDungGetChiTietQuery(Guid Id) : IRequest<PheDuyetNoiDungChiTietDto>;

internal class PheDuyetNoiDungGetChiTietQueryHandler : IRequestHandler<PheDuyetNoiDungGetChiTietQuery, PheDuyetNoiDungChiTietDto> {
    private readonly IRepository<PheDuyetNoiDung, Guid> _repository;
    private readonly IRepository<TepDinhKem, Guid> _tepDinhKem;
    private readonly IRepository<PheDuyetNoiDungHistory, Guid> _historyRepository;

    public PheDuyetNoiDungGetChiTietQueryHandler(IServiceProvider serviceProvider) {
        _repository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDung, Guid>>();
        _tepDinhKem = serviceProvider.GetRequiredService<IRepository<TepDinhKem, Guid>>();
        _historyRepository = serviceProvider.GetRequiredService<IRepository<PheDuyetNoiDungHistory, Guid>>();
    }

    public async Task<PheDuyetNoiDungChiTietDto> Handle(PheDuyetNoiDungGetChiTietQuery request, CancellationToken cancellationToken) {
        var entity = await _repository.GetQueryableSet().AsNoTracking()
            .Include(e => e.VanBanQuyetDinh)
            .Include(e => e.DuAn)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        ManagedException.ThrowIfNull(entity, "Không tìm thấy nội dung phê duyệt");

        var lichSu = await _historyRepository.GetQueryableSet().AsNoTracking()
            .Where(h => h.PheDuyetNoiDungId == entity.Id)
            .Select(h => new PheDuyetNoiDungLichSuDto {
                Id = h.Id,
                NguoiXuLyId = h.NguoiXuLyId,
                TrangThai = h.TrangThai,
                NoiDung = h.NoiDung,
                NgayXuLy = h.NgayXuLy
            })
            .ToListAsync(cancellationToken);

        lichSu = lichSu.OrderByDescending(h => h.NgayXuLy).ToList();

        var tepDinhKem = await _tepDinhKem.GetQueryableSet().AsNoTracking()
            .Where(t => t.GroupId == entity.VanBanQuyetDinhId.ToString())
            .Select(t => t.ToDto())
            .ToListAsync(cancellationToken);

        return new PheDuyetNoiDungChiTietDto {
            Id = entity.Id,
            VanBanQuyetDinhId = entity.VanBanQuyetDinhId,
            DuAnId = entity.DuAnId,
            BuocId = entity.BuocId,
            TenDuAn = entity.DuAn?.TenDuAn,
            So = entity.VanBanQuyetDinh?.So,
            Ngay = entity.VanBanQuyetDinh?.Ngay,
            TrichYeu = entity.VanBanQuyetDinh?.TrichYeu,
            LoaiVanBan = entity.VanBanQuyetDinh?.Loai,
            TrangThai = entity.TrangThai,
            NoiDungPhanHoi = entity.NoiDungPhanHoi,
            DaChuyenQLVB = entity.DaChuyenQLVB,
            SoPhatHanh = entity.SoPhatHanh,
            NgayPhatHanh = entity.NgayPhatHanh,
            NguoiKy = entity.VanBanQuyetDinh?.NguoiKy,
            NgayKy = entity.VanBanQuyetDinh?.NgayKy,
            NguoiXuLyId = entity.NguoiXuLyId,
            DanhSachTepDinhKem = tepDinhKem,
            LichSu = lichSu
        };
    }
}
