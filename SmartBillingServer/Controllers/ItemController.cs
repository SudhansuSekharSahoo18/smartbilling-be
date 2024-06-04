using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item> _repo;
        private readonly ILogger<Item> _logger;

        public ItemController(IRepository<Item> repo, ILogger<Item> logger)
        {
            _repo = repo;
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


        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _repo.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(item);
            }
            else
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
    }
}
