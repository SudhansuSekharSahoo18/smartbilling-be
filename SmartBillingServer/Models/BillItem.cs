namespace SmartBillingServer.Models
{
    public class BillItem
    {
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double DiscountAmount { get; set; }
        public double Amount { get; set; }

        public BillItem(int itemId, string barcode, string itemName, int quantity, double price, double discountAmount, double amount)
        {
            ItemId = itemId;
            Barcode = barcode;
            ItemName = itemName;
            Quantity = quantity;
            Price = price;
            DiscountAmount = discountAmount;
            Amount = amount;
        }

    }
}
