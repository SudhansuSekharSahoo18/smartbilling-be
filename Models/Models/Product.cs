using SmartBillingServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Barcode { get; set; }
        [Required]
        [MaxLength(30)]
        public required string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double CostPrice { get; set; }
        [Required]
        public double SellPrice { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
