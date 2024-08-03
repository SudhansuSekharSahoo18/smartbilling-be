using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Updated_ItemBillItemModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Barcodes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Barcodes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BillItem",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BillItem",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BillItem",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BillItem",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Bills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "BillItem");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BillItem",
                newName: "MRP");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "BillItem",
                newName: "DiscountPercentage");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "BillItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2024, 8, 3, 17, 43, 44, 610, DateTimeKind.Local).AddTicks(9680));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "BillItem");

            migrationBuilder.RenameColumn(
                name: "MRP",
                table: "BillItem",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "BillItem",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<double>(
                name: "DiscountAmount",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SellPrice",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "BillItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Barcodes",
                columns: new[] { "Id", "ItemCode", "ItemName", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, "101", "Barcode 1", 100, 1 },
                    { 2, "102", "Barcode 2", 200, 1 }
                });

            migrationBuilder.InsertData(
                table: "Bills",
                columns: new[] { "Id", "CreatedById", "CreatedDateTime", "CustomerAddress", "CustomerName", "DiscountAmount", "ModeOfPayment", "SubTotal", "TotalAmount", "UpdatedById", "UpdatedDateTime" },
                values: new object[] { 1, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "NA", "default", 0.0, "Cash", 2000.0, 2000.0, null, null });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Action" },
                    { 2, 2, "SciFi" },
                    { 3, 3, "History" }
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2024, 7, 29, 14, 39, 21, 764, DateTimeKind.Local).AddTicks(3502));

            migrationBuilder.InsertData(
                table: "BillItem",
                columns: new[] { "Id", "Amount", "Barcode", "BillId", "DiscountAmount", "ItemId", "ItemName", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 500.0, "101", 1, 0.0, 0, "Shirt", 500.0, 1 },
                    { 2, 500.0, "102", 1, 0.0, 0, "T-Shirt", 500.0, 1 },
                    { 3, 500.0, "103", 1, 0.0, 0, "Jeans", 500.0, 1 },
                    { 4, 500.0, "104", 1, 0.0, 0, "T-Shirt", 500.0, 1 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Barcode", "CategoryId", "CostPrice", "CreatedById", "CreatedDateTime", "Description", "DiscountAmount", "DiscountPercentage", "HSNCode", "IsTaxInclusive", "ItemName", "MRP", "Quantity", "SellPrice", "Tax", "Unit", "UpdatedById", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, "101", 1, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Saree", 0.0, 1, 500.0, 0.0, "Pieces", null, null },
                    { 2, "102", 2, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Jeans", 0.0, 1, 1500.0, 0.0, "Pieces", null, null },
                    { 3, "103", 2, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Shirt", 0.0, 1, 400.0, 0.0, "Pieces", null, null },
                    { 4, "104", 1, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Socks", 0.0, 1, 150.0, 0.0, "Pieces", null, null },
                    { 5, "105", 3, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Lungi", 0.0, 1, 80.0, 0.0, "Pieces", null, null },
                    { 6, "106", 1, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Frock", 0.0, 1, 345.0, 0.0, "Pieces", null, null },
                    { 7, "107", 3, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Lengha", 0.0, 1, 800.0, 0.0, "Pieces", null, null },
                    { 8, "108", 1, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Gamcha", 0.0, 1, 120.0, 0.0, "Pieces", null, null },
                    { 9, "109", 2, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Shoes", 0.0, 1, 700.0, 0.0, "Pieces", null, null },
                    { 10, "110", 1, 300.0, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 0.0, 0.0, null, true, "Cap", 0.0, 1, 50.0, 0.0, "Pieces", null, null }
                });
        }
    }
}
