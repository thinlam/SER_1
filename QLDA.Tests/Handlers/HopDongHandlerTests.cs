using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Application.HopDongs.Queries;
using QLDA.Domain.Entities;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using BuildingBlocks.Domain.Interfaces;
using BuildingBlocks.Persistence.Repositories;
using Xunit;

namespace QLDA.Tests.Handlers;

[Collection("SharedSqlite")]
public class HopDongHandlerTests(SharedSqliteFixture fixture)
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

    private async Task<HopDong> SeedHopDongChainAsync()
    {
        var data = await BusinessDataSeeder.SeedAsync(fixture.DbContext);
        return data.HopDong;
    }

    [Fact]
    public async Task HopDongGetQuery_ShouldReturnExistingHopDong()
    {
        var hopDong = await SeedHopDongChainAsync();
        var mediator = CreateMediator();

        var result = await mediator.Send(new HopDongGetQuery
        {
            Id = hopDong.Id,
            ThrowIfNull = true
        });

        result.Should().NotBeNull();
        result.Id.Should().Be(hopDong.Id);
        result.Ten.Should().Be(hopDong.Ten);
        result.SoHopDong.Should().Be(hopDong.SoHopDong);
    }

    [Fact]
    public async Task HopDongGetQuery_ShouldThrow_WhenNotFound()
    {
        var mediator = CreateMediator();

        var act = () => mediator.Send(new HopDongGetQuery
        {
            Id = Guid.NewGuid(),
            ThrowIfNull = true
        });

        await act.Should().ThrowAsync<BuildingBlocks.CrossCutting.Exceptions.ManagedException>();
    }

    [Fact]
    public async Task HopDongGetQuery_ShouldReturnLinkedDuAnAndGoiThau()
    {
        var data = await BusinessDataSeeder.SeedAsync(fixture.DbContext);
        var mediator = CreateMediator();

        var result = await mediator.Send(new HopDongGetQuery
        {
            Id = data.HopDong.Id,
            ThrowIfNull = true
        });

        result.Should().NotBeNull();
        result.DuAnId.Should().Be(data.DuAn.Id);
        result.GoiThauId.Should().Be(data.GoiThau.Id);
    }
}
