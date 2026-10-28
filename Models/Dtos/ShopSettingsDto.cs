using System.ComponentModel.DataAnnotations;

namespace Models.Dtos
{
    public class ShopSettingsDto
    {
        [Required]
        [StringLength(200)]
        public required string ShopName { get; set; }
        [StringLength(500)]
        public string? ShopAddress { get; set; }
        [StringLength(20)]
        public string? ShopGSTNumber { get; set; }
        [StringLength(500)]
        public string? BarcodeFilePath { get; set; }
    }
}
