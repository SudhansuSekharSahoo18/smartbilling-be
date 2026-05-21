using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barcode = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Barcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemCode = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubTotal = table.Column<double>(type: "REAL", nullable: false),
                    DiscountAmount = table.Column<double>(type: "REAL", nullable: false),
                    TotalAmount = table.Column<double>(type: "REAL", nullable: false),
                    ModeOfPayment = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Due = table.Column<double>(type: "REAL", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    BillId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MRP = table.Column<double>(type: "REAL", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "REAL", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillItem_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    HSNCode = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CostPrice = table.Column<double>(type: "REAL", nullable: false),
                    MRP = table.Column<double>(type: "REAL", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "REAL", nullable: false),
                    Tax = table.Column<double>(type: "REAL", nullable: false),
                    IsTaxInclusive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ApplicationConfiguration",
                columns: new[] { "Id", "Barcode", "Version" },
                values: new object[] { 1, 101, "1.0.0" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedById", "CreatedDateTime", "IsAdmin", "Name", "Password", "PhoneNumber", "UpdatedById", "UpdatedDateTime", "UserName" },
                values: new object[] { 1, 1, new DateTime(2026, 5, 21, 12, 15, 18, 394, DateTimeKind.Local).AddTicks(3798), true, "System", "12345", "1234567890", null, null, "System" });

            migrationBuilder.CreateIndex(
                name: "IX_BillItem_BillId",
                table: "BillItem",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationConfiguration");

            migrationBuilder.DropTable(
                name: "Barcodes");

            migrationBuilder.DropTable(
                name: "BillItem");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
