namespace SmartBillingServer.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public double SubTotal { get; set; }
        public double DiscountAmount { get; set; }
        public double TotalAmount { get; set; }
        public string ModeOfPayment { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public List<BillItem> BillItems { get; set; } = new List<BillItem>();

        public Bill(int id, double subTotal, double discountAmount, double totalAmount, string modeOfPayment, 
            string customerName, string customerAddress, List<BillItem> billItems)
        {
            Id = id;
            SubTotal = subTotal;
            DiscountAmount = discountAmount;
            TotalAmount = totalAmount;
            ModeOfPayment = modeOfPayment;
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            BillItems = billItems;
        }

        public Bill()
        {
            Id = 0;
            SubTotal = 0;
            DiscountAmount = 0;
            TotalAmount = 0;
            ModeOfPayment = "Cash";
            CustomerName = "default";
            CustomerAddress = "NA";
            BillItems = new List<BillItem>();
        }
    }
}
