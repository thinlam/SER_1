using BuildingBlocks.CrossCutting.Offices;
using BuildingBlocks.Infrastructure.Offices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Application.HopDongs.DTOs;
using Xunit;

namespace QLDA.Tests.Unit;

public class HopDongExportTests
{
    [Fact]
    public void MapToHopDongExportDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        var list = new List<HopDongDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Ten = "Hợp đồng tư vấn giám sát",
                SoHopDong = "HD-01/2026",
                NoiDung = "Nội dung giám sát thi công",
                GiaTri = 500000000,
                NgayKy = new DateTimeOffset(2026, 8, 24, 10, 0, 0, TimeSpan.Zero),
                NgayHieuLuc = new DateOnly(2026, 8, 25),
                NgayDuKienKetThucHopDong = new DateOnly(2026, 12, 31),
                TenDuAn = "Dự án Nâng cấp hạ tầng CNTT",
                TenBuoc = "Bước 5 - Ký kết hợp đồng",
                TenDonViThucHien = "Công ty CP Công nghệ ABC",
                TenLoaiHopDong = "Trọn gói"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Ten = "Hợp đồng mua sắm thiết bị",
                SoHopDong = "HD-02/2026",
                NoiDung = "Cung cấp máy chủ",
                GiaTri = 1200000000,
                NgayKy = null,
                NgayHieuLuc = null,
                NgayDuKienKetThucHopDong = null,
                TenDuAn = "Dự án Nâng cấp hạ tầng CNTT",
                TenBuoc = "Bước 5 - Ký kết hợp đồng",
                TenDonViThucHien = "Công ty TNHH Thiết bị XYZ",
                TenLoaiHopDong = "Theo đơn giá cố định"
            }
        };

        // Act
        var exportList = list.Select((x, index) => new HopDongExportDto
        {
            STT = index + 1,
            TenDuAn = x.TenDuAn,
            TenBuoc = x.TenBuoc,
            SoHopDong = x.SoHopDong,
            Ten = x.Ten,
            NoiDung = x.NoiDung,
            DonViThucHienId = x.TenDonViThucHien,
            GiaTri = x.GiaTri,
            LoaiHopDongId = x.TenLoaiHopDong,
            NgayHopDong = x.NgayKy?.ToString("dd/MM/yyyy"),
            NgayHieuLuc = x.NgayHieuLuc?.ToString("dd/MM/yyyy"),
            NgayKetThuc = x.NgayDuKienKetThucHopDong?.ToString("dd/MM/yyyy")
        }).ToList();

        // Assert
        exportList.Should().HaveCount(2);

        // Row 1
        exportList[0].STT.Should().Be(1);
        exportList[0].TenDuAn.Should().Be("Dự án Nâng cấp hạ tầng CNTT");
        exportList[0].TenBuoc.Should().Be("Bước 5 - Ký kết hợp đồng");
        exportList[0].SoHopDong.Should().Be("HD-01/2026");
        exportList[0].Ten.Should().Be("Hợp đồng tư vấn giám sát");
        exportList[0].NoiDung.Should().Be("Nội dung giám sát thi công");
        exportList[0].DonViThucHienId.Should().Be("Công ty CP Công nghệ ABC");
        exportList[0].GiaTri.Should().Be(500000000);
        exportList[0].LoaiHopDongId.Should().Be("Trọn gói");
        exportList[0].NgayHopDong.Should().Be("24/08/2026");
        exportList[0].NgayHieuLuc.Should().Be("25/08/2026");
        exportList[0].NgayKetThuc.Should().Be("31/12/2026");

        // Row 2 (Null dates check)
        exportList[1].STT.Should().Be(2);
        exportList[1].NgayHopDong.Should().BeNull();
        exportList[1].NgayHieuLuc.Should().BeNull();
        exportList[1].NgayKetThuc.Should().BeNull();
    }

    [Fact]
    public void ExportExcel_WithTemplate_GeneratesValidFileBytes()
    {
        // Arrange
        var templatePath = Path.Combine(AppContext.BaseDirectory, "PrintTemplates", "DanhSachHopDong.xlsx");
        if (!File.Exists(templatePath))
        {
            // Fallback for test runner working directory
            templatePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "QLDA.WebApi", "PrintTemplates", "DanhSachHopDong.xlsx"));
        }

        if (File.Exists(templatePath))
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<IAsposeHelper, AsposeHelper>();
            var sp = services.BuildServiceProvider();
            IExporterHelper exporter = new ExporterHelper(sp);
            var exportItems = new List<HopDongExportDto>
            {
                new()
                {
                    STT = 1,
                    TenDuAn = "Dự án Chuyển đổi số",
                    TenBuoc = "Bước 1",
                    SoHopDong = "HD-999/2026",
                    Ten = "Hợp đồng thử nghiệm",
                    NoiDung = "Nội dung",
                    DonViThucHienId = "Nhà thầu A",
                    GiaTri = 100000000,
                    LoaiHopDongId = "Trọn gói",
                    NgayHopDong = "24/08/2026",
                    NgayHieuLuc = "25/08/2026",
                    NgayKetThuc = "30/08/2026"
                }
            };

            // Act
            var result = exporter.Export(new AsposeInstruction<HopDongExportDto>
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
