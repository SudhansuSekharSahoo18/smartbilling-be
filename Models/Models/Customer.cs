using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Customer : BaseModel
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        public double Due { get; set; }
    }
}
