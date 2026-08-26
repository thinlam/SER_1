using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDA.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDmTrangThaiDuAn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrangThaiDangTai",
                table: "KetQuaTrungThau",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsHoanThanh",
                table: "DmTrangThaiDuAn",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DmTrangThaiDuAn",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsHoanThanh",
                value: null);

            migrationBuilder.UpdateData(
                table: "DmTrangThaiDuAn",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsHoanThanh",
                value: null);

            migrationBuilder.UpdateData(
                table: "DmTrangThaiDuAn",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsHoanThanh",
                value: null);

            migrationBuilder.UpdateData(
                table: "DmTrangThaiDuAn",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsHoanThanh",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHoanThanh",
                table: "DmTrangThaiDuAn");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThaiDangTai",
                table: "KetQuaTrungThau",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
