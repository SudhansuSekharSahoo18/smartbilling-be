using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBillingServer.Models
{
    public class BillItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int BillId { get; set; }
        public string? Barcode { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double DiscountAmount { get; set; }
        public double Amount { get; set; }

        //public BillItem(int itemId, string barcode, string itemName, int quantity, double price, double discountAmount, double amount)
        //{
        //    ItemId = itemId;
        //    Barcode = barcode;
        //    ItemName = itemName;
        //    Quantity = quantity;
        //    Price = price;
        //    DiscountAmount = discountAmount;
        //    Amount = amount;
        //}

    }
}
