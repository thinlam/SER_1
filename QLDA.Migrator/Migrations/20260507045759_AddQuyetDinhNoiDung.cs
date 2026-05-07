using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLDA.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddQuyetDinhNoiDung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt");

            migrationBuilder.DropForeignKey(
                name: "FK_PheDuyetDuToan_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "PheDuyetDuToan");

            migrationBuilder.DropForeignKey(
                name: "FK_PheDuyetDuToanHistory_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "PheDuyetDuToanHistory");

            migrationBuilder.DropTable(
                name: "DmTrangThaiPheDuyetDuToan");

            migrationBuilder.AlterColumn<int>(
                name: "NguonVonId",
                table: "KeHoachVon",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ma",
                table: "DmQuyen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DmTrangThaiPheDuyet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stt = table.Column<int>(type: "int", nullable: true),
                    Loai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "DATEDIFF(SECOND, '19700101', GETUTCDATE())"),
                    Ma = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmTrangThaiPheDuyet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PheDuyetNoiDung",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    VanBanQuyetDinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DuAnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocId = table.Column<int>(type: "int", nullable: true),
                    TrangThaiId = table.Column<int>(type: "int", nullable: true),
                    NguoiXuLyId = table.Column<long>(type: "bigint", nullable: true),
                    NoiDungPhanHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoPhatHanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayPhatHanh = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DaChuyenQLVB = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "DATEDIFF(SECOND, '19700101', GETUTCDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PheDuyetNoiDung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDung_DmTrangThaiPheDuyet_TrangThaiId",
                        column: x => x.TrangThaiId,
                        principalTable: "DmTrangThaiPheDuyet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDung_DuAnBuoc_BuocId",
                        column: x => x.BuocId,
                        principalTable: "DuAnBuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDung_DuAn_DuAnId",
                        column: x => x.DuAnId,
                        principalTable: "DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDung_VanBanQuyetDinh_VanBanQuyetDinhId",
                        column: x => x.VanBanQuyetDinhId,
                        principalTable: "VanBanQuyetDinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PheDuyetNoiDungHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PheDuyetNoiDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DuAnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiXuLyId = table.Column<long>(type: "bigint", nullable: true),
                    TrangThaiId = table.Column<int>(type: "int", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayXuLy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "DATEDIFF(SECOND, '19700101', GETUTCDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PheDuyetNoiDungHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDungHistory_DmTrangThaiPheDuyet_TrangThaiId",
                        column: x => x.TrangThaiId,
                        principalTable: "DmTrangThaiPheDuyet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDungHistory_DuAn_DuAnId",
                        column: x => x.DuAnId,
                        principalTable: "DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetNoiDungHistory_PheDuyetNoiDung_PheDuyetNoiDungId",
                        column: x => x.PheDuyetNoiDungId,
                        principalTable: "PheDuyetNoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.InsertData(
                                        table: "DmQuyen",
                                        columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Ma", "MoTa", "NhomQuyen", "Stt", "Ten", "UpdatedAt", "UpdatedBy", "Used" },
                                        values: new object[,]
                                        {
                    { 23, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.XemTatCa", null, "PheDuyet", 1, "Xem tất cả PheDuyet", null, "", true },
                    { 24, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.XemTheoPhong", null, "PheDuyet", 2, "Xem theo phòng PheDuyet", null, "", true },
                    { 25, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.Duyet", null, "PheDuyet", 3, "Duyet PheDuyet", null, "", true },
                    { 26, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.KySo", null, "PheDuyet", 4, "KySo PheDuyet", null, "", true },
                    { 27, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.ChuyenQLVB", null, "PheDuyet", 5, "ChuyenQLVB PheDuyet", null, "", true },
                    { 28, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.PhatHanh", null, "PheDuyet", 6, "PhatHanh PheDuyet", null, "", true },
                    { 29, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "PheDuyet.TuChoi", null, "PheDuyet", 7, "TuChoi PheDuyet", null, "", true }
                                        });

            migrationBuilder.InsertData(
                table: "DmTrangThaiPheDuyet",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Loai", "Ma", "MoTa", "Stt", "Ten", "UpdatedAt", "UpdatedBy", "Used" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "DuToan", "DT", null, 1, "Dự thảo", null, "", true },
                    { 2, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "DuToan", "ĐTr", null, 2, "Đã trình", null, "", true },
                    { 3, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "DuToan", "ĐD", null, 3, "Đã duyệt", null, "", true },
                    { 4, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "DuToan", "TL", null, 4, "Trả lại", null, "", true },
                    { 5, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "DuToan", "LEG", null, 0, "Migrated", null, "", false },
                    { 6, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "CXL", null, 10, "Chờ xử lý", null, "", true },
                    { 7, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "TC", null, 11, "Từ chối", null, "", true },
                    { 8, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "DKS", null, 12, "Đã ký số", null, "", true },
                    { 9, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "DQLVB", null, 13, "Đã chuyển QLVB", null, "", true },
                    { 10, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "DPH", null, 14, "Đã phát hành", null, "", true },
                    { 11, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "DD", null, 15, "Đã duyệt", null, "", true },
                    { 12, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, "NoiDung", "TL", null, 16, "Trả lại", null, "", true }
                });

            migrationBuilder.InsertData(
                table: "CauHinhVaiTroQuyen",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "KichHoat", "QuyenId", "UpdatedAt", "UpdatedBy", "VaiTro" },
                values: new object[,]
                {
                    { 68, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 23, null, "", "QLDA_LD" },
                    { 73, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 24, null, "", "QLDA_ChuyenVien" }
                });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 23, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 24, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 25, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 26, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 27, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 28, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 29, "QLDA_TatCa" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 30,
                column: "QuyenId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 31,
                column: "QuyenId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 32,
                column: "QuyenId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 33,
                column: "QuyenId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 34,
                column: "QuyenId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 35,
                column: "QuyenId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 36,
                column: "QuyenId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 37,
                column: "QuyenId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 38,
                column: "QuyenId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 39,
                column: "QuyenId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 40,
                column: "QuyenId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 41,
                column: "QuyenId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 42,
                column: "QuyenId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 43,
                column: "QuyenId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 44,
                column: "QuyenId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 16, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 17, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 18, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 19, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 20, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 21, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 22, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 23, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 24, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 25, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 26, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 27, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 28, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 29, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 1, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 7, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 12, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 17, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 23, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 1, "QLDA_LD" });

            migrationBuilder.InsertData(
                           table: "CauHinhVaiTroQuyen",
                           columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "KichHoat", "QuyenId", "UpdatedAt", "UpdatedBy", "VaiTro" },
                           values: new object[,]
                           {
                    { 65, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 7, null, "", "QLDA_LD" },
                    { 66, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 12, null, "", "QLDA_LD" },
                    { 67, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 17, null, "", "QLDA_LD" },
                    { 69, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 2, null, "", "QLDA_ChuyenVien" },
                    { 70, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 8, null, "", "QLDA_ChuyenVien" },
                    { 71, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 13, null, "", "QLDA_ChuyenVien" },
                    { 72, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 18, null, "", "QLDA_ChuyenVien" },
                    { 74, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 3, null, "", "QLDA_ChuyenVien" },
                    { 75, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 4, null, "", "QLDA_ChuyenVien" },
                    { 76, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 9, null, "", "QLDA_ChuyenVien" },
                    { 77, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 10, null, "", "QLDA_ChuyenVien" },
                    { 78, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 14, null, "", "QLDA_ChuyenVien" },
                    { 79, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 15, null, "", "QLDA_ChuyenVien" },
                    { 80, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 19, null, "", "QLDA_ChuyenVien" },
                    { 81, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "", false, true, 20, null, "", "QLDA_ChuyenVien" }
                           });



            migrationBuilder.CreateIndex(
                name: "IX_DmTrangThaiPheDuyet_Index",
                table: "DmTrangThaiPheDuyet",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_DmTrangThaiPheDuyet_Ma_Loai",
                table: "DmTrangThaiPheDuyet",
                columns: new[] { "Ma", "Loai" },
                unique: true,
                filter: "[Ma] IS NOT NULL AND [Ma] <> '' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDung_BuocId",
                table: "PheDuyetNoiDung",
                column: "BuocId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDung_DuAnId",
                table: "PheDuyetNoiDung",
                column: "DuAnId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDung_Index",
                table: "PheDuyetNoiDung",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDung_TrangThaiId",
                table: "PheDuyetNoiDung",
                column: "TrangThaiId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDung_VanBanQuyetDinhId",
                table: "PheDuyetNoiDung",
                column: "VanBanQuyetDinhId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDungHistory_DuAnId",
                table: "PheDuyetNoiDungHistory",
                column: "DuAnId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDungHistory_Index",
                table: "PheDuyetNoiDungHistory",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDungHistory_PheDuyetNoiDungId",
                table: "PheDuyetNoiDungHistory",
                column: "PheDuyetNoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetNoiDungHistory_TrangThaiId",
                table: "PheDuyetNoiDungHistory",
                column: "TrangThaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyet_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyet",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            // migrationBuilder.CreateIndex(
            //                name: "IX_KeHoachVon_NguonVonId",
            //                table: "KeHoachVon",
            //                column: "NguonVonId");
            
            // migrationBuilder.AddForeignKey(
            //     name: "FK_KeHoachVon_DmNguonVon_NguonVonId",
            //     table: "KeHoachVon",
            //     column: "NguonVonId",
            //     principalTable: "DmNguonVon",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PheDuyetDuToan_DmTrangThaiPheDuyet_TrangThaiId",
                table: "PheDuyetDuToan",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PheDuyetDuToanHistory_DmTrangThaiPheDuyet_TrangThaiId",
                table: "PheDuyetDuToanHistory",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyet_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt");

            migrationBuilder.DropForeignKey(
                name: "FK_KeHoachVon_DmNguonVon_NguonVonId",
                table: "KeHoachVon");

            migrationBuilder.DropForeignKey(
                name: "FK_PheDuyetDuToan_DmTrangThaiPheDuyet_TrangThaiId",
                table: "PheDuyetDuToan");

            migrationBuilder.DropForeignKey(
                name: "FK_PheDuyetDuToanHistory_DmTrangThaiPheDuyet_TrangThaiId",
                table: "PheDuyetDuToanHistory");

            migrationBuilder.DropTable(
                name: "PheDuyetNoiDungHistory");

            migrationBuilder.DropTable(
                name: "PheDuyetNoiDung");

            migrationBuilder.DropTable(
                name: "DmTrangThaiPheDuyet");

            migrationBuilder.DropIndex(
                name: "IX_KeHoachVon_NguonVonId",
                table: "KeHoachVon");

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "DmQuyen",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.AlterColumn<Guid>(
                name: "NguonVonId",
                table: "KeHoachVon",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ma",
                table: "DmQuyen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DmTrangThaiPheDuyetDuToan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "DATEDIFF(SECOND, '19700101', GETUTCDATE())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Ma = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Stt = table.Column<int>(type: "int", nullable: true),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmTrangThaiPheDuyetDuToan", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 1, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 2, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 3, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 4, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 5, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 6, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 7, "QLDA_QuanTri" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 30,
                column: "QuyenId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 31,
                column: "QuyenId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 32,
                column: "QuyenId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 33,
                column: "QuyenId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 34,
                column: "QuyenId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 35,
                column: "QuyenId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 36,
                column: "QuyenId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 37,
                column: "QuyenId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 38,
                column: "QuyenId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 39,
                column: "QuyenId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 40,
                column: "QuyenId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 41,
                column: "QuyenId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 42,
                column: "QuyenId",
                value: 20);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 43,
                column: "QuyenId",
                value: 21);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 44,
                column: "QuyenId",
                value: 22);

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 1, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 7, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 12, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 17, "QLDA_LDDV" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 1, "QLDA_LD" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 7, "QLDA_LD" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 12, "QLDA_LD" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 17, "QLDA_LD" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 2, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 8, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 13, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 18, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 3, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 4, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 9, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 10, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 14, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 15, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 19, "QLDA_ChuyenVien" });

            migrationBuilder.UpdateData(
                table: "CauHinhVaiTroQuyen",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "QuyenId", "VaiTro" },
                values: new object[] { 20, "QLDA_ChuyenVien" });

            migrationBuilder.InsertData(
                table: "DmTrangThaiPheDuyetDuToan",
                columns: new[] { "Id", "CreatedBy", "IsDeleted", "Ma", "MoTa", "Stt", "Ten", "UpdatedAt", "UpdatedBy", "Used" },
                values: new object[,]
                {
                    { 1, "", false, "DT", null, 1, "Dự thảo", null, "", true },
                    { 2, "", false, "ĐTr", null, 2, "Đã trình", null, "", true },
                    { 3, "", false, "ĐD", null, 3, "Đã duyệt", null, "", true },
                    { 4, "", false, "TL", null, 4, "Trả lại", null, "", true },
                    { 5, "", false, "LEG", null, 0, "Migrated", null, "", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DmTrangThaiPheDuyetDuToan_Index",
                table: "DmTrangThaiPheDuyetDuToan",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_DmTrangThaiPheDuyetDuToan_Ma",
                table: "DmTrangThaiPheDuyetDuToan",
                column: "Ma",
                unique: true,
                filter: "[Ma] IS NOT NULL AND [Ma] <> ''");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyetDuToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PheDuyetDuToan_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "PheDuyetDuToan",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyetDuToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PheDuyetDuToanHistory_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "PheDuyetDuToanHistory",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyetDuToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
