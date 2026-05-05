using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDA.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddDmCapDoCnttAndHoSoDeXuatCapDoCntt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DmCapDoCntt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stt = table.Column<int>(type: "int", nullable: true),
                    MaMau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_DmCapDoCntt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HoSoDeXuatCapDoCntt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    DuAnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocId = table.Column<int>(type: "int", nullable: true),
                    TrangThaiId = table.Column<int>(type: "int", nullable: true),
                    CapDoId = table.Column<int>(type: "int", nullable: true),
                    NgayTrinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonViChuTriId = table.Column<int>(type: "int", nullable: true),
                    NoiDungDeNghi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NoiDungBaoCao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NoiDungDuThao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "DATEDIFF(SECOND, '19700101', GETUTCDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoDeXuatCapDoCntt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoDeXuatCapDoCntt_DmCapDoCntt_CapDoId",
                        column: x => x.CapDoId,
                        principalTable: "DmCapDoCntt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HoSoDeXuatCapDoCntt_DuAn_DuAnId",
                        column: x => x.DuAnId,
                        principalTable: "DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DmCapDoCntt_Index",
                table: "DmCapDoCntt",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_DmCapDoCntt_Ma",
                table: "DmCapDoCntt",
                column: "Ma",
                unique: true,
                filter: "[Ma] IS NOT NULL AND [Ma] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoDeXuatCapDoCntt_CapDoId",
                table: "HoSoDeXuatCapDoCntt",
                column: "CapDoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoDeXuatCapDoCntt_DuAnId",
                table: "HoSoDeXuatCapDoCntt",
                column: "DuAnId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoDeXuatCapDoCntt_Index",
                table: "HoSoDeXuatCapDoCntt",
                column: "Index")
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoDeXuatCapDoCntt");

            migrationBuilder.DropTable(
                name: "DmCapDoCntt");
        }
    }
}
