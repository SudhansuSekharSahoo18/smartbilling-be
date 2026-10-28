using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer
{
    /// <summary>
    /// Passwords and sign-in sessions for employee accounts (AuthController, EmployeeController).
    /// </summary>
    public static class Accounts
    {
        public const string AdminRole = "Admin";
        public const string StaffRole = "Staff";
        // Password of the seeded System account. Nobody can sign in with it: the first start asks for a real one.
        public const string SeededPassword = "12345";
        public const int MinPasswordLength = 6;

        // Changes whenever the password changes, so sessions signed in with an old password end.
        private const string StampClaim = "stamp";

        private static readonly PasswordHasher<Employee> Hasher = new();

        public static string HashPassword(Employee employee, string password) => Hasher.HashPassword(employee, password);

        public static string? ValidateNewPassword(string? password) =>
            string.IsNullOrEmpty(password) || password.Length < MinPasswordLength
                ? $"The password must have at least {MinPasswordLength} characters."
                : password == SeededPassword ? "Choose a password other than 12345." : null;

        /// <summary>
        /// Checks a password against the stored hash. True with rehash = true when the stored value must be
        /// replaced by a new hash (passwords saved as plain text by older versions). The seeded 12345 never matches.
        /// </summary>
        public static bool VerifyPassword(Employee employee, string? password, out bool rehash)
        {
            rehash = false;
            if (string.IsNullOrEmpty(password) || employee.Password == SeededPassword)
            {
                return false;
            }

            PasswordVerificationResult result;
            try
            {
                result = Hasher.VerifyHashedPassword(employee, employee.Password, password);
            }
            catch (FormatException)
            {
                result = PasswordVerificationResult.Failed;
            }

            if (result == PasswordVerificationResult.Failed)
            {
                // A real hash (version 3 format, "AQAAAA…") must never be matched as plain text.
                if (employee.Password.StartsWith("AQAAAA") || employee.Password != password)
                {
                    return false;
                }
                result = PasswordVerificationResult.SuccessRehashNeeded;
            }
            rehash = result == PasswordVerificationResult.SuccessRehashNeeded;
            return true;
        }

        public static int? EmployeeId(ClaimsPrincipal user) =>
            int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        public static Task SignInAsync(HttpContext context, Employee employee) =>
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreatePrincipal(employee),
                new AuthenticationProperties { IsPersistent = false });

        /// <summary>
        /// Runs on every signed-in request: ends the session when the account was deleted or its password
        /// changed, and picks up changes to the name or admin rights without signing in again.
        /// </summary>
        public static async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context)
        {
            var id = context.Principal == null ? null : EmployeeId(context.Principal);
            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var employee = id == null ? null : await db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null || context.Principal!.FindFirstValue(StampClaim) != Stamp(employee))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            if (context.Principal!.Identity?.Name != employee.UserName || context.Principal.IsInRole(AdminRole) != employee.IsAdmin)
            {
                context.ReplacePrincipal(CreatePrincipal(employee));
                context.ShouldRenew = true;
            }
        }

        private static ClaimsPrincipal CreatePrincipal(Employee employee) => new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Name, employee.UserName),
            new Claim(ClaimTypes.Role, employee.IsAdmin ? AdminRole : StaffRole),
            new Claim(StampClaim, Stamp(employee)),
        ], CookieAuthenticationDefaults.AuthenticationScheme));

        private static string Stamp(Employee employee) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(employee.Password)))[..16];
    }
}
