namespace SmartBillingServer.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }

        public Item(int id, string barcode, string itemName, double price)
        {
            Id = id;
            Barcode = barcode;
            ItemName = itemName;
            Price = price;
        }
    }
}
