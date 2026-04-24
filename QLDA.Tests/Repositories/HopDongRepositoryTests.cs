using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QLDA.Domain.Entities;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using Xunit;

namespace QLDA.Tests.Repositories;

[Collection("SharedSqlite")]
public class HopDongRepositoryTests(SharedSqliteFixture fixture)
{
    private readonly DuAnFaker _duAnFaker = new();

    private async Task<(DuAn DuAn, GoiThau GoiThau)> SeedDuAnAndGoiThauAsync()
    {
        var duAn = _duAnFaker.WithTrangThai(1).Generate();
        fixture.DbContext.Set<DuAn>().Add(duAn);
        await fixture.DbContext.SaveChangesAsync();

        var goiThau = new GoiThauFaker(duAn.Id).Generate();
        fixture.DbContext.Set<GoiThau>().Add(goiThau);
        await fixture.DbContext.SaveChangesAsync();

        return (duAn, goiThau);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistHopDong()
    {
        var (duAn, goiThau) = await SeedDuAnAndGoiThauAsync();
        var hopDong = new HopDongFaker(duAn.Id, goiThau.Id).Generate();

        fixture.DbContext.Set<HopDong>().Add(hopDong);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Set<HopDong>().FindAsync(hopDong.Id);
        result.Should().NotBeNull();
        result!.Ten.Should().Be(hopDong.Ten);
        result.DuAnId.Should().Be(duAn.Id);
        result.GoiThauId.Should().Be(goiThau.Id);
    }

    [Fact]
    public async Task Query_ShouldIncludeNavigationProperties()
    {
        var (duAn, goiThau) = await SeedDuAnAndGoiThauAsync();
        var hopDong = new HopDongFaker(duAn.Id, goiThau.Id).Generate();
        fixture.DbContext.Set<HopDong>().Add(hopDong);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Set<HopDong>()
            .Include(h => h.DuAn)
            .Include(h => h.GoiThau)
            .FirstOrDefaultAsync(h => h.Id == hopDong.Id);

        result.Should().NotBeNull();
        result!.DuAn.Should().NotBeNull();
        result.DuAn!.TenDuAn.Should().Be(duAn.TenDuAn);
        result.GoiThau.Should().NotBeNull();
        result.GoiThau!.Ten.Should().Be(goiThau.Ten);
    }

    [Fact]
    public async Task Query_ShouldFilterByDuAn()
    {
        var (duAn1, goiThau1) = await SeedDuAnAndGoiThauAsync();
        var (duAn2, goiThau2) = await SeedDuAnAndGoiThauAsync();

        var hd1 = new HopDongFaker(duAn1.Id, goiThau1.Id).Generate();
        var hd2 = new HopDongFaker(duAn2.Id, goiThau2.Id).Generate();
        fixture.DbContext.Set<HopDong>().AddRange(hd1, hd2);
        await fixture.DbContext.SaveChangesAsync();

        var results = await fixture.DbContext.Set<HopDong>()
            .Where(h => h.DuAnId == duAn1.Id)
            .ToListAsync();

        results.Should().ContainSingle(h => h.Id == hd1.Id);
    }

    [Fact]
    public async Task BusinessDataSeeder_ShouldCreateLinkedEntities()
    {
        var data = await BusinessDataSeeder.SeedAsync(fixture.DbContext);

        // Verify relationships
        data.DuAn.Should().NotBeNull();
        data.GoiThau.Should().NotBeNull();
        data.HopDong.Should().NotBeNull();

        data.GoiThau.DuAnId.Should().Be(data.DuAn.Id);
        data.HopDong.DuAnId.Should().Be(data.DuAn.Id);
        data.HopDong.GoiThauId.Should().Be(data.GoiThau.Id);

        // Verify DB persistence
        var duAnFromDb = await fixture.DbContext.Set<DuAn>()
            .Include(d => d.GoiThaus)
            .Include(d => d.HopDongs)
            .FirstOrDefaultAsync(d => d.Id == data.DuAn.Id);

        duAnFromDb.Should().NotBeNull();
        duAnFromDb!.GoiThaus.Should().ContainSingle(g => g.Id == data.GoiThau.Id);
        duAnFromDb.HopDongs.Should().ContainSingle(h => h.Id == data.HopDong.Id);
    }
}
