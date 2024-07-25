using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBillingServer.Models
{
    public class Barcode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(30)]
        public required string ItemCode { get; set; }
        [MaxLength(30)]
        public required string ItemName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
  