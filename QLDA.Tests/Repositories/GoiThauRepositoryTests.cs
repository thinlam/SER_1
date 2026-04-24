using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Domain.Entities;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using BuildingBlocks.Domain.Interfaces;
using BuildingBlocks.Persistence.Repositories;
using Xunit;

namespace QLDA.Tests.Repositories;

[Collection("SharedSqlite")]
public class GoiThauRepositoryTests(SharedSqliteFixture fixture)
{
    private readonly DuAnFaker _duAnFaker = new();

    private async Task<DuAn> SeedDuAnAsync()
    {
        var duAn = _duAnFaker.WithTrangThai(1).Generate();
        fixture.DbContext.Set<DuAn>().Add(duAn);
        await fixture.DbContext.SaveChangesAsync();
        return duAn;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistGoiThau()
    {
        var duAn = await SeedDuAnAsync();
        var goiThau = new GoiThauFaker(duAn.Id).Generate();

        fixture.DbContext.Set<GoiThau>().Add(goiThau);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Set<GoiThau>().FindAsync(goiThau.Id);
        result.Should().NotBeNull();
        result!.Ten.Should().Be(goiThau.Ten);
        result.DuAnId.Should().Be(duAn.Id);
    }

    [Fact]
    public async Task Query_ShouldFilterByDuAn()
    {
        var duAn1 = await SeedDuAnAsync();
        var duAn2 = await SeedDuAnAsync();

        var gt1 = new GoiThauFaker(duAn1.Id).Generate();
        var gt2 = new GoiThauFaker(duAn2.Id).Generate();
        fixture.DbContext.Set<GoiThau>().AddRange(gt1, gt2);
        await fixture.DbContext.SaveChangesAsync();

        var results = await fixture.DbContext.Set<GoiThau>()
            .Where(g => g.DuAnId == duAn1.Id)
            .ToListAsync();

        results.Should().ContainSingle(g => g.Id == gt1.Id);
    }

    [Fact]
    public async Task Query_ShouldIncludeDuAnNavigation()
    {
        var duAn = await SeedDuAnAsync();
        var goiThau = new GoiThauFaker(duAn.Id).Generate();
        fixture.DbContext.Set<GoiThau>().Add(goiThau);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Set<GoiThau>()
            .Include(g => g.DuAn)
            .FirstOrDefaultAsync(g => g.Id == goiThau.Id);

        result.Should().NotBeNull();
        result!.DuAn.Should().NotBeNull();
        result.DuAn!.Id.Should().Be(duAn.Id);
    }
}
