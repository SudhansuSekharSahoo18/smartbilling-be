using SmartBillingServer.Models;

namespace SmartBillingServer.Helper
{
    public static class Helper
    {

        public static string BillToCSV(this IEnumerable<Bill> bills)
        {
            var content = "";

            // Adding Header
            content += "Date, Quantity, Amount, Net Value, CGST 2.5%, SGST 2.5%, Net Value, CGST 6%, SGST 6%, Net Value, CGST 9%, SGST 9%\n";

            foreach (var item in bills)
            {
                // Date, Quantity, Amount, Net Value, CGST 2.5%, SGST 2.5%, Net Value, CGST 6%, SGST 6%, Net Value, CGST 9%, SGST 9%, 
                content += $"{item.CreatedDateTime.ToString("MM/dd/yyyy")}, {item.BillItems.Sum(x => x.Quantity)}, {item.TotalAmount}, " +
                    // Add Tax based calculation 
                    $"{Math.Round(item.TotalAmount * 100 / 105, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 105) / 2, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 105) / 2, 2)}, " +
                    $"{Math.Round(item.TotalAmount * 100 / 112, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 112) / 2, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 112) / 2, 2)}, " +
                    $"{Math.Round(item.TotalAmount * 100 / 118, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 118) / 2, 2)}, {Math.Round((item.TotalAmount - item.TotalAmount * 100 / 118) / 2, 2)}\n";
            }

            return content;
        }

        public static string BillToSaleReport(this IEnumerable<Bill> bills, string month, int year)
        {
            var content = "";
            var shopName = "";
            int startIndex = 7;
            int lastIndex = bills.Count() - 1 + startIndex;
            int i = 67; // ASCII value of C

            // Adding Details
            content += $"Location\n{shopName}\nSale register\nDate {month} {year}\n\n";
            content += bills.BillToCSV();
            content += $",,=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})" +
                $",=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex}),=SUM({(char)i}{startIndex}:{(char)i++}{lastIndex})\n";

            return content;
        }
    }
}
