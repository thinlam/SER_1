using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Domain.Entities;
using QLDA.Domain.Interfaces;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using BuildingBlocks.Domain.Interfaces;
using BuildingBlocks.Persistence.Repositories;
using Xunit;

namespace QLDA.Tests.Repositories;

[Collection("SharedSqlite")]
public class DuAnRepositoryTests(SharedSqliteFixture fixture)
{
    private IDuAnRepository CreateRepository()
    {
        var services = new ServiceCollection();
        services.AddScoped<DbContext>(_ => fixture.DbContext);
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IDuAnRepository, QLDA.Persistence.Repositories.DuAnRepository>();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IDuAnRepository>();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistDuAn()
    {
        var repo = CreateRepository();
        var duAn = new DuAnFaker().WithTrangThai(1).Generate();

        await repo.AddAsync(duAn);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(duAn.Id);
        result.Should().NotBeNull();
        result!.TenDuAn.Should().Be(duAn.TenDuAn);
        result.TrangThaiDuAnId.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDuAn()
    {
        var repo = CreateRepository();
        var duAns = new DuAnFaker().Generate(3);
        foreach (var d in duAns)
        {
            await repo.AddAsync(d);
        }
        await repo.SaveChangesAsync();

        var result = await repo.GetAllAsync();
        result.Should().HaveCountGreaterOrEqualTo(3);
    }

    [Fact]
    public async Task GetByTrangThaiAsync_ShouldFilterCorrectly()
    {
        var repo = CreateRepository();
        var dangThucHien = new DuAnFaker().WithTrangThai(1).Generate(2);
        var daHoanThanh = new DuAnFaker().WithTrangThai(3).Generate(1);
        foreach (var d in dangThucHien.Concat(daHoanThanh))
        {
            await repo.AddAsync(d);
        }
        await repo.SaveChangesAsync();

        var result = await repo.GetByTrangThaiAsync(1);
        result.Should().OnlyContain(d => d.TrangThaiDuAnId == 1);
    }

    [Fact]
    public async Task SearchAsync_ShouldFindByName()
    {
        var repo = CreateRepository();
        var target = new DuAnFaker().Generate();
        target.TenDuAn = "Hệ thống quản lý điện tử đặc biệt";
        await repo.AddAsync(target);
        await repo.SaveChangesAsync();

        var result = await repo.SearchAsync("đặc biệt");
        result.Should().ContainSingle(d => d.Id == target.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        var repo = CreateRepository();
        var duAn = new DuAnFaker().Generate();
        await repo.AddAsync(duAn);
        await repo.SaveChangesAsync();

        duAn.TenDuAn = "Updated name";
        await repo.UpdateAsync(duAn);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(duAn.Id);
        result!.TenDuAn.Should().Be("Updated name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDelete()
    {
        var repo = CreateRepository();
        var duAn = new DuAnFaker().Generate();
        await repo.AddAsync(duAn);
        await repo.SaveChangesAsync();

        await repo.DeleteAsync(duAn.Id);
        await repo.SaveChangesAsync();

        // GetById uses GetQueryableSet which filters IsDeleted
        var result = await repo.GetByIdAsync(duAn.Id);
        result.Should().BeNull();
    }
}
