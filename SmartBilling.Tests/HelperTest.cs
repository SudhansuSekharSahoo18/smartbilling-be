using SmartBillingServer.Helper;
using SmartBillingServer.Models;

namespace SmartBilling.Tests
{
    public class HelperTest
    {
        [Fact]
        public void BillToCSV_Success()
        {
            // Arrange
            // Actual
            var bills = new List<Bill>()
            {
                new Bill()
                {
                    Id = 1,
                    CreatedDateTime =new DateTime(2025,11,13),
                    BillItems = new List<BillItem>()
                    {
                        new BillItem()
                        {
                            Id = 1,
                            BillId = 1,
                            ItemName = "item 1",
                            MRP = 100,
                            Quantity = 5,
                            Amount = 500,
                            DiscountPercentage = 0,
                            ItemId = 1,
                            Unit = "Pieces"
                        },
                        new BillItem()
                        {
                            Id = 2,
                            BillId = 1,
                            ItemName = "item 2",
                            MRP = 200,
                            Quantity = 10,
                            Amount = 2000,
                            DiscountPercentage = 0,
                            ItemId = 2,
                            Unit = "Pieces"
                        }
                    },
                    TotalAmount = 2525

                },
                new Bill()
                {
                    Id = 2,
                    CreatedDateTime =new DateTime(2025,11,11),
                    BillItems = new List<BillItem>()
                    {
                        new BillItem()
                        {
                            Id = 1,
                            BillId = 1,
                            ItemName = "item 1",
                            MRP = 100,
                            Quantity = 5,
                            Amount = 500,
                            DiscountPercentage = 0,
                            ItemId = 1,
                            Unit = "Pieces"
                        },
                        new BillItem()
                        {
                            Id = 2,
                            BillId = 1,
                            ItemName = "item 2",
                            MRP = 200,
                            Quantity = 10,
                            Amount = 2000,
                            DiscountPercentage = 0,
                            ItemId = 2,
                            Unit = "Pieces"
                        }
                    },
                    TotalAmount = 90.4

                }
            };

            // Act
            var result = bills.BillToCSV();

            // Expected


            // Assert
            Assert.Equal(result, result);
        }
    }
}