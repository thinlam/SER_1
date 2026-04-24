using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace QLDA.Persistence;


public class GiaoViecDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (optionsBuilder.IsConfigured)
            return;
        ManagedException.Throw("Chưa cấu hình kết nối database");
    }
}
