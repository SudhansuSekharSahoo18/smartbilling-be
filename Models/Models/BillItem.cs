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
        // Copied from the item when the bill is created, so tax reports don't change when the item is
        // edited or deleted later. Null on bills created before these were stored.
        public string? HSNCode { get; set; }
        public double? Tax { get; set; }
        //[Required]
        public string? Unit { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double MRP { get; set; }
        //public double SellPrice { get; set; }
        public double DiscountPercentage { get; set; }
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
