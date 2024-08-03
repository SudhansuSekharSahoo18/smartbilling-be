using SmartBillingServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    public class Item : BaseModel
    {
        public required string Barcode { get; set; }
        [Required]
        [MaxLength(30)]
        public required string ItemName { get; set; }
        public string? Description { get; set; }
        public string? HSNCode { get; set; }
        [Required]
        public required string Unit { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double CostPrice { get; set; }
        //[Required]
        //public double SellPrice { get; set; }
        [Required]
        public double MRP { get; set; }
        [Range(0, 100)]
        public double DiscountPercentage { get; set; }
        //public double DiscountAmount { get; set; }
        [Range(0, 100)]
        public double Tax { get; set; }
        public bool IsTaxInclusive { get; set; } = true;
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; } 
    }
}
