using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer.Controllers
{
    /// <summary>
    /// Staff accounts (Staff page). Only admins can see and change them. There is always at least one admin.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Accounts.AdminRole)]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_db.Employees.OrderBy(x => x.Name).AsEnumerable().Select(ToDto));
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] EmployeeDto dto)
        {
            var error = ValidateDetails(dto) ?? Accounts.ValidateNewPassword(dto.Password);
            if (error != null)
            {
                return BadRequest(error);
            }
            if (UserNameTaken(dto.UserName, 0))
            {
                return Conflict($"The user name \"{dto.UserName.Trim()}\" is already used.");
            }

            var employee = new Employee
            {
                Name = dto.Name.Trim(),
                UserName = dto.UserName.Trim(),
                PhoneNumber = dto.PhoneNumber?.Trim() ?? "",
                IsAdmin = dto.IsAdmin,
                Password = "",
                CreatedById = Accounts.EmployeeId(User) ?? 1,
                CreatedDateTime = DateTime.Now,
            };
            employee.Password = Accounts.HashPassword(employee, dto.Password!);
            _db.Employees.Add(employee);
            _db.SaveChanges();
            return Ok(ToDto(employee));
        }

        // Changes the details and admin rights; the password is changed with ResetPassword.
        [HttpPost("Update")]
        public IActionResult Update([FromBody] EmployeeDto dto)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == dto.Id);
            if (employee == null)
            {
                return NotFound("This account no longer exists.");
            }
            var error = ValidateDetails(dto);
            if (error != null)
            {
                return BadRequest(error);
            }
            if (UserNameTaken(dto.UserName, dto.Id))
            {
                return Conflict($"The user name \"{dto.UserName.Trim()}\" is already used.");
            }
            if (employee.IsAdmin && !dto.IsAdmin && IsLastAdmin(employee))
            {
                return BadRequest($"{employee.Name} is the only admin. Make another account an admin first.");
            }

            employee.Name = dto.Name.Trim();
            employee.UserName = dto.UserName.Trim();
            employee.PhoneNumber = dto.PhoneNumber?.Trim() ?? "";
            employee.IsAdmin = dto.IsAdmin;
            employee.UpdatedById = Accounts.EmployeeId(User);
            employee.UpdatedDateTime = DateTime.Now;
            _db.SaveChanges();
            return Ok(ToDto(employee));
        }

        // Sets a new password, for example when someone has forgotten theirs. Their open sessions end.
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == dto.Id);
            if (employee == null)
            {
                return NotFound("This account no longer exists.");
            }
            var error = Accounts.ValidateNewPassword(dto.Password);
            if (error != null)
            {
                return BadRequest(error);
            }
            if (employee.Id == Accounts.EmployeeId(User))
            {
                return BadRequest("Change your own password on the Setting page.");
            }

            employee.Password = Accounts.HashPassword(employee, dto.Password);
            employee.UpdatedById = Accounts.EmployeeId(User);
            employee.UpdatedDateTime = DateTime.Now;
            _db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return NotFound("This account no longer exists.");
            }
            if (employee.Id == Accounts.EmployeeId(User))
            {
                return BadRequest("You can't delete your own account.");
            }
            if (employee.IsAdmin && IsLastAdmin(employee))
            {
                return BadRequest($"{employee.Name} is the only admin and can't be deleted.");
            }

            _db.Employees.Remove(employee);
            _db.SaveChanges();
            return NoContent();
        }

        private static EmployeeDto ToDto(Employee x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            UserName = x.UserName,
            PhoneNumber = x.PhoneNumber,
            IsAdmin = x.IsAdmin,
        };

        private static string? ValidateDetails(EmployeeDto dto) =>
            string.IsNullOrWhiteSpace(dto.Name) ? "Enter the name."
            : string.IsNullOrWhiteSpace(dto.UserName) ? "Enter a user name."
            : dto.UserName.Trim().Length > 100 || dto.Name.Trim().Length > 100 ? "The name and user name can have at most 100 characters."
            : null;

        private bool UserNameTaken(string userName, int exceptId) =>
            _db.Employees.AsEnumerable().Any(x => x.Id != exceptId && string.Equals(x.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase));

        private bool IsLastAdmin(Employee employee) => !_db.Employees.Any(x => x.IsAdmin && x.Id != employee.Id);
    }
}
