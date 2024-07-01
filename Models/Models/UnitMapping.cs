using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBillingServer.Models
{
    public class UnitMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Pieces { get; set; }
        public int Boxes { get; set; }
        public int Meters { get; set; }
    }
}
