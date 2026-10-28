using System.ComponentModel.DataAnnotations;

namespace Models.Dtos
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = "";
        [Required]
        public string NewPassword { get; set; } = "";
    }
}
