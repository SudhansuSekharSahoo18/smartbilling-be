using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedMRPandDiscountToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountAmount",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountPercentage",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MRP",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2024, 7, 29, 14, 39, 21, 764, DateTimeKind.Local).AddTicks(3502));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DiscountAmount", "DiscountPercentage", "IsTaxInclusive", "MRP" },
                values: new object[] { 0.0, 0.0, true, 0.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MRP",
                table: "Items");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2024, 7, 24, 0, 43, 5, 84, DateTimeKind.Local).AddTicks(2064));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsTaxInclusive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsTaxInclusive",
                value: false);
        }
    }
}
