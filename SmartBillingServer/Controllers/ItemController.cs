using ClosedXML.Excel;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepo;
        private readonly IBarcodeRepository _barcodeRepo;
        private readonly IApplicationConfigurationRepository _appConfigRepo;
        private readonly ILogger<ItemController> _logger;

        // Columns of the Export file; Import reads the same headers (in any order).
        private static readonly string[] ItemColumns =
            ["Barcode", "Item name", "HSN code", "Unit", "Quantity", "Cost price", "MRP", "Discount %", "Tax %", "Tax inclusive", "Description"];

        public ItemController(IItemRepository db, ILogger<ItemController> logger, IApplicationConfigurationRepository appConfigRepo,
            IBarcodeRepository barcodeRepo)
        {
            _itemRepo = db;
            _logger = logger;
            _appConfigRepo = appConfigRepo;
            _barcodeRepo = barcodeRepo;
        }

        [HttpGet("Get")]
        public IEnumerable<Item> Get()
        {
            var objCategoryList = _itemRepo.GetAll();
            return objCategoryList;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _itemRepo.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] Item product)
        {
            if (product == null || ModelState.IsValid == false)
            {
                return BadRequest("The item details are not valid.");
            }

            product.Barcode = product.Barcode?.Trim() ?? "";
            if (string.IsNullOrEmpty(product.Barcode))
            {
                product.Barcode = NextBarcode();
            }
            else if (BarcodeInUse(product.Barcode, 0) is Item other)
            {
                return Conflict($"Barcode {product.Barcode} is already used by \"{other.ItemName}\".");
            }

            product.Id = 0;
            product.CreatedById = 1;
            product.CreatedDateTime = DateTime.Now;
            _itemRepo.Add(product);
            QueueLabels(product);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("The item details are not valid.");
            }

            var existing = _itemRepo.Get(x => x.Id == item.Id);
            if (existing == null)
            {
                return NotFound($"Item {item.Id} was not found.");
            }

            var barcode = item.Barcode?.Trim();
            if (!string.IsNullOrEmpty(barcode) && BarcodeInUse(barcode, item.Id) is Item other)
            {
                return Conflict($"Barcode {barcode} is already used by \"{other.ItemName}\".");
            }

            // Copy only the fields the Stock page edits, so created date, category etc. are kept.
            if (!string.IsNullOrEmpty(barcode))
            {
                existing.Barcode = barcode;
            }
            CopyEditableFields(item, existing);
            existing.UpdatedById = 1;
            existing.UpdatedDateTime = DateTime.Now;
            _itemRepo.Update(existing);

            return Ok(existing);
        }

        [HttpPost("AddToBarcode")]
        public IActionResult AddToBarcodeById([FromBody] IdParamDto dto)
        {
            if (dto.Id == 0)
            {
                return NotFound();
            }
            var item = _itemRepo.Get(x => x.Id == dto.Id);
            if (item == null)
            {
                return NotFound();
            }

            QueueLabels(item);

            return Ok(item);
        }

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] IdParamDto dto)
        {
            if (dto.Id == 0)
            {
                return NotFound();
            }
            var category = _itemRepo.Get(x => x.Id == dto.Id);
            if (category == null)
            {
                return NotFound();
            }
            _itemRepo.Remove(category);

            return Ok(category);
        }

        [HttpGet("Export")]
        public IActionResult Export()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Items");

            for (int col = 0; col < ItemColumns.Length; col++)
            {
                var cell = ws.Cell(1, col + 1);
                cell.Value = ItemColumns[col];
                cell.Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var item in _itemRepo.GetAll().OrderBy(x => x.Id))
            {
                // Barcodes and HSN codes are text: "0012" must not become 12.
                ws.Cell(row, 1).SetValue(item.Barcode);
                ws.Cell(row, 2).SetValue(item.ItemName);
                ws.Cell(row, 3).SetValue(item.HSNCode);
                ws.Cell(row, 4).SetValue(item.Unit);
                ws.Cell(row, 5).SetValue(item.Quantity);
                ws.Cell(row, 6).SetValue(item.CostPrice);
                ws.Cell(row, 7).SetValue(item.MRP);
                ws.Cell(row, 8).SetValue(item.DiscountPercentage);
                ws.Cell(row, 9).SetValue(item.Tax);
                ws.Cell(row, 10).SetValue(item.IsTaxInclusive ? "Yes" : "No");
                ws.Cell(row, 11).SetValue(item.Description ?? "");
                row++;
            }
            ws.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Items_{DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        /// <summary>
        /// Adds or updates items from an Excel file laid out like the Export file. Rows are matched by barcode:
        /// a known barcode updates that item, anything else adds a new item. Rows with errors are skipped and listed.
        /// </summary>
        [HttpPost("Import")]
        public IActionResult Import(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Choose an Excel (.xlsx) file to import.");
            }
            if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only Excel .xlsx files can be imported. Use Export to get a file in the right layout.");
            }

            IXLWorksheet ws;
            XLWorkbook workbook;
            try
            {
                using var input = file.OpenReadStream();
                workbook = new XLWorkbook(input);
                ws = workbook.Worksheets.First();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Item import: could not read {FileName}", file.FileName);
                return BadRequest("The file could not be read as an Excel workbook.");
            }

            using (workbook)
            {
                var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in ws.Row(1).CellsUsed())
                {
                    columns[cell.GetString().Trim()] = cell.Address.ColumnNumber;
                }
                var missing = new[] { "Item name", "HSN code", "MRP" }.Where(x => !columns.ContainsKey(x)).ToList();
                if (missing.Count > 0)
                {
                    return BadRequest($"The first row must have the column headings {string.Join(", ", missing)}. Use Export to get a file in the right layout.");
                }

                int added = 0, updated = 0;
                var errors = new List<string>();
                int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);
                    string Text(string column) => columns.TryGetValue(column, out var c) ? row.Cell(c).GetString().Trim() : "";
                    if (ItemColumns.All(c => Text(c) == ""))
                    {
                        continue;
                    }

                    var error = ParseItemRow(Text, out var parsed);
                    if (error != null)
                    {
                        errors.Add($"Row {r}: {error}");
                        continue;
                    }

                    var existing = parsed.Barcode == "" ? null : _itemRepo.Get(x => x.Barcode == parsed.Barcode);
                    if (existing != null)
                    {
                        CopyEditableFields(parsed, existing);
                        existing.UpdatedById = 1;
                        existing.UpdatedDateTime = DateTime.Now;
                        _itemRepo.Update(existing);
                        updated++;
                    }
                    else
                    {
                        if (parsed.Barcode == "")
                        {
                            parsed.Barcode = NextBarcode();
                        }
                        parsed.CreatedById = 1;
                        parsed.CreatedDateTime = DateTime.Now;
                        _itemRepo.Add(parsed);
                        QueueLabels(parsed);
                        added++;
                    }
                }

                return Ok(new { added, updated, errors });
            }
        }

        private static string? ParseItemRow(Func<string, string> text, out Item item)
        {
            item = new Item { Barcode = text("Barcode"), ItemName = text("Item name"), HSNCode = text("HSN code"), Unit = text("Unit") };
            if (item.Unit == "")
            {
                item.Unit = "Pieces";
            }
            item.Description = text("Description") is { Length: > 0 } description ? description : null;

            if (item.ItemName == "")
            {
                return "Item name is empty.";
            }
            if (item.ItemName.Length > 30)
            {
                return $"Item name \"{item.ItemName}\" is longer than 30 characters.";
            }
            if (item.HSNCode == "")
            {
                return "HSN code is empty.";
            }

            if (!TryReadNumber(text("MRP"), null, out var mrp) || mrp <= 0)
            {
                return $"MRP \"{text("MRP")}\" is not a price.";
            }
            if (!TryReadNumber(text("Quantity"), 0, out var quantity) || quantity != Math.Floor(quantity))
            {
                return $"Quantity \"{text("Quantity")}\" is not a whole number.";
            }
            if (!TryReadNumber(text("Cost price"), 0, out var costPrice) || costPrice < 0)
            {
                return $"Cost price \"{text("Cost price")}\" is not a price.";
            }
            if (!TryReadNumber(text("Discount %"), 0, out var discount) || discount is < 0 or > 100)
            {
                return $"Discount % \"{text("Discount %")}\" must be between 0 and 100.";
            }
            if (!TryReadNumber(text("Tax %"), 0, out var tax) || tax is < 0 or > 100)
            {
                return $"Tax % \"{text("Tax %")}\" must be between 0 and 100.";
            }

            item.MRP = mrp;
            item.Quantity = (int)quantity;
            item.CostPrice = costPrice;
            item.DiscountPercentage = discount;
            item.Tax = tax;
            item.IsTaxInclusive = text("Tax inclusive").ToLowerInvariant() is not ("no" or "n" or "false" or "0");
            return null;
        }

        // Blank cells take the default; when there is no default, a blank cell is an error.
        private static bool TryReadNumber(string value, double? blankValue, out double number)
        {
            number = blankValue ?? 0;
            if (value == "")
            {
                return blankValue != null;
            }
            return double.TryParse(value.TrimEnd('%'), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out number);
        }

        private static void CopyEditableFields(Item from, Item to)
        {
            to.ItemName = from.ItemName;
            to.HSNCode = from.HSNCode;
            to.Unit = from.Unit;
            to.Quantity = from.Quantity;
            to.CostPrice = from.CostPrice;
            to.MRP = from.MRP;
            to.DiscountPercentage = from.DiscountPercentage;
            to.Tax = from.Tax;
            to.IsTaxInclusive = from.IsTaxInclusive;
            if (from.Description != null)
            {
                to.Description = from.Description;
            }
        }

        private Item? BarcodeInUse(string barcode, int exceptItemId) =>
            _itemRepo.Get(x => x.Barcode == barcode && x.Id != exceptItemId);

        // The next automatic barcode, skipping numbers already typed in by hand.
        private string NextBarcode()
        {
            var appConfig = _appConfigRepo.Get(x => x.Id == 1);
            if (appConfig == null)
            {
                return "";
            }
            string barcode;
            do
            {
                barcode = appConfig.Barcode.ToString();
                appConfig.Barcode++;
            } while (BarcodeInUse(barcode, 0) != null);
            _appConfigRepo.Update(appConfig);
            return barcode;
        }

        private void QueueLabels(Item item)
        {
            _barcodeRepo.Add(new Barcode()
            {
                ItemCode = item.Barcode,
                ItemName = item.ItemName,
                Price = (int)item.MRP,
                Quantity = item.Quantity
            });
        }
    }
}
