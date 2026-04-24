using Microsoft.EntityFrameworkCore;
using QLDA.Persistence;

namespace QLDA.Tests;

/// <summary>
/// Test-specific DbContext that clears SQL Server-specific defaults for SQLite compatibility.
/// </summary>
public class TestAppDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ClearSqlServerDefaults(modelBuilder);
    }

    /// <summary>
    /// Remove SQL Server-specific default SQL expressions and index filters.
    /// SQLite doesn't support NEWSEQUENTIALID, SYSDATETIMEOFFSET, DATEDIFF, or [bracket] filter syntax.
    /// </summary>
    private static void ClearSqlServerDefaults(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var entityBuilder = modelBuilder.Entity(entityType.ClrType);

            foreach (var property in entityType.GetProperties().ToList())
            {
                try
                {
                    var propBuilder = entityBuilder.Property(property.Name);
                    propBuilder.HasDefaultValueSql((string?)null);

                    // Replace SQL Server-specific column types (nvarchar(max), etc.)
                    var columnType = property.GetColumnType();
                    if (columnType != null && columnType.Contains("max", StringComparison.OrdinalIgnoreCase))
                    {
                        propBuilder.HasColumnType("TEXT");
                    }
                }
                catch
                {
                    // Skip properties that can't be modified
                }
            }

            foreach (var index in entityType.GetIndexes().ToList())
            {
                var propertyNames = index.Properties.Select(p => p.Name).ToArray();
                try
                {
                    entityBuilder.HasIndex(propertyNames).HasFilter((string?)null);
                }
                catch
                {
                    // Skip indexes that can't be modified
                }
            }
        }
    }
}
