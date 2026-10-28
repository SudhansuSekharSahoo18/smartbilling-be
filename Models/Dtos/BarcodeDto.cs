using SmartBillingServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class BarcodeDto
    {
        // Optional: when empty, the path saved in Setting (or the default) is used.
        public string? FilePath { get; set; }
        [Required]
        public required List<Barcode> BarcodeList { get; set; }
    }
}
