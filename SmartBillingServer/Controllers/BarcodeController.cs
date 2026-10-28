using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        private readonly IBarcodeRepository _barcodeRepo;
        private readonly IApplicationConfigurationRepository _appConfigRepo;
        private readonly ILogger<BarcodeController> _logger;
        public BarcodeController(IBarcodeRepository db, IApplicationConfigurationRepository appConfigRepo, ILogger<BarcodeController> logger)
        {
            _barcodeRepo = db;
            _appConfigRepo = appConfigRepo;
            _logger = logger;
        }

        [HttpGet(Name = "Barcode")]
        public IEnumerable<Barcode> Get()
        {
            IEnumerable<Barcode> objCategoryList = _barcodeRepo.GetAll();
            return objCategoryList;
        }

        [HttpPost("GenerateBarcode")]
        public IActionResult GenerateBarcodeDocument([FromBody] BarcodeDto barcodeDto)
        {
            try
            {
                var content = "";
                content += $"ItemCode,ItemName,Price,Quantity\n";
                foreach (var barcode in barcodeDto.BarcodeList)
                {
                    content += $"{barcode.ItemCode},{barcode.ItemName},{barcode.Price},{barcode.Quantity}\n";
                }

                // Only the path saved on the Setting page (or the default) is used, never one sent by the browser.
                var filePath = SettingsController.ResolveBarcodeFilePath(_appConfigRepo.Get(x => x.Id == 1)?.BarcodeFilePath);
                var folder = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                System.IO.File.WriteAllText(filePath, content);
                var barcodeList = barcodeDto.BarcodeList.Where(x => x.Id != 0);
                _barcodeRepo.RemoveRange(barcodeList);
                return Ok(new { filePath, barcodeList = barcodeDto.BarcodeList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not write the barcode file");
                return BadRequest($"Could not write the barcode file: {ex.Message}");
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var barcode = _barcodeRepo.Get(x => x.Id == id);
            if (barcode == null)
            {
                return NotFound();
            }
            _barcodeRepo.Remove(barcode);

            return NoContent();
        }
    }
}
