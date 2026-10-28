using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer.Controllers
{
    /// <summary>
    /// Sign-in for the whole app. Every other endpoint needs a signed-in user (see ServerHost).
    /// The session cookie is not persistent, so closing the app or the browser signs the user out.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutTime = TimeSpan.FromMinutes(1);
        private static readonly ConcurrentDictionary<string, (int Failures, DateTime LockedUntil)> FailedAttempts = new();

        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        // True until the first start has replaced the seeded password.
        private bool NeedsAdminPassword => !_db.Employees.Any(x => x.Password != Accounts.SeededPassword);

        [AllowAnonymous]
        [HttpGet("Status")]
        public IActionResult Status()
        {
            var signedIn = User.Identity?.IsAuthenticated == true;
            return Ok(new
            {
                signedIn,
                userName = signedIn ? User.Identity!.Name : null,
                isAdmin = signedIn && User.IsInRole(Accounts.AdminRole),
                needsAdminPassword = !signedIn && NeedsAdminPassword,
            });
        }

        // First start only: choose the admin's user name and password in place of System / 12345.
        [AllowAnonymous]
        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] LoginDto login)
        {
            if (!NeedsAdminPassword)
            {
                return Conflict("The admin password has already been set. Sign in instead.");
            }
            var error = Accounts.ValidateNewPassword(login.Password) ?? (string.IsNullOrWhiteSpace(login.UserName) ? "Enter a user name." : null);
            if (error != null)
            {
                return BadRequest(error);
            }

            var admin = _db.Employees.OrderBy(x => x.Id).FirstOrDefault(x => x.IsAdmin);
            if (admin == null)
            {
                admin = new Employee { Name = login.UserName.Trim(), UserName = "", PhoneNumber = "", Password = "", IsAdmin = true, CreatedById = 1, CreatedDateTime = DateTime.Now };
                _db.Employees.Add(admin);
            }
            admin.UserName = login.UserName.Trim();
            // Replace the seeded System account's placeholder details.
            if (admin.Name == "System")
            {
                admin.Name = admin.UserName;
            }
            if (admin.PhoneNumber == "1234567890")
            {
                admin.PhoneNumber = "";
            }
            admin.Password = Accounts.HashPassword(admin, login.Password);
            admin.UpdatedDateTime = DateTime.Now;
            _db.SaveChanges();

            await Accounts.SignInAsync(HttpContext, admin);
            return Ok(new { userName = admin.UserName, isAdmin = true });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var userName = login.UserName.Trim();
            var attemptKey = $"{userName.ToLowerInvariant()}|{HttpContext.Connection.RemoteIpAddress}";
            if (FailedAttempts.TryGetValue(attemptKey, out var attempts) && attempts.LockedUntil > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, "Too many wrong passwords. Wait a minute and try again.");
            }

            var employee = _db.Employees.AsEnumerable()
                .FirstOrDefault(x => string.Equals(x.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (employee == null || !Accounts.VerifyPassword(employee, login.Password, out var rehash))
            {
                var failures = attempts.Failures + 1;
                FailedAttempts[attemptKey] = failures >= MaxFailedAttempts ? (0, DateTime.UtcNow + LockoutTime) : (failures, DateTime.MinValue);
                return Unauthorized("Wrong user name or password.");
            }
            if (rehash)
            {
                employee.Password = Accounts.HashPassword(employee, login.Password);
                _db.SaveChanges();
            }

            FailedAttempts.TryRemove(attemptKey, out _);
            await Accounts.SignInAsync(HttpContext, employee);
            return Ok(new { userName = employee.UserName, isAdmin = employee.IsAdmin });
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto change)
        {
            var id = Accounts.EmployeeId(User);
            var employee = id == null ? null : _db.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return Unauthorized();
            }
            if (!Accounts.VerifyPassword(employee, change.CurrentPassword, out _))
            {
                return BadRequest("The current password is wrong.");
            }
            var error = Accounts.ValidateNewPassword(change.NewPassword);
            if (error != null)
            {
                return BadRequest(error);
            }

            employee.Password = Accounts.HashPassword(employee, change.NewPassword);
            employee.UpdatedById = employee.Id;
            employee.UpdatedDateTime = DateTime.Now;
            _db.SaveChanges();

            // Other sessions of this account end (their stamp no longer matches); this one continues.
            await Accounts.SignInAsync(HttpContext, employee);
            return NoContent();
        }
    }
}
