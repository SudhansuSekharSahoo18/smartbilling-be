using System.Globalization;
using ClosedXML.Excel;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Helper;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillRepository _billRepo;
        private readonly IItemRepository _itemRepo;
        private readonly ILogger<BillController> _logger;

        public BillController(IBillRepository db, ILogger<BillController> logger, IItemRepository itemRepo)
        {
            _billRepo = db;
            _logger = logger;
            _itemRepo = itemRepo;
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

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid ID provided.");
            }

            var bill = _billRepo.Get(x => x.Id == id, includeProperties: "BillItems");
            if (bill == null)
            {
                return NotFound($"Bill with ID {id} not found.");
            }

            try
            {
                _billRepo.Remove(bill);
                _logger.LogInformation($"Bill with ID {id} deleted successfully.");
                return Ok(new { message = $"Bill with ID {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting bill with ID {id}");
                return StatusCode(500, "An error occurred while deleting the bill.");
            }
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

        [HttpGet("GetTaxSummaryReport")]
        public IActionResult GetTaxSummaryReport(int? month, int? year)
        {
            int targetMonth = month ?? DateTime.Now.Month;
            int targetYear = year ?? DateTime.Now.Year;

            var bills = _billRepo.GetRange(
                x => x.CreatedDateTime.Year == targetYear && x.CreatedDateTime.Month == targetMonth,
                includeProperties: "BillItems");

            var itemDict = _itemRepo.GetAll().ToDictionary(i => i.Id);

            var grouped = bills
                .SelectMany(b => b.BillItems)
                .Where(bi => itemDict.ContainsKey(bi.ItemId) && itemDict[bi.ItemId].Tax > 0)
                .GroupBy(bi => new { itemDict[bi.ItemId].HSNCode, itemDict[bi.ItemId].Tax })
                .Select(g =>
                {
                    var assessableValue = Math.Round(g.Sum(bi => bi.Amount) / (1.0 + g.Key.Tax / 100.0), 2);
                    return new
                    {
                        HSNCode = g.Key.HSNCode,
                        TaxRate = g.Key.Tax,
                        TotalQuantity = g.Sum(bi => bi.Quantity),
                        TotalAmount = Math.Round(g.Sum(bi => bi.Amount), 2),
                        AssessableValue = assessableValue,
                        CGST = Math.Round(g.Key.Tax / 200.0 * assessableValue, 2),
                        SGST = Math.Round(g.Key.Tax / 200.0 * assessableValue, 2),
                    };
                })
                .OrderBy(x => x.HSNCode)
                .ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tax Summary");

            // Header row
            var headers = new[] { "HSN Code", "Tax Rate (%)", "Total Quantity", "Total Amount (₹)", "Assessable Value (₹)", "CGST (₹)", "SGST (₹)" };
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(1, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Data rows
            for (int r = 0; r < grouped.Count; r++)
            {
                var row = grouped[r];
                ws.Cell(r + 2, 1).Value = row.HSNCode;
                ws.Cell(r + 2, 2).Value = row.TaxRate;
                ws.Cell(r + 2, 3).Value = row.TotalQuantity;
                ws.Cell(r + 2, 4).Value = row.TotalAmount;
                ws.Cell(r + 2, 5).Value = row.AssessableValue;
                ws.Cell(r + 2, 6).Value = row.CGST;
                ws.Cell(r + 2, 7).Value = row.SGST;
            }

            ws.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(targetMonth);
            var filename = $"TaxSummary_{monthName}_{targetYear}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        [HttpGet("GetDailySales")]
        public ActionResult<IEnumerable<object>> GetDailySales(int? month, int? year)
        {
            int targetMonth = month ?? DateTime.Now.Month;
            int targetYear = year ?? DateTime.Now.Year;
            int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);

            var bills = _billRepo.GetRange(x =>
                x.CreatedDateTime.Year == targetYear && x.CreatedDateTime.Month == targetMonth);

            var dailySales = Enumerable.Range(1, daysInMonth).Select(day => new
            {
                day,
                totalSale = bills.Where(b => b.CreatedDateTime.Day == day).Sum(b => b.TotalAmount)
            }).ToList();

            return Ok(dailySales);
        }

        [HttpGet("GetMonthlySales")]
        public ActionResult<IEnumerable<object>> GetMonthlySales(int? year)
        {
            // Financial year starts in April; default to current FY
            int fyStartYear = year ?? (DateTime.Now.Month >= 4 ? DateTime.Now.Year : DateTime.Now.Year - 1);
            int fyEndYear = fyStartYear + 1;

            var bills = _billRepo.GetRange(x =>
                (x.CreatedDateTime.Year == fyStartYear && x.CreatedDateTime.Month >= 4) ||
                (x.CreatedDateTime.Year == fyEndYear && x.CreatedDateTime.Month <= 3));

            // Apr → Mar order
            var fyMonths = new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3 };

            var monthlySales = fyMonths.Select(month => new
            {
                month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                totalSale = bills.Where(b => b.CreatedDateTime.Month == month).Sum(b => b.TotalAmount)
            });

            return Ok(monthlySales);
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
