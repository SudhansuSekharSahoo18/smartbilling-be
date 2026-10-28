using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBillItemSaleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "BillItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HSNCode",
                table: "BillItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Tax",
                table: "BillItem",
                type: "REAL",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2026, 10, 28, 18, 46, 58, 904, DateTimeKind.Local).AddTicks(5391));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "BillItem");

            migrationBuilder.DropColumn(
                name: "HSNCode",
                table: "BillItem");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "BillItem");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2026, 10, 28, 17, 21, 30, 961, DateTimeKind.Local).AddTicks(9945));
        }
    }
}
