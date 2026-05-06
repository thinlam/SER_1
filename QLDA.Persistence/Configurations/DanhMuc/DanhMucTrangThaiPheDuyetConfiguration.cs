using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLDA.Domain.Constants;
using QLDA.Domain.Entities.DanhMuc;

namespace QLDA.Persistence.Configurations.DanhMuc;

public class DanhMucTrangThaiPheDuyetConfiguration : AggregateRootConfiguration<DanhMucTrangThaiPheDuyet> {
    private static readonly DateTimeOffset SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public override void Configure(EntityTypeBuilder<DanhMucTrangThaiPheDuyet> builder) {
        builder.ToTable("DmTrangThaiPheDuyet");
        builder.ConfigureForDanhMuc();

        builder.HasMany(e => e.PheDuyetDuToans)
            .WithOne(e => e.TrangThai)
            .HasForeignKey(e => e.TrangThaiId);

        builder.Property(e => e.Loai).HasMaxLength(50);

        builder.HasData(
            new DanhMucTrangThaiPheDuyet { Id = 1, Ma = "DT", Ten = "Dự thảo", Loai = TrangThaiPheDuyetCodes.Loai.DuToan, Stt = 1, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 2, Ma = "ĐTr", Ten = "Đã trình", Loai = TrangThaiPheDuyetCodes.Loai.DuToan, Stt = 2, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 3, Ma = "ĐD", Ten = "Đã duyệt", Loai = TrangThaiPheDuyetCodes.Loai.DuToan, Stt = 3, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 4, Ma = "TL", Ten = "Trả lại", Loai = TrangThaiPheDuyetCodes.Loai.DuToan, Stt = 4, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 5, Ma = "LEG", Ten = "Migrated", Loai = TrangThaiPheDuyetCodes.Loai.DuToan, Stt = 0, Used = false, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 6, Ma = "CXL", Ten = "Chờ xử lý", Loai = TrangThaiPheDuyetCodes.Loai.NoiDung, Stt = 10, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 7, Ma = "TC", Ten = "Từ chối", Loai = TrangThaiPheDuyetCodes.Loai.NoiDung, Stt = 11, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 8, Ma = "DKS", Ten = "Đã ký số", Loai = TrangThaiPheDuyetCodes.Loai.NoiDung, Stt = 12, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 9, Ma = "DQLVB", Ten = "Đã chuyển QLVB", Loai = TrangThaiPheDuyetCodes.Loai.NoiDung, Stt = 13, Used = true, CreatedAt = SeedCreatedAt },
            new DanhMucTrangThaiPheDuyet { Id = 10, Ma = "DPH", Ten = "Đã phát hành", Loai = TrangThaiPheDuyetCodes.Loai.NoiDung, Stt = 14, Used = true, CreatedAt = SeedCreatedAt }
        );
    }
}
