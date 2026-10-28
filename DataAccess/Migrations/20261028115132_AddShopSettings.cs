using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShopSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarcodeFilePath",
                table: "ApplicationConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopAddress",
                table: "ApplicationConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopGSTNumber",
                table: "ApplicationConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                table: "ApplicationConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationConfiguration",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BarcodeFilePath", "ShopAddress", "ShopGSTNumber", "ShopName" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2026, 10, 28, 17, 21, 30, 961, DateTimeKind.Local).AddTicks(9945));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeFilePath",
                table: "ApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "ShopAddress",
                table: "ApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "ShopGSTNumber",
                table: "ApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "ShopName",
                table: "ApplicationConfiguration");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2026, 10, 27, 23, 55, 35, 362, DateTimeKind.Local).AddTicks(9278));
        }
    }
}
