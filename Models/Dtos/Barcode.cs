using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class Barcode
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
