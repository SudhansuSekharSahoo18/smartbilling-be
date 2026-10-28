using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    /// <summary>
    /// Barcode label layout made in the Label Design page. The layout itself (label size and
    /// positioned elements) is owned by the client and stored as JSON.
    /// </summary>
    public class LabelDesign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public required string DesignJson { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
