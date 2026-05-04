using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDA.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddTrangThaiToHoSoDeXuatCapDoCntt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HoSoDeXuatCapDoCntt_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt",
                column: "TrangThaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt",
                column: "TrangThaiId",
                principalTable: "DmTrangThaiPheDuyetDuToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoDeXuatCapDoCntt_DmTrangThaiPheDuyetDuToan_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt");

            migrationBuilder.DropIndex(
                name: "IX_HoSoDeXuatCapDoCntt_TrangThaiId",
                table: "HoSoDeXuatCapDoCntt");
        }
    }
}
