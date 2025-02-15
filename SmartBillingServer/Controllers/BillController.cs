using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Helper;
using SmartBillingServer.Models;
using System.Globalization;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _billRepo;
        private readonly ILogger<BillController> _logger;

        public BillController(IBillRepository db, ILogger<BillController> logger)
        {
            _billRepo = db;
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult<IEnumerable<Bill>> Get()
        {
            var objBillList = _billRepo.GetAll(includeProperties: "BillItems");
            return Ok(objBillList.OrderByDescending(x => x.Id));
        }

        [HttpGet("GetById/{id}")]
        public ActionResult<Bill> GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var bill = _billRepo.Get(x => x.Id == id, includeProperties: "BillItems");
            if (bill == null)
            {
                return NotFound();
            }

            return Ok(bill);
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
            return CreatedAtAction(nameof(GetById), new { id = bill.Id }, bill);
        }

        [HttpGet("GetTotalSaleByDate")]
        public ActionResult<double> GetTotalSaleByDate(string? date)
        {
            DateTime localDate = date == null ? DateTime.Now : DateTime.Parse(date);
            var bills = _billRepo.GetRange(x => x.CreatedDateTime.Date == localDate.Date);
            double todalAmount = bills.Sum(x => x.TotalAmount);

            return Ok(todalAmount);
        }

        [HttpGet("GenerateSaleReport")]
        public async Task<IActionResult> GetSaleReport(int month, int year)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            IEnumerable<Bill> bills = _billRepo.GetRange(x => x.CreatedDateTime.Year == year && x.CreatedDateTime.Month > (month - 1) && x.CreatedDateTime.Month <= month,
                includeProperties: "BillItems");
            if (!bills.Any())
            {
                bills = [];
            }

            var content = bills.BillToSaleReport(monthName, year);
            var fileName = "saleReport_" + monthName + "_" + year;
            Directory.CreateDirectory("temp");
            var filePath = $"./temp/{fileName}.csv";

            System.IO.File.WriteAllText(filePath, content);

            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("Filename is not provided.");
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
            // Delete file after sending
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
