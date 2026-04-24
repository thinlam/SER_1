# Testing Guide

## Quick Start

```bash
# Run all tests
dotnet test QLDA.Tests/QLDA.Tests.csproj

# Run specific test class
dotnet test QLDA.Tests --filter "FullyQualifiedName~DuAnHandlerTests"

# Generate fake data
dotnet run --project QLDA.DevSeeder -- seed -c 10 -o dev-data.db

# Or use PowerShell script
./scripts/generate-test-data.ps1 -Count 10 -Output dev-data.db
```

## Architecture

```
QLDA.Tests/           # xUnit test project
├── Fakers/           # Bogus fake data generators
│   ├── EntityFakerBase.cs       # Abstract base with deterministic seed
│   ├── DuAnFaker.cs             # DuAn entity faker
│   ├── GoiThauFaker.cs          # GoiThau entity faker
│   ├── HopDongFaker.cs          # HopDong entity faker
│   ├── CatalogSeeder.cs         # Master data seeding
│   └── BusinessDataSeeder.cs    # Linked data (DuAn→GoiThau→HopDong)
├── Fixtures/
│   ├── SharedSqliteFixture.cs   # In-memory SQLite (shared collection)
│   └── IsolatedSqliteFixture.cs # File-based SQLite (per-test DB)
├── Repositories/     # Repository-level tests
├── Handlers/         # MediatR handler tests
└── Tests/            # Infrastructure/fixture tests

QLDA.DevSeeder/       # CLI tool for data generation
├── Commands/
│   ├── SeedCommand.cs           # seed command
│   └── ClearCommand.cs          # clear command
└── Services/
    └── DataGeneratorService.cs  # Bogus data generation logic
```

## Test Fixtures

| Fixture | Type | Use Case |
|---------|------|----------|
| `SharedSqliteFixture` | In-memory | Fast unit tests (~5-10ms/test), shared DB |
| `IsolatedSqliteFixture` | File-based | Integration tests needing clean state |

Both fixtures use `TestAppDbContext` which clears SQL Server-specific defaults (NEWSEQUENTIALID, nvarchar(max), bracket filters) for SQLite compatibility.

## Writing Tests

### Repository Test Pattern

```csharp
[Collection("SharedSqlite")]
public class MyRepositoryTests(SharedSqliteFixture fixture)
{
    [Fact]
    public async Task ShouldPersistEntity()
    {
        var entity = new MyFaker().Generate();
        fixture.DbContext.Set<MyEntity>().Add(entity);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Set<MyEntity>().FindAsync(entity.Id);
        result.Should().NotBeNull();
    }
}
```

### Handler Test Pattern

```csharp
[Collection("SharedSqlite")]
public class MyHandlerTests(SharedSqliteFixture fixture)
{
    private IMediator CreateMediator()
    {
        var services = new ServiceCollection();
        services.AddScoped<DbContext>(_ => fixture.DbContext);
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IUnitOfWork), sp => sp.GetRequiredService<DbContext>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(DuAnInsertCommand).Assembly));
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
    }
}
```

### Bogus Faker Pattern

```csharp
public class MyEntityFaker : EntityFakerBase<MyEntity>
{
    public MyEntityFaker() : base()
    {
        RuleFor(e => e.Name, f => f.Company.CompanyName());
        RuleFor(e => e.CreatedAt, DateTimeOffset.UtcNow);
        RuleFor(e => e.IsDeleted, false);
    }

    public MyEntityFaker WithCustomField(int value)
    {
        RuleFor(e => e.CustomField, value);
        return this;
    }
}
```

## DevSeeder CLI

```bash
# Seed 10 of each entity type to SQLite file
dotnet run --project QLDA.DevSeeder -- seed -c 10 -o dev-data.db

# Seed only DuAn
dotnet run --project QLDA.DevSeeder -- seed -t duan -c 20 -o dev-data.db

# Seed to SQL Server
dotnet run --project QLDA.DevSeeder -- seed -c 10 --connection-string "Server=.;Database=QLDA;..."

# Clear all seeded data (keeps catalog/master data)
dotnet run --project QLDA.DevSeeder -- clear -o dev-data.db
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| `SQLite Error: 'near "max": syntax error'` | TestAppDbContext should clear nvarchar(max) → TEXT |
| `not constant default value` | TestAppDbContext clears DefaultValueSql (NEWSEQUENTIALID etc.) |
| `relationship has been severed` | GoiThau-HopDong is 1:1, ensure unique GoiThauId per HopDong |
| Tests share data | SharedSqliteFixture uses shared DB; use IsolatedSqliteFixture for clean state |

## Test Count

| Category | Tests | Status |
|----------|-------|--------|
| Infrastructure | 6 | Passing |
| DuAn Repository | 6 | Passing |
| GoiThau Repository | 3 | Passing |
| HopDong Repository | 4 | Passing |
| DuAn Handler | 3 | Passing |
| HopDong Handler | 3 | Passing |
| **Total** | **25** | **All passing** |
