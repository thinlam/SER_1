using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDuToanBanDauIdColumnClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add the DuToanBanDauId column to the existing DuAn table
            migrationBuilder.AddColumn<Guid>(
                name: "DuToanBanDauId",
                table: "DuAn",
                type: "uniqueidentifier",
                nullable: true);

            // Add foreign key relationship
            migrationBuilder.CreateIndex(
                name: "IX_DuAn_DuToanBanDauId",
                table: "DuAn",
                column: "DuToanBanDauId");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAn_DuToan_DuToanBanDauId",
                table: "DuAn",
                column: "DuToanBanDauId",
                principalTable: "DuToan",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAn_DuToan_DuToanBanDauId",
                table: "DuAn");

            migrationBuilder.DropIndex(
                name: "IX_DuAn_DuToanBanDauId",
                table: "DuAn");

            migrationBuilder.DropColumn(
                name: "DuToanBanDauId",
                table: "DuAn");
        }
    }
}
