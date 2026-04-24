using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QLDA.Persistence;
using QLDA.Tests.Fakers;
using Xunit;

namespace QLDA.Tests.Fixtures;

/// <summary>
/// File-based SQLite fixture for integration tests. Each instance gets its own DB file.
/// </summary>
public class IsolatedSqliteFixture : IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"qlda-test-{Guid.NewGuid()}.db");
    private SqliteConnection _connection = null!;

    public AppDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection($"DataSource={_dbPath}");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new TestAppDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();
        await CatalogSeeder.SeedAsync(DbContext);
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();

        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }
}
