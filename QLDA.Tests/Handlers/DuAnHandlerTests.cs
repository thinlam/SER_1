using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Application.DuAns.Queries;
using QLDA.Domain.Entities;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using BuildingBlocks.Domain.Interfaces;
using BuildingBlocks.Persistence.Repositories;
using Xunit;

namespace QLDA.Tests.Handlers;

[Collection("SharedSqlite")]
public class DuAnHandlerTests(SharedSqliteFixture fixture)
{
    private IMediator CreateMediator()
    {
        var services = new ServiceCollection();
        services.AddScoped<DbContext>(_ => fixture.DbContext);
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IUnitOfWork), sp => sp.GetRequiredService<DbContext>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(QLDA.Application.DuAns.Commands.DuAnInsertCommand).Assembly));
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
    }

    private async Task<DuAn> SeedDuAnAsync()
    {
        var duAn = new DuAnFaker().WithTrangThai(1).Generate();
        fixture.DbContext.Set<DuAn>().Add(duAn);
        await fixture.DbContext.SaveChangesAsync();
        return duAn;
    }

    [Fact]
    public async Task DuAnGetQuery_ShouldReturnExistingDuAn()
    {
        var duAn = await SeedDuAnAsync();
        var mediator = CreateMediator();

        var result = await mediator.Send(new DuAnGetQuery
        {
            Id = duAn.Id,
            ThrowIfNull = true,
            IsNoTracking = true
        });

        result.Should().NotBeNull();
        result.Id.Should().Be(duAn.Id);
        result.TenDuAn.Should().Be(duAn.TenDuAn);
    }

    [Fact]
    public async Task DuAnGetQuery_ShouldThrow_WhenNotFound()
    {
        var mediator = CreateMediator();

        var act = () => mediator.Send(new DuAnGetQuery
        {
            Id = Guid.NewGuid(),
            ThrowIfNull = true
        });

        await act.Should().ThrowAsync<BuildingBlocks.CrossCutting.Exceptions.ManagedException>();
    }

    [Fact]
    public async Task DuAnGetQuery_ShouldReturnNull_WhenNotThrowIfNull()
    {
        var mediator = CreateMediator();

        var result = await mediator.Send(new DuAnGetQuery
        {
            Id = Guid.NewGuid(),
            ThrowIfNull = false
        });

        result.Should().BeNull();
    }
}
