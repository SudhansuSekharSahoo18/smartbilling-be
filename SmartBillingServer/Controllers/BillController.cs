using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        List<Bill> bills = new()
        {
            new Bill(id: 1, subTotal: 100, discountAmount: 10, totalAmount: 90, modeOfPayment: "Cash"
                ,customerName: "Default", customerAddress: "", billItems: new List<BillItem>()),
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public BillController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Bill")]
        public IEnumerable<Bill> Get()
        {
            return bills;
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var bill = bills.FirstOrDefault(x=>x.Id == id);
            if(bill  == null)
            {
                return NotFound();
            }

            return Ok(bill);
        }


        [HttpPost(Name = "Create")]
        public IActionResult Create([FromBody] Bill bill)
        {
            if (bill == null)
            {
                return BadRequest();
            }

            bill.Id = bills.Count + 1;
            bills.Add(bill);
            return CreatedAtAction(nameof(GetById), new {id = bill.Id}, bill);

        }
    }
}
