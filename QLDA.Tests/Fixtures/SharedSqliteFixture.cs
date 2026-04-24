using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QLDA.Persistence;
using QLDA.Tests.Fakers;
using Xunit;

namespace QLDA.Tests.Fixtures;

/// <summary>
/// Shared in-memory SQLite fixture for unit tests. Single DB shared across tests in a collection.
/// </summary>
[CollectionDefinition("SharedSqlite")]
public class SharedSqliteCollection : ICollectionFixture<SharedSqliteFixture>;

public class SharedSqliteFixture : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public AppDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
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
    }
}
