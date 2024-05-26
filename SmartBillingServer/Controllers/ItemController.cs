using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public ItemController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Items")]
        public IEnumerable<Item> Get()
        {
            List<Item> items =
            [
                new Item(1, "101", "Saree", 500),
                new Item(2, "102", "Jeans", 1500),
                new Item(3, "103", "Shirt", 400),
                new Item(4, "104", "Socks", 150),
                new Item(5, "105", "Lungi", 80),
                new Item(5, "106", "Frock", 345),
                new Item(5, "107", "Gamcha", 120),
                new Item(5, "108", "Lengha", 800),
                new Item(5, "109", "Shoes", 700),
                new Item(5, "110", "Cap", 50),
            ];

            return items;
        }
    }
}
