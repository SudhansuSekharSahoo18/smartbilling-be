using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeededBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillItem_Bills_BillId",
                table: "BillItem");

            migrationBuilder.AlterColumn<int>(
                name: "BillId",
                table: "BillItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Bills",
                columns: new[] { "Id", "CustomerAddress", "CustomerName", "DiscountAmount", "ModeOfPayment", "SubTotal", "TotalAmount" },
                values: new object[] { 1, "NA", "default", 0.0, "Cash", 2000.0, 2000.0 });

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

            migrationBuilder.AddForeignKey(
                name: "FK_BillItem_Bills_BillId",
                table: "BillItem",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillItem_Bills_BillId",
                table: "BillItem");

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
                table: "Bills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "BillId",
                table: "BillItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BillItem_Bills_BillId",
                table: "BillItem",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id");
        }
    }
}
