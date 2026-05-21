using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.DataAccess.Data;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public HealthController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

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

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                issues.Add("ConnectionStrings.DefaultConnection is missing.");

            var barcodeFilePath = _configuration["ConfigSettings:BarcodeGenerationFilePath"];
            if (barcodeFilePath == null)
                issues.Add("ConfigSettings.BarcodeGenerationFilePath is missing.");

            if (issues.Count > 0)
                return StatusCode(503, new { healthy = false, message = string.Join(" ", issues) });

            return Ok(new { healthy = true, message = "All required configuration values are present." });
        }
    }
}
