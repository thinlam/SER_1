using BuildingBlocks.CrossCutting.Offices;
using BuildingBlocks.Infrastructure.Offices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QLDA.Application.BaoCaoTienDos.DTOs;
using Xunit;

namespace QLDA.Tests.Unit;

public class BaoCaoTienDoExportTests
{
    [Fact]
    public void MapToBaoCaoTienDoExportDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        var list = new List<BaoCaoTienDoDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DuAnId = Guid.NewGuid(),
                TenDuAn = "Dự án Chuyển đổi số 2026",
                BuocId = 1,
                TenBuoc = "Bước 1 - Khởi tạo dự án",
                NoiDung = "Đã hoàn thành khảo sát hiện trạng hạ tầng mạng",
                Ngay = new DateTimeOffset(2026, 8, 24, 14, 30, 0, TimeSpan.Zero),
                NguoiBaoCaoId = 23,
                TenNguoiBaoCao = "Nguyễn Văn Hậu"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DuAnId = Guid.NewGuid(),
                TenDuAn = "Dự án Chuyển đổi số 2026",
                BuocId = 2,
                TenBuoc = "Bước 2 - Lập báo cáo nghiên cứu khả thi",
                NoiDung = "Đang tiến hành lập dự toán chi tiết",
                Ngay = null,
                NguoiBaoCaoId = null,
                TenNguoiBaoCao = null
            }
        };

        // Act
        var exportList = list.Select((x, index) => new BaoCaoTienDoExportDto
        {
            STT = index + 1,
            TenDuAn = x.TenDuAn,
            TenBuoc = x.TenBuoc,
            NgayBaoCao = x.Ngay?.ToString("dd/MM/yyyy"),
            UserId = x.TenNguoiBaoCao,
            NoiDung = x.NoiDung
        }).ToList();

        // Assert
        exportList.Should().HaveCount(2);

        // Record 1 - full fields
        var item1 = exportList[0];
        item1.STT.Should().Be(1);
        item1.TenDuAn.Should().Be("Dự án Chuyển đổi số 2026");
        item1.TenBuoc.Should().Be("Bước 1 - Khởi tạo dự án");
        item1.NgayBaoCao.Should().Be("24/08/2026");
        item1.UserId.Should().Be("Nguyễn Văn Hậu");
        item1.NoiDung.Should().Be("Đã hoàn thành khảo sát hiện trạng hạ tầng mạng");

        // Record 2 - nullable fields
        var item2 = exportList[1];
        item2.STT.Should().Be(2);
        item2.TenDuAn.Should().Be("Dự án Chuyển đổi số 2026");
        item2.TenBuoc.Should().Be("Bước 2 - Lập báo cáo nghiên cứu khả thi");
        item2.NgayBaoCao.Should().BeNull();
        item2.UserId.Should().BeNull();
        item2.NoiDung.Should().Be("Đang tiến hành lập dự toán chi tiết");
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
            "DanhSachBaoCaoTienDo.xlsx"
        );

        if (File.Exists(templatePath))
        {
            var exportItems = new List<BaoCaoTienDoExportDto>
            {
                new()
                {
                    STT = 1,
                    TenDuAn = "Dự án Nâng cấp hạ tầng CNTT",
                    TenBuoc = "Bước 1 - Lập hồ sơ",
                    NgayBaoCao = "26/08/2026",
                    UserId = "Nguyễn Văn Hậu",
                    NoiDung = "Đã hoàn thành bàn giao giai đoạn 1"
                }
            };

            // Act
            var result = exporter.Export(new AsposeInstruction<BaoCaoTienDoExportDto>
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
