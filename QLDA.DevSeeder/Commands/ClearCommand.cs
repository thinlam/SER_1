using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using QLDA.DevSeeder.Services;
using QLDA.Persistence;

namespace QLDA.DevSeeder.Commands;

[Command("clear", Description = "Clear all seeded data (keeps catalog/master data)")]
public class ClearCommand
{
    [Option("-o|--output", Description = "SQLite file to clear")]
    public string? OutputFile { get; set; }

    [Option("--connection-string", Description = "SQL Server connection string")]
    public string? ConnectionString { get; set; }

    public async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(OutputFile) && string.IsNullOrEmpty(ConnectionString))
        {
            Console.WriteLine("Error: Specify --output <sqlite-file> or --connection-string.");
            return 1;
        }

        try
        {
            await using var db = CreateDbContext();
            var service = new DataGeneratorService(db);
            var removed = await service.ClearAsync(ct);

            Console.WriteLine($"Cleared {removed} records.");
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

        optionsBuilder.UseSqlite($"DataSource={OutputFile}");
        return new SqliteAppDbContext(optionsBuilder.Options);
    }
}
