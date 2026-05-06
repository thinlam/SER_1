using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Application.Common.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QLDA.Domain.Entities;
using QLDA.Persistence;
using QLDA.Tests.Fixtures;
using Xunit;

namespace QLDA.Tests.Integration;

[Collection("WebApi")]
public class PheDuyetNoiDungControllerTests(WebApiFixture fixture) {
    private HttpClient AuthedClient => fixture.CreateAuthenticatedClient();
    private HttpClient BgdClient => fixture.CreateBgdClient();
    private HttpClient HcthClient => fixture.CreateHcthClient();

    [Fact]
    public async Task GetDanhSach_ReturnsOk() {
        var response = await AuthedClient.GetAsync("/api/phe-duyet-noi-dung/danh-sach");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task GetChiTiet_ExistingId_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync();
        var response = await AuthedClient.GetAsync($"/api/phe-duyet-noi-dung/{id}/chi-tiet");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task GetChiTiet_NonExistentId_ReturnsFailure() {
        var fakeId = Guid.NewGuid();
        var response = await AuthedClient.GetAsync($"/api/phe-duyet-noi-dung/{fakeId}/chi-tiet");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task Trinh_CreatesPheDuyetNoiDung() {
        var vbqdId = await CreateTestVbqdAsync();
        var trinhModel = new { DuAnId = fixture.SeededDuAnId, BuocId = (int?)null, NoiDung = (string?)null };
        var response = await AuthedClient.PostAsJsonAsync(
            $"/api/phe-duyet-noi-dung/{vbqdId}/trinh", trinhModel);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task Trinh_DuplicateVBQD_ReturnsFailure() {
        var trinhModel = new { DuAnId = fixture.SeededDuAnId, BuocId = (int?)null, NoiDung = (string?)null };
        await AuthedClient.PostAsJsonAsync(
            $"/api/phe-duyet-noi-dung/{fixture.SeededVanBanQuyetDinhId}/trinh", trinhModel);

        // Try again with same VBQD
        var response = await AuthedClient.PostAsJsonAsync(
            $"/api/phe-duyet-noi-dung/{fixture.SeededVanBanQuyetDinhId}/trinh", trinhModel);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task Duyet_AsBgdUser_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/duyet", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task Duyet_AsNonBgdUser_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await AuthedClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/duyet", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task Duyet_WhenNotChoXuLy_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DD");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/duyet", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task TuChoi_AsBgdUser_WithReason_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await BgdClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/tu-choi", new { NoiDung = "Không hợp lệ" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task TuChoi_WithoutReason_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await BgdClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/tu-choi", new { NoiDung = "" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task TraLai_AsBgdUser_WithReason_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await BgdClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/tra-lai", new { NoiDung = "Cần sửa lại" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task KySo_WhenDaDuyet_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DD");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/ky-so", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task KySo_WhenNotDaDuyet_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/ky-so", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task ChuyenQLVB_WhenDaDuyet_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DD");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/chuyen-qlvb", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task ChuyenQLVB_WhenDaKySo_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DKS");
        var response = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{id}/chuyen-qlvb", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task PhatHanh_AsHcthUser_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DQLVB");
        var response = await HcthClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/phat-hanh",
            new { SoPhatHanh = "PH_001", NgayPhatHanh = (DateTimeOffset?)null });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task PhatHanh_AsNonHcthUser_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("DQLVB");
        var response = await BgdClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/phat-hanh",
            new { SoPhatHanh = "PH_001" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task GuiLai_WhenTraLai_ReturnsOk() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("TL");
        var response = await AuthedClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/gui-lai",
            new { NoiDung = (string?)null });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeTrue();
    }

    [Fact]
    public async Task GuiLai_WhenNotTraLai_ReturnsFailure() {
        var id = await fixture.CreatePheDuyetNoiDungAsync("CXL");
        var response = await AuthedClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{id}/gui-lai",
            new { NoiDung = (string?)null });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResultApi>();
        result.Should().NotBeNull();
        result!.Result.Should().BeFalse();
    }

    [Fact]
    public async Task FullWorkflow_TrinhToPhatHanh() {
        // 1. Create VBQD for this test
        var trinhModel = new { DuAnId = fixture.SeededDuAnId, BuocId = (int?)null, NoiDung = (string?)null };

        // Create fresh VBQD
        var vbqdId = await CreateTestVbqdAsync();

        // 2. Trinh
        var trinhRes = await AuthedClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{vbqdId}/trinh", trinhModel);
        (await trinhRes.Content.ReadFromJsonAsync<ResultApi>())!.Result.Should().BeTrue();

        var pdndId = await GetPdndIdByVbqdAsync(vbqdId);

        // 3. Duyet (BGĐ)
        var duyetRes = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{pdndId}/duyet", null);
        (await duyetRes.Content.ReadFromJsonAsync<ResultApi>())!.Result.Should().BeTrue();

        // 4. Ky so (BGĐ)
        var kySoRes = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{pdndId}/ky-so", null);
        (await kySoRes.Content.ReadFromJsonAsync<ResultApi>())!.Result.Should().BeTrue();

        // 5. Chuyen QLVB (BGĐ)
        var chuyenRes = await BgdClient.PostAsync($"/api/phe-duyet-noi-dung/{pdndId}/chuyen-qlvb", null);
        (await chuyenRes.Content.ReadFromJsonAsync<ResultApi>())!.Result.Should().BeTrue();

        // 6. Phat hanh (HC-TH)
        var phRes = await HcthClient.PostAsJsonAsync($"/api/phe-duyet-noi-dung/{pdndId}/phat-hanh",
            new { SoPhatHanh = "PH_FULL_001" });
        (await phRes.Content.ReadFromJsonAsync<ResultApi>())!.Result.Should().BeTrue();

        // 7. Verify lich su
        var lsRes = await AuthedClient.GetAsync($"/api/phe-duyet-noi-dung/{pdndId}/lich-su");
        lsRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestVbqdAsync() {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(fixture.GetSqliteConnection()).Options;
        using var db = new SqliteAppDbContext(options);
        var vbqd = new VanBanQuyetDinh {
            DuAnId = fixture.SeededDuAnId,
            TrichYeu = $"Test {Guid.NewGuid():N}",
            So = $"VB_{Guid.NewGuid():N}",
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false,
        };
        db.Set<VanBanQuyetDinh>().Add(vbqd);
        await db.SaveChangesAsync();
        return vbqd.Id;
    }

    private async Task<Guid> GetPdndIdByVbqdAsync(Guid vbqdId) {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(fixture.GetSqliteConnection()).Options;
        using var db = new SqliteAppDbContext(options);
        var pdnd = await db.Set<PheDuyetNoiDung>()
            .FirstAsync(e => e.VanBanQuyetDinhId == vbqdId);
        return pdnd.Id;
    }
}
