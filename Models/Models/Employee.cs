using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Employee : BaseModel
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required bool IsAdmin { get; set; }
    }
}
