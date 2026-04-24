using Microsoft.EntityFrameworkCore;
using QLDA.Domain.Entities.DanhMuc;
using QLDA.Persistence;

namespace QLDA.Tests.Fakers;

/// <summary>
/// Seeds catalog/master data for testing.
/// DanhMucTrangThaiDuAn is auto-seeded via HasData in configuration.
/// This seeder adds additional DanhMuc records needed for tests.
/// </summary>
public static class CatalogSeeder
{
    private static readonly DateTimeOffset SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedDanhMucLoaiDuAnAsync(db);
        await db.SaveChangesAsync();
    }

    private static async Task SeedDanhMucLoaiDuAnAsync(AppDbContext db)
    {
        if (await db.Set<DanhMucLoaiDuAn>().AnyAsync())
            return;

        db.Set<DanhMucLoaiDuAn>().AddRange(
            new DanhMucLoaiDuAn { Id = 1, Ma = "CDS", Ten = "Cơ sở hạ tầng", CreatedAt = SeedCreatedAt, Used = true },
            new DanhMucLoaiDuAn { Id = 2, Ma = "DA06", Ten = "Đề án 06", CreatedAt = SeedCreatedAt, Used = true },
            new DanhMucLoaiDuAn { Id = 3, Ma = "KHTP", Ten = "Kế hoạch TP HCM", CreatedAt = SeedCreatedAt, Used = true }
        );
    }
}
