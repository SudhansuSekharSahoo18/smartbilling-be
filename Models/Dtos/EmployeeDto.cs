using System.Text.Json.Serialization;

namespace Models.Dtos
{
    // A staff account as shown and edited on the Staff page. Password is only used when creating an account.
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string UserName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }
    }

    public class ResetPasswordDto
    {
        public int Id { get; set; }
        public string Password { get; set; } = "";
    }
}
