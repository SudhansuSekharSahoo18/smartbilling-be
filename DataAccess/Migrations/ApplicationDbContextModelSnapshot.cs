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

            modelBuilder.Entity("Models.Models.Product", b =>
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

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("SellPrice")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Barcode = "101",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 500.0,
                            Title = "Saree"
                        },
                        new
                        {
                            Id = 2,
                            Barcode = "102",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 1500.0,
                            Title = "Jeans"
                        },
                        new
                        {
                            Id = 3,
                            Barcode = "103",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 400.0,
                            Title = "Shirt"
                        },
                        new
                        {
                            Id = 4,
                            Barcode = "104",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 150.0,
                            Title = "Socks"
                        },
                        new
                        {
                            Id = 5,
                            Barcode = "105",
                            CategoryId = 3,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 80.0,
                            Title = "Lungi"
                        },
                        new
                        {
                            Id = 6,
                            Barcode = "106",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 345.0,
                            Title = "Frock"
                        },
                        new
                        {
                            Id = 7,
                            Barcode = "107",
                            CategoryId = 3,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 800.0,
                            Title = "Lengha"
                        },
                        new
                        {
                            Id = 8,
                            Barcode = "108",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 120.0,
                            Title = "Gamcha"
                        },
                        new
                        {
                            Id = 9,
                            Barcode = "109",
                            CategoryId = 2,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 700.0,
                            Title = "Shoes"
                        },
                        new
                        {
                            Id = 10,
                            Barcode = "110",
                            CategoryId = 1,
                            CostPrice = 300.0,
                            Description = "",
                            Quantity = 1,
                            SellPrice = 50.0,
                            Title = "Cap"
                        });
                });

            modelBuilder.Entity("SmartBillingServer.Models.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

                    b.HasKey("Id");

                    b.ToTable("Bills");

                    b.HasData(
                        new
                        {
                            Id = 1,
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

            modelBuilder.Entity("Models.Models.Product", b =>
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
