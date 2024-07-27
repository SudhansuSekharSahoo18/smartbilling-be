﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartBillingServer.DataAccess.Data;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Models.ApplicationConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Barcode")
                        .HasColumnType("int");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationConfiguration");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Barcode = 101,
                            Version = "1.0.0"
                        });
                });

            modelBuilder.Entity("Models.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("Due")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Models.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedById = 1,
                            CreatedDateTime = new DateTime(2024, 7, 24, 0, 43, 5, 84, DateTimeKind.Local).AddTicks(2064),
                            IsAdmin = true,
                            Name = "System",
                            Password = "12345",
                            PhoneNumber = "1234567890",
                            UserName = "System"
                        });
                });

            modelBuilder.Entity("Models.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<double>("CostPrice")
                        .HasColumnType("float");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HSNCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsTaxInclusive")
                        .HasColumnType("bit");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("SellPrice")
                        .HasColumnType("float");

                    b.Property<double>("Tax")
                        .HasColumnType("float");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Items");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Barcode = "101",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Saree",
                            Quantity = 1,
                            SellPrice = 500.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 2,
                            Barcode = "102",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Jeans",
                            Quantity = 1,
                            SellPrice = 1500.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 3,
                            Barcode = "103",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Shirt",
                            Quantity = 1,
                            SellPrice = 400.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 4,
                            Barcode = "104",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Socks",
                            Quantity = 1,
                            SellPrice = 150.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 5,
                            Barcode = "105",
                            CategoryId = 3,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Lungi",
                            Quantity = 1,
                            SellPrice = 80.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 6,
                            Barcode = "106",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Frock",
                            Quantity = 1,
                            SellPrice = 345.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 7,
                            Barcode = "107",
                            CategoryId = 3,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Lengha",
                            Quantity = 1,
                            SellPrice = 800.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 8,
                            Barcode = "108",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Gamcha",
                            Quantity = 1,
                            SellPrice = 120.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 9,
                            Barcode = "109",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Shoes",
                            Quantity = 1,
                            SellPrice = 700.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        },
                        new
                        {
                            Id = 10,
                            Barcode = "110",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "",
                            IsTaxInclusive = false,
                            ItemName = "Cap",
                            Quantity = 1,
                            SellPrice = 50.0,
                            Tax = 0.0,
                            Unit = "Pieces"
                        });
                });

            modelBuilder.Entity("SmartBillingServer.Models.Barcode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ItemCode")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Barcodes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ItemCode = "101",
                            ItemName = "Barcode 1",
                            Price = 100,
                            Quantity = 1
                        },
                        new
                        {
                            Id = 2,
                            ItemCode = "102",
                            ItemName = "Barcode 2",
                            Price = 200,
                            Quantity = 1
                        });
                });

            modelBuilder.Entity("SmartBillingServer.Models.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("DiscountAmount")
                        .HasColumnType("float");

                    b.Property<string>("ModeOfPayment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SubTotal")
                        .HasColumnType("float");

                    b.Property<double>("TotalAmount")
                        .HasColumnType("float");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Bills");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedById = 0,
                            CreatedDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CustomerAddress = "NA",
                            CustomerName = "default",
                            DiscountAmount = 0.0,
                            ModeOfPayment = "Cash",
                            SubTotal = 2000.0,
                            TotalAmount = 2000.0
                        });
                });

            modelBuilder.Entity("SmartBillingServer.Models.BillItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("Barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BillId")
                        .HasColumnType("int");

                    b.Property<double>("DiscountAmount")
                        .HasColumnType("float");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<string>("ItemName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BillId");

                    b.ToTable("BillItem");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 500.0,
                            Barcode = "101",
                            BillId = 1,
                            DiscountAmount = 0.0,
                            ItemId = 0,
                            ItemName = "Shirt",
                            Price = 500.0,
                            Quantity = 1
                        },
                        new
                        {
                            Id = 2,
                            Amount = 500.0,
                            Barcode = "102",
                            BillId = 1,
                            DiscountAmount = 0.0,
                            ItemId = 0,
                            ItemName = "T-Shirt",
                            Price = 500.0,
                            Quantity = 1
                        },
                        new
                        {
                            Id = 3,
                            Amount = 500.0,
                            Barcode = "103",
                            BillId = 1,
                            DiscountAmount = 0.0,
                            ItemId = 0,
                            ItemName = "Jeans",
                            Price = 500.0,
                            Quantity = 1
                        },
                        new
                        {
                            Id = 4,
                            Amount = 500.0,
                            Barcode = "104",
                            BillId = 1,
                            DiscountAmount = 0.0,
                            ItemId = 0,
                            ItemName = "T-Shirt",
                            Price = 500.0,
                            Quantity = 1
                        });
                });

            modelBuilder.Entity("SmartBillingServer.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "SciFi"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "History"
                        });
                });

            modelBuilder.Entity("Models.Models.Item", b =>
                {
                    b.HasOne("SmartBillingServer.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("SmartBillingServer.Models.BillItem", b =>
                {
                    b.HasOne("SmartBillingServer.Models.Bill", null)
                        .WithMany("BillItems")
                        .HasForeignKey("BillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartBillingServer.Models.Bill", b =>
                {
                    b.Navigation("BillItems");
                });
#pragma warning restore 612, 618
        }
    }
}
