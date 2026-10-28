using Models.Models;
using SmartBillingServer.Models;

namespace SmartBillingServer.Helper
{
    public static class Helper
    {
        // GST slabs shown as columns in the sale register.
        public static readonly double[] SaleReportTaxRates = [5, 12, 18];

        public static double Round2(double value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        public static double LineAmount(BillItem billItem) =>
            Round2(billItem.MRP * billItem.Quantity * (100 - billItem.DiscountPercentage) / 100);

        /// <summary>
        /// Works out every amount on the bill from its items and the bill discount, so the saved totals
        /// never depend on what the client sent.
        /// </summary>
        public static void Recalculate(this Bill bill)
        {
            foreach (var billItem in bill.BillItems)
            {
                billItem.Amount = LineAmount(billItem);
            }
            bill.SubTotal = Round2(bill.BillItems.Sum(x => x.Amount));
            bill.DiscountAmount = Round2(Math.Clamp(bill.DiscountAmount, 0, bill.SubTotal));
            bill.TotalAmount = Round2(bill.SubTotal - bill.DiscountAmount);
        }

        /// <summary>The part of the bill total paid for this line: its amount less its share of the bill discount.</summary>
        public static double NetAmount(this Bill bill, BillItem billItem) =>
            bill.SubTotal > 0 ? billItem.Amount * bill.TotalAmount / bill.SubTotal : 0;

        /// <summary>
        /// The GST rate the line was sold at. Bills created before rates were stored on the line fall back to
        /// the item's current rate; 0 when that is unknown (typed-in lines or deleted items).
        /// </summary>
        public static double TaxRate(this BillItem billItem, IReadOnlyDictionary<int, Item>? itemsById) =>
            billItem.Tax ?? (itemsById != null && itemsById.TryGetValue(billItem.ItemId, out var item) ? item.Tax : 0);

        public static string? HSNCode(this BillItem billItem, IReadOnlyDictionary<int, Item>? itemsById) =>
            billItem.HSNCode ?? (itemsById != null && itemsById.TryGetValue(billItem.ItemId, out var item) ? item.HSNCode : null);

        /// <summary>Taxable value of a tax-inclusive amount (prices are MRPs, which include GST).</summary>
        public static double TaxableValue(double amount, double rate) => amount * 100 / (100 + rate);

        /// <summary>CGST (= SGST) included in a tax-inclusive amount, rounded to paise.</summary>
        public static double HalfGst(double amount, double rate) => Round2((amount - TaxableValue(amount, rate)) / 2);

        public static string BillToCSV(this IEnumerable<Bill> bills, IReadOnlyDictionary<int, Item>? itemsById = null)
        {
            var content = "";

            // Adding Header
            content += "Date, Quantity, Amount, Net Value, CGST 2.5%, SGST 2.5%, Net Value, CGST 6%, SGST 6%, Net Value, CGST 9%, SGST 9%\n";

            foreach (var bill in bills)
            {
                // Date, Quantity, Amount, then for each slab: Net Value, CGST, SGST
                content += $"{bill.CreatedDateTime.ToString("MM/dd/yyyy")}, {bill.BillItems.Sum(x => x.Quantity)}, {Round2(bill.TotalAmount)}";
                foreach (var rate in SaleReportTaxRates)
                {
                    double amount = bill.BillItems.Where(x => x.TaxRate(itemsById) == rate).Sum(x => bill.NetAmount(x));
                    double netValue = TaxableValue(amount, rate);
                    double halfGst = HalfGst(amount, rate);
                    content += $", {Round2(netValue)}, {Round2(halfGst)}, {Round2(halfGst)}";
                }
                content += "\n";
            }

            return content;
        }

        public static string BillToSaleReport(this IEnumerable<Bill> bills, string month, int year, string shopName = "",
            IReadOnlyDictionary<int, Item>? itemsById = null)
        {
            var content = "";
            int startIndex = 7;
            int lastIndex = bills.Count() - 1 + startIndex;
            int i = 67; // ASCII value of C

            // Adding Details
            content += $"Location\n{shopName}\nSale register\nDate {month} {year}\n\n";
            content += bills.BillToCSV(itemsById);
            content += $",,=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})\n";

            return content;
        }
    }
}
