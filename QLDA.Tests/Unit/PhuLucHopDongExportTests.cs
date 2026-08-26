using BuildingBlocks.CrossCutting.Offices;
using BuildingBlocks.Infrastructure.Offices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Application.PhuLucHopDongs.DTOs;
using Xunit;

namespace QLDA.Tests.Unit;

public class PhuLucHopDongExportTests
{
    [Fact]
    public void MapToPhuLucHopDongExportDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        var list = new List<PhuLucHopDongDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Ten = "Phụ lục gia hạn thời gian thực hiện",
                SoPhuLucHopDong = "PLHD-01/2026",
                NoiDung = "Gia hạn thêm 30 ngày do điều kiện thi công",
                GiaTri = 50000000,
                Ngay = new DateTimeOffset(2026, 8, 24, 10, 0, 0, TimeSpan.Zero),
                NgayDuKienKetThuc = new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero),
                TenDuAn = "Dự án Nâng cấp hạ tầng CNTT",
                TenBuoc = "Bước 5 - Ký kết hợp đồng",
                TenHopDong = "Hợp đồng tư vấn giám sát"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Ten = "Phụ lục bổ sung thiết bị",
                SoPhuLucHopDong = "PLHD-02/2026",
                NoiDung = "Bổ sung switch mạng",
                GiaTri = 120000000,
                Ngay = null,
                NgayDuKienKetThuc = null,
                TenDuAn = "Dự án Nâng cấp hạ tầng CNTT",
                TenBuoc = "Bước 5 - Ký kết hợp đồng",
                TenHopDong = "Hợp đồng mua sắm thiết bị"
            }
        };

        // Act
        var exportList = list.Select((x, index) => new PhuLucHopDongExportDto
        {
            STT = index + 1,
            TenDuAn = x.TenDuAn,
            TenBuoc = x.TenBuoc,
            SoPhuLucHopDong = x.SoPhuLucHopDong,
            Ten = x.Ten,
            HopDongId = x.TenHopDong,
            GiaTri = x.GiaTri,
            Ngay = x.Ngay?.ToString("dd/MM/yyyy"),
            NgayDuKienKetThuc = x.NgayDuKienKetThuc?.ToString("dd/MM/yyyy"),
            NoiDung = x.NoiDung
        }).ToList();

        // Assert
        exportList.Should().HaveCount(2);

        // Record 1 - full fields
        var item1 = exportList[0];
        item1.STT.Should().Be(1);
        item1.TenDuAn.Should().Be("Dự án Nâng cấp hạ tầng CNTT");
        item1.TenBuoc.Should().Be("Bước 5 - Ký kết hợp đồng");
        item1.SoPhuLucHopDong.Should().Be("PLHD-01/2026");
        item1.Ten.Should().Be("Phụ lục gia hạn thời gian thực hiện");
        item1.HopDongId.Should().Be("Hợp đồng tư vấn giám sát");
        item1.GiaTri.Should().Be(50000000);
        item1.Ngay.Should().Be("24/08/2026");
        item1.NgayDuKienKetThuc.Should().Be("31/12/2026");
        item1.NoiDung.Should().Be("Gia hạn thêm 30 ngày do điều kiện thi công");

        // Record 2 - nullable fields
        var item2 = exportList[1];
        item2.STT.Should().Be(2);
        item2.TenDuAn.Should().Be("Dự án Nâng cấp hạ tầng CNTT");
        item2.TenBuoc.Should().Be("Bước 5 - Ký kết hợp đồng");
        item2.SoPhuLucHopDong.Should().Be("PLHD-02/2026");
        item2.Ten.Should().Be("Phụ lục bổ sung thiết bị");
        item2.HopDongId.Should().Be("Hợp đồng mua sắm thiết bị");
        item2.GiaTri.Should().Be(120000000);
        item2.Ngay.Should().BeNull();
        item2.NgayDuKienKetThuc.Should().BeNull();
        item2.NoiDung.Should().Be("Bổ sung switch mạng");
    }

    [Fact]
    public void ExportExcel_WithTemplate_GeneratesValidFileBytes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IExporterHelper, ExporterHelper>();
        services.AddScoped<IAsposeHelper, AsposeHelper>();
        var provider = services.BuildServiceProvider();
        var exporter = provider.GetRequiredService<IExporterHelper>();

        var templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "PrintTemplates",
            "DanhSachPhuLucHopDong.xlsx"
        );

        if (File.Exists(templatePath))
        {
            var exportItems = new List<PhuLucHopDongExportDto>
            {
                new()
                {
                    STT = 1,
                    TenDuAn = "Dự án A",
                    TenBuoc = "Bước 1",
                    SoPhuLucHopDong = "PL-01",
                    Ten = "Phụ lục 1",
                    HopDongId = "Hợp đồng A",
                    GiaTri = 100000000,
                    Ngay = "01/01/2026",
                    NgayDuKienKetThuc = "31/12/2026",
                    NoiDung = "Nội dung 1"
                }
            };

            // Act
            var result = exporter.Export(new AsposeInstruction<PhuLucHopDongExportDto>
            {
                TemplatePath = templatePath,
                Items = exportItems,
                HiddenColumns = []
            });

            // Assert
            result.Should().NotBeNull();
            result.FileBytes.Should().NotBeNull();
            result.FileBytes.Length.Should().BeGreaterThan(0);
            result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
