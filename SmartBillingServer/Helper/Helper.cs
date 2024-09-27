using SmartBillingServer.Models;

namespace SmartBillingServer.Helper
{
    public static class Helper
    {

        public static string BillToCSV(this IEnumerable<Bill> bills)
        {
            var content = "";

            foreach (var item in bills)
            {
                // Date, Quantity, Amount, Net Value, CGST 2.5%, SGST 2.5%, Net Value, CGST 6%, SGST 6%, Net Value, CGST 9%, SGST 9%, 
                content += $"{item.CreatedDateTime.ToLongDateString()}, {item.BillItems.Sum(x => x.Quantity)}, {item.TotalAmount}, " +
                    // Add Tax based calculation 
                    $"{Math.Round(item.TotalAmount * 100 / 105, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 105, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 105, 2)}, " +
                    $"{Math.Round(item.TotalAmount * 100 / 112, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 112, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 112, 2)}, " +
                    $"{Math.Round(item.TotalAmount * 100 / 118, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 118, 2)}, {Math.Round(item.TotalAmount - item.TotalAmount * 100 / 118, 2)}\n";
            }

            return content;
        }
    }
}
