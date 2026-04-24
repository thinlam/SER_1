using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QLDA.Domain.Entities;
using QLDA.Domain.Entities.DanhMuc;
using QLDA.Tests.Fakers;
using QLDA.Tests.Fixtures;
using Xunit;

namespace QLDA.Tests.Tests;

[Collection("SharedSqlite")]
public class InfrastructureTests(SharedSqliteFixture fixture)
{
    [Fact]
    public async Task SharedSqliteFixture_ShouldCreateSqliteDatabase()
    {
        fixture.DbContext.Database.IsSqlite().Should().BeTrue();
        var canConnect = await fixture.DbContext.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task CatalogSeeder_TrangThaiDuAnShouldBeSeeded()
    {
        var trangThai = await fixture.DbContext.Set<DanhMucTrangThaiDuAn>().ToListAsync();

        trangThai.Should().HaveCount(4);
        trangThai.Select(t => t.Ten).Should().Contain("Đang thực hiện");
    }

    [Fact]
    public async Task CatalogSeeder_LoaiDuAnShouldBeSeeded()
    {
        var loaiDuAn = await fixture.DbContext.Set<DanhMucLoaiDuAn>().ToListAsync();

        loaiDuAn.Should().HaveCount(3);
        loaiDuAn.Select(l => l.Ma).Should().Contain("CDS");
    }

    [Fact]
    public void DuAnFaker_ShouldGenerateValidEntity()
    {
        var duAn = new DuAnFaker().Generate();

        duAn.Should().NotBeNull();
        duAn.TenDuAn.Should().NotBeNullOrEmpty();
        duAn.MaDuAn.Should().NotBeNullOrEmpty();
        duAn.CreatedAt.Should().NotBe(default(DateTimeOffset));
        duAn.IsDeleted.Should().BeFalse();
        duAn.Id.Should().NotBeEmpty();
        duAn.Path.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DuAnFaker_ShouldGenerateDeterministicData()
    {
        var duAn1 = new DuAnFaker().Generate();
        var duAn2 = new DuAnFaker().Generate();

        duAn1.TenDuAn.Should().Be(duAn2.TenDuAn);
        duAn1.MaDuAn.Should().Be(duAn2.MaDuAn);
    }

    [Fact]
    public async Task DuAnFaker_ShouldInsertIntoDatabase()
    {
        var duAn = new DuAnFaker().WithTrangThai(1).Generate();

        var dbSet = fixture.DbContext.Set<DuAn>();
        await dbSet.AddAsync(duAn);
        await fixture.DbContext.SaveChangesAsync();

        var saved = await dbSet.FirstOrDefaultAsync(d => d.Id == duAn.Id);
        saved.Should().NotBeNull();
        saved!.TenDuAn.Should().Be(duAn.TenDuAn);
    }
}
