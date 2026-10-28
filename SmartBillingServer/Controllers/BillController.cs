using System.Globalization;
using ClosedXML.Excel;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;
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
        private readonly IApplicationConfigurationRepository _appConfigRepo;
        private readonly ILogger<BillController> _logger;

        public BillController(IBillRepository db, ILogger<BillController> logger, IItemRepository itemRepo, IApplicationConfigurationRepository appConfigRepo)
        {
            _billRepo = db;
            _logger = logger;
            _itemRepo = itemRepo;
            _appConfigRepo = appConfigRepo;
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
                return BadRequest("No bill was sent.");
            }

            var error = ValidateBillItems(bill);
            if (error != null)
            {
                return BadRequest(error);
            }
            if (bill.DiscountAmount < 0)
            {
                return BadRequest("The bill discount can't be negative.");
            }

            bill.Id = 0;
            foreach (var billItem in bill.BillItems)
            {
                billItem.Id = 0;
                billItem.BillId = 0;
                // Lines typed in by hand have no item (ItemId 0), so there is no stock or tax rate to use.
                var item = billItem.ItemId > 0 ? _itemRepo.Get(x => x.Id == billItem.ItemId) : null;
                if (item == null)
                {
                    billItem.ItemId = 0;
                    continue;
                }
                billItem.Barcode = item.Barcode;
                billItem.HSNCode = item.HSNCode;
                billItem.Tax = item.Tax;
                billItem.Unit ??= item.Unit;
                item.Quantity -= billItem.Quantity;
            }

            var requestedDiscount = bill.DiscountAmount;
            bill.Recalculate();
            if (requestedDiscount > bill.SubTotal)
            {
                return BadRequest($"The bill discount (₹{requestedDiscount}) is more than the bill amount (₹{bill.SubTotal}).");
            }
            bill.CreatedDateTime = DateTime.Now;
            // The items above were loaded through the same DbContext, so this one save also stores the
            // lower stock: the bill and the stock change are saved together or not at all.
            _billRepo.Add(bill);
            return CreatedAtAction(nameof(GetById), new { id = bill.Id }, bill);
        }

        /// <summary>
        /// Takes one unit of an item off a bill (a customer return): the bill's amounts are worked out again
        /// and the unit goes back into stock.
        /// </summary>
        [HttpPost("ReturnItem")]
        public IActionResult ReturnItem([FromBody] ReturnItemDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Barcode))
            {
                return BadRequest("Scan or type the barcode of the item being returned.");
            }
            var bill = _billRepo.Get(x => x.Id == dto.BillId, includeProperties: "BillItems");
            if (bill == null)
            {
                return NotFound($"Bill {dto.BillId} was not found.");
            }

            var barcode = dto.Barcode.Trim();
            // Bills created before barcodes were stored on the line are matched through the item.
            var item = _itemRepo.Get(x => x.Barcode == barcode);
            var billItem = bill.BillItems.FirstOrDefault(x => x.Barcode == barcode)
                ?? (item != null ? bill.BillItems.FirstOrDefault(x => x.Barcode == null && x.ItemId == item.Id) : null);
            if (billItem == null)
            {
                return NotFound($"Barcode {barcode} is not on bill {bill.Id}.");
            }

            billItem.Quantity -= 1;
            if (billItem.Quantity <= 0)
            {
                bill.BillItems.Remove(billItem);
            }
            if (billItem.ItemId > 0)
            {
                var stockItem = _itemRepo.Get(x => x.Id == billItem.ItemId);
                if (stockItem != null)
                {
                    stockItem.Quantity += 1;
                }
            }
            bill.Recalculate();
            _billRepo.Save();

            return Ok(bill);
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
                // The sold items go back into stock (saved together with the delete).
                foreach (var billItem in bill.BillItems.Where(x => x.ItemId > 0))
                {
                    var item = _itemRepo.Get(x => x.Id == billItem.ItemId);
                    if (item != null)
                    {
                        item.Quantity += billItem.Quantity;
                    }
                }
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

        private static string? ValidateBillItems(Bill bill)
        {
            if (bill.BillItems == null || bill.BillItems.Count == 0)
            {
                return "Add at least one item to the bill.";
            }
            foreach (var billItem in bill.BillItems)
            {
                var name = string.IsNullOrWhiteSpace(billItem.ItemName) ? "An item" : billItem.ItemName;
                if (billItem.Quantity <= 0)
                {
                    return $"{name}: the quantity must be at least 1.";
                }
                if (billItem.MRP < 0 || double.IsNaN(billItem.MRP))
                {
                    return $"{name}: the price can't be negative.";
                }
                if (billItem.DiscountPercentage < 0 || billItem.DiscountPercentage > 100)
                {
                    return $"{name}: the discount must be between 0 and 100%.";
                }
            }
            return null;
        }

        private static string? ValidateMonthYear(int? month, int? year)
        {
            if (month is < 1 or > 12)
            {
                return "Month must be between 1 and 12.";
            }
            if (year is < 2000 or > 2100)
            {
                return "Year must be between 2000 and 2100.";
            }
            return null;
        }

        private Dictionary<int, Item> ItemsById() => _itemRepo.GetAll().ToDictionary(i => i.Id);

        [HttpGet("GetTotalSaleByDate")]
        public ActionResult<double> GetTotalSaleByDate(string? date)
        {
            DateTime localDate = DateTime.Now;
            if (date != null && !DateTime.TryParse(date, CultureInfo.InvariantCulture, out localDate))
            {
                return BadRequest($"\"{date}\" is not a valid date. Use yyyy-MM-dd.");
            }
            var bills = _billRepo.GetRange(x => x.CreatedDateTime.Date == localDate.Date);
            double todalAmount = bills.Sum(x => x.TotalAmount);

            return Ok(todalAmount);
        }

        [HttpGet("GenerateSaleReport")]
        public IActionResult GetSaleReport(int month, int year)
        {
            var error = ValidateMonthYear(month, year);
            if (error != null)
            {
                return BadRequest(error);
            }
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            IEnumerable<Bill> bills = _billRepo.GetRange(x => x.CreatedDateTime.Year == year && x.CreatedDateTime.Month == month,
                includeProperties: "BillItems").OrderBy(x => x.CreatedDateTime);

            var shopName = _appConfigRepo.Get(x => x.Id == 1)?.ShopName ?? "";
            var content = bills.BillToSaleReport(monthName, year, shopName, ItemsById());
            var fileName = $"saleReport_{monthName}_{year}.csv";

            return File(System.Text.Encoding.UTF8.GetBytes(content), GetContentType(fileName), fileName);
        }

        [HttpGet("GetTaxSummaryReport")]
        public IActionResult GetTaxSummaryReport(int? month, int? year)
        {
            var error = ValidateMonthYear(month, year);
            if (error != null)
            {
                return BadRequest(error);
            }
            int targetMonth = month ?? DateTime.Now.Month;
            int targetYear = year ?? DateTime.Now.Year;

            var bills = _billRepo.GetRange(
                x => x.CreatedDateTime.Year == targetYear && x.CreatedDateTime.Month == targetMonth,
                includeProperties: "BillItems");

            // Lines use the HSN code and rate stored when they were sold; older bills fall back to the item.
            var itemDict = ItemsById();

            var grouped = bills
                .SelectMany(b => b.BillItems.Select(bi => new
                {
                    HSNCode = bi.HSNCode(itemDict) ?? "",
                    Tax = bi.TaxRate(itemDict),
                    bi.Quantity,
                    // After the bill discount, which lowers the value GST is charged on.
                    Amount = b.NetAmount(bi),
                }))
                .Where(bi => bi.Tax > 0)
                .GroupBy(bi => new { bi.HSNCode, bi.Tax })
                .Select(g =>
                {
                    var totalAmount = g.Sum(bi => bi.Amount);
                    var assessableValue = Helper.Helper.Round2(Helper.Helper.TaxableValue(totalAmount, g.Key.Tax));
                    var halfGst = Helper.Helper.HalfGst(totalAmount, g.Key.Tax);
                    return new
                    {
                        HSNCode = g.Key.HSNCode,
                        TaxRate = g.Key.Tax,
                        TotalQuantity = g.Sum(bi => bi.Quantity),
                        TotalAmount = Helper.Helper.Round2(totalAmount),
                        AssessableValue = assessableValue,
                        CGST = halfGst,
                        SGST = halfGst,
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
            var error = ValidateMonthYear(month, year);
            if (error != null)
            {
                return BadRequest(error);
            }
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
            var error = ValidateMonthYear(null, year);
            if (error != null)
            {
                return BadRequest(error);
            }
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
