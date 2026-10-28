using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public HealthController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Public, so the desktop app can check that the server is up before anyone signs in.
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { healthy = true, message = "API is running." });
        }

        [HttpGet("db")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var canConnect = _db.Database.CanConnect();
                if (canConnect)
                    return Ok(new { healthy = true, message = "Database connection is successful." });

                return StatusCode(503, new { healthy = false, message = "Database is configured but could not connect." });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { healthy = false, message = $"Database error: {ex.Message}" });
            }
        }

        [HttpGet("config")]
        public IActionResult CheckConfig()
        {
            var issues = new List<string>();

            try
            {
                var config = _db.Set<ApplicationConfiguration>().AsNoTracking().FirstOrDefault(x => x.Id == 1);
                if (string.IsNullOrWhiteSpace(config?.ShopName))
                    issues.Add("Shop name is not set. Enter the shop details on the Setting page.");
            }
            catch (Exception ex)
            {
                issues.Add($"Could not read the shop settings: {ex.Message}");
            }

            if (issues.Count > 0)
                return StatusCode(503, new { healthy = false, message = string.Join(" ", issues) });

            return Ok(new { healthy = true, message = $"Shop settings are set. Database: {_db.Database.GetDbConnection().DataSource}" });
        }
    }
}
