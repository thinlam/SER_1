using Bogus;
using Microsoft.EntityFrameworkCore;
using QLDA.Domain.Entities;
using QLDA.Domain.Entities.DanhMuc;
using QLDA.Persistence;

namespace QLDA.DevSeeder.Services;

/// <summary>
/// Generates realistic fake data for development/testing using Bogus.
/// Supports both SQLite and SQL Server targets.
/// </summary>
public class DataGeneratorService
{
    private readonly AppDbContext _db;

    private static readonly string[] ProjectNames =
    [
        "Hệ thống quản lý văn bản điện tử", "Cổng thông tin điện tử thành phố",
        "Hệ thống quản lý hồ sơ điện tử", "Phần mềm quản lý nhân sự",
        "Hệ thống thanh toán điện tử", "Cơ sở dữ liệu quốc gia về dân cư",
        "Hệ thống giám sát an ninh mạng", "Phần mềm quản lý tài sản công",
        "Hệ thống chữ ký số", "Phần mềm quản lý dự án đầu tư"
    ];

    public DataGeneratorService(AppDbContext db) => _db = db;

    public async Task<int> SeedDuAnAsync(int count, CancellationToken ct = default)
    {
        var faker = new Faker<DuAn>()
            .UseSeed(12345)
            .RuleFor(e => e.TenDuAn, f => f.PickRandom(ProjectNames))
            .RuleFor(e => e.MaDuAn, f => $"DA-{f.Random.Number(1000, 9999)}")
            .RuleFor(e => e.DiaDiem, f => f.Address.City())
            .RuleFor(e => e.MaNganSach, f => $"NS-{f.Random.Number(100, 999)}")
            .RuleFor(e => e.DuAnTrongDiem, f => f.Random.Bool(0.3f))
            .RuleFor(e => e.ThoiGianKhoiCong, f => 2024 + f.Random.Int(0, 3))
            .RuleFor(e => e.ThoiGianHoanThanh, f => 2026 + f.Random.Int(0, 4))
            .RuleFor(e => e.TongMucDauTu, f => f.Random.Long(1_000_000_000, 100_000_000_000))
            .RuleFor(e => e.TrangThaiDuAnId, 1)
            .RuleFor(e => e.LoaiDuAnId, 1)
            .RuleFor(e => e.CreatedAt, DateTimeOffset.UtcNow)
            .RuleFor(e => e.CreatedBy, "dev-seeder")
            .RuleFor(e => e.IsDeleted, false)
            .RuleFor(e => e.Index, f => f.Random.Long(1_000_000))
            .RuleFor(e => e.ParentId, (Guid?)null)
            .RuleFor(e => e.Path, (_, e) => $"/{e.Id}/")
            .RuleFor(e => e.Level, 0);

        var entities = faker.Generate(count);
        _db.Set<DuAn>().AddRange(entities);
        return await _db.SaveChangesAsync(ct);
    }

    public async Task<int> SeedGoiThauAsync(int count, CancellationToken ct = default)
    {
        var duAnIds = await _db.Set<DuAn>().Select(d => d.Id).ToListAsync(ct);
        if (duAnIds.Count == 0) return 0;

        var faker = new Faker<GoiThau>()
            .UseSeed(54321)
            .RuleFor(e => e.DuAnId, f => f.PickRandom(duAnIds))
            .RuleFor(e => e.Ten, f => $"Gói thầu {f.Commerce.ProductName()}")
            .RuleFor(e => e.GiaTri, f => f.Random.Long(100_000_000, 50_000_000_000))
            .RuleFor(e => e.DaDuyet, false)
            .RuleFor(e => e.CreatedAt, DateTimeOffset.UtcNow)
            .RuleFor(e => e.CreatedBy, "dev-seeder")
            .RuleFor(e => e.IsDeleted, false)
            .RuleFor(e => e.Index, f => f.Random.Long(1_000_000));

        var entities = faker.Generate(count);
        _db.Set<GoiThau>().AddRange(entities);
        return await _db.SaveChangesAsync(ct);
    }

    public async Task<int> SeedHopDongAsync(int count, CancellationToken ct = default)
    {
        var duAnIds = await _db.Set<DuAn>().Select(d => d.Id).ToListAsync(ct);
        var goiThauIds = await _db.Set<GoiThau>().Select(g => g.Id).ToListAsync(ct);
        if (duAnIds.Count == 0 || goiThauIds.Count == 0) return 0;

        // GoiThau-HopDong is 1:1, so each HopDong needs a unique GoiThauId
        var random = new Random(98765);
        var shuffledGoiThau = goiThauIds.OrderBy(_ => random.Next()).ToList();
        var actualCount = Math.Min(count, shuffledGoiThau.Count);
        if (actualCount < count)
            Console.WriteLine($"  Note: Only {actualCount} HopDong created (limited by {shuffledGoiThau.Count} GoiThau)");

        var faker = new Faker<HopDong>()
            .UseSeed(98765)
            .RuleFor(e => e.DuAnId, f => f.PickRandom(duAnIds))
            .RuleFor(e => e.Ten, f => $"Hợp đồng {f.Commerce.ProductName()}")
            .RuleFor(e => e.SoHopDong, f => $"HD-{f.Random.Number(1000, 9999)}/{DateTime.UtcNow.Year}")
            .RuleFor(e => e.NoiDung, f => f.Lorem.Paragraph())
            .RuleFor(e => e.NgayKy, f => f.Date.PastOffset(1))
            .RuleFor(e => e.GiaTri, f => f.Random.Long(100_000_000, 50_000_000_000))
            .RuleFor(e => e.IsBienBan, false)
            .RuleFor(e => e.CreatedAt, DateTimeOffset.UtcNow)
            .RuleFor(e => e.CreatedBy, "dev-seeder")
            .RuleFor(e => e.IsDeleted, false)
            .RuleFor(e => e.Index, f => f.Random.Long(1_000_000));

        var entities = faker.Generate(actualCount);
        for (var i = 0; i < entities.Count; i++)
            entities[i].GoiThauId = shuffledGoiThau[i];

        _db.Set<HopDong>().AddRange(entities);
        return await _db.SaveChangesAsync(ct);
    }

    public async Task<int> SeedAllAsync(int countPerEntity, CancellationToken ct = default)
    {
        var total = 0;
        total += await SeedDuAnAsync(countPerEntity, ct);
        total += await SeedGoiThauAsync(countPerEntity, ct);
        total += await SeedHopDongAsync(countPerEntity, ct);
        return total;
    }

    public async Task<int> ClearAsync(CancellationToken ct = default)
    {
        var total = 0;
        total += await _db.Set<HopDong>().ExecuteDeleteAsync(ct);
        total += await _db.Set<GoiThau>().ExecuteDeleteAsync(ct);
        total += await _db.Set<DuAn>().ExecuteDeleteAsync(ct);
        return total;
    }
}
