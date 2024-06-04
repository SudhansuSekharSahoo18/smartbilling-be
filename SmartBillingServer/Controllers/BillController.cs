using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _billRepo;
        private readonly ILogger<WeatherForecastController> _logger;

        public BillController(IBillRepository db, ILogger<WeatherForecastController> logger)
        {
            _billRepo = db;
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult<IEnumerable<Bill>> Get()
        {
            var objBillList = _billRepo.GetAll(includeProperties: "BillItems");
            return Ok(objBillList);
        }


        [HttpGet("GetById/{id}")]
        public ActionResult<Bill> GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _billRepo.Get(x => x.Id == id, includeProperties: "BillItems");
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] Bill bill)
        {
            if (bill == null)
            {
                return BadRequest();
            }

            bill.CreatedDateTime = DateTime.Now;
            _billRepo.Add(bill);
            return CreatedAtAction(nameof(GetById), new {id = bill.Id}, bill);
        }   
    }
}
