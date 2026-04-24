using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using QLDA.DevSeeder.Services;
using QLDA.Domain.Entities.DanhMuc;
using QLDA.Persistence;

namespace QLDA.DevSeeder.Commands;

[Command("seed", Description = "Seed fake data into the database")]
public class SeedCommand
{
    [Option("-c|--count", Description = "Number of entities per type to generate (default: 10)")]
    public int Count { get; set; } = 10;

    [Option("-t|--type", Description = "Entity type: duan, goithau, hopdong, all (default: all)")]
    public string Type { get; set; } = "all";

    [Option("-o|--output", Description = "SQLite output file path (default: in-memory, no persistence)")]
    public string? OutputFile { get; set; }

    [Option("--connection-string", Description = "SQL Server connection string (overrides output file)")]
    public string? ConnectionString { get; set; }

    public async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken ct)
    {
        if (Count < 1)
        {
            Console.WriteLine("Error: Count must be at least 1.");
            return 1;
        }

        Console.WriteLine($"Seeding {Type} (count: {Count})...");

        try
        {
            await using var db = CreateDbContext();
            await db.Database.EnsureCreatedAsync(ct);
            await SeedCatalogAsync(db, ct);

            var service = new DataGeneratorService(db);
            var saved = Type.ToLower() switch
            {
                "duan" => await service.SeedDuAnAsync(Count, ct),
                "goithau" => await service.SeedGoiThauAsync(Count, ct),
                "hopdong" => await service.SeedHopDongAsync(Count, ct),
                "all" => await service.SeedAllAsync(Count, ct),
                _ => throw new CommandParsingException(app, $"Unknown type: {Type}. Use: duan, goithau, hopdong, all")
            };

            Console.WriteLine($"Saved {saved} records.");
            if (OutputFile != null)
                Console.WriteLine($"Database saved to: {Path.GetFullPath(OutputFile)}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        if (!string.IsNullOrEmpty(ConnectionString))
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            return new AppDbContext(optionsBuilder.Options);
        }

        var connStr = !string.IsNullOrEmpty(OutputFile)
            ? $"DataSource={OutputFile}"
            : "DataSource=:memory:";
        optionsBuilder.UseSqlite(connStr);
        return new SqliteAppDbContext(optionsBuilder.Options);
    }

    private static async Task SeedCatalogAsync(AppDbContext db, CancellationToken ct)
    {
        if (!await db.Set<DanhMucLoaiDuAn>().AnyAsync(ct))
        {
            var ts = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
            db.Set<DanhMucLoaiDuAn>().AddRange(
                new DanhMucLoaiDuAn { Id = 1, Ma = "CDS", Ten = "Cơ sở hạ tầng", CreatedAt = ts, Used = true },
                new DanhMucLoaiDuAn { Id = 2, Ma = "DA06", Ten = "Đề án 06", CreatedAt = ts, Used = true },
                new DanhMucLoaiDuAn { Id = 3, Ma = "KHTP", Ten = "Kế hoạch TP HCM", CreatedAt = ts, Used = true }
            );
            await db.SaveChangesAsync(ct);
        }
    }
}
