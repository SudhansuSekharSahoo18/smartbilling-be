using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Models
{
    public class ApplicationConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Barcode { get; set; }
        public string? Version { get; set; }

        // Shop details printed on bills and labels, entered in the app's first-run setup / Setting page.
        public string? ShopName { get; set; }
        public string? ShopAddress { get; set; }
        public string? ShopGSTNumber { get; set; }
        // Full path of the CSV written by Barcode > Export. Empty means the default under Documents\SmartBilling.
        public string? BarcodeFilePath { get; set; }
    }
}
