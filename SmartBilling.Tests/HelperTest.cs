using Models.Models;
using SmartBillingServer.Helper;
using SmartBillingServer.Models;

namespace SmartBilling.Tests
{
    public class HelperTest
    {
        private static BillItem Line(double mrp, int quantity, double? tax, double discount = 0, int itemId = 0) => new()
        {
            ItemName = "item",
            MRP = mrp,
            Quantity = quantity,
            DiscountPercentage = discount,
            Tax = tax,
            ItemId = itemId,
        };

        [Fact]
        public void Recalculate_IgnoresClientTotalsAndAppliesBillDiscount()
        {
            var bill = new Bill
            {
                SubTotal = 1,
                TotalAmount = 1,
                DiscountAmount = 100,
                BillItems = [Line(1299, 1, 5, discount: 7), Line(499.5, 2, 12)],
            };

            bill.Recalculate();

            Assert.Equal(1208.07, bill.BillItems[0].Amount);
            Assert.Equal(999, bill.BillItems[1].Amount);
            Assert.Equal(2207.07, bill.SubTotal);
            Assert.Equal(100, bill.DiscountAmount);
            Assert.Equal(2107.07, bill.TotalAmount);
        }

        [Fact]
        public void Recalculate_CapsDiscountAtSubTotal()
        {
            var bill = new Bill { DiscountAmount = 500, BillItems = [Line(100, 1, 5)] };

            bill.Recalculate();

            Assert.Equal(100, bill.DiscountAmount);
            Assert.Equal(0, bill.TotalAmount);
        }

        [Fact]
        public void BillToCSV_SplitsGstByEachLinesRate()
        {
            var bill = new Bill
            {
                CreatedDateTime = new DateTime(2025, 11, 13),
                DiscountAmount = 164, // 10%: every line is lowered by the same share
                BillItems = [Line(1050, 1, 5), Line(590, 1, 18)],
            };
            bill.Recalculate();

            var lines = new[] { bill }.BillToCSV().Split('\n', StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(2, lines.Length);
            var columns = lines[1].Split(',').Select(x => x.Trim()).ToArray();
            // Date, Quantity, Amount
            Assert.Equal(["11/13/2025", "2", "1476"], columns[..3]);
            // 5%: 945 incl. GST → 900 + 22.5 + 22.5
            Assert.Equal(["900", "22.5", "22.5"], columns[3..6]);
            // 12%: nothing sold
            Assert.Equal(["0", "0", "0"], columns[6..9]);
            // 18%: 531 incl. GST → 450 + 40.5 + 40.5
            Assert.Equal(["450", "40.5", "40.5"], columns[9..12]);
        }

        [Fact]
        public void TaxRate_UsesRateStoredOnLineThenFallsBackToItem()
        {
            var items = new Dictionary<int, Item>
            {
                [7] = new Item { Id = 7, Barcode = "101", ItemName = "Shirt", HSNCode = "6205", Unit = "Pieces", Tax = 12 },
            };

            Assert.Equal(5, Line(100, 1, tax: 5, itemId: 7).TaxRate(items));
            Assert.Equal(12, Line(100, 1, tax: null, itemId: 7).TaxRate(items));
            Assert.Equal(0, Line(100, 1, tax: null, itemId: 99).TaxRate(items));
        }
    }
}
