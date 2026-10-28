using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;

namespace SmartBillingServer.Controllers
{
    /// <summary>
    /// Shop settings entered in the app (first-run setup and the Setting page), stored in
    /// the ApplicationConfiguration row so no config file has to be edited by hand.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IApplicationConfigurationRepository _appConfigRepo;

        public SettingsController(IApplicationConfigurationRepository appConfigRepo)
        {
            _appConfigRepo = appConfigRepo;
        }

        public static string DefaultBarcodeFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartBilling", "Barcode.csv");

        public static string ResolveBarcodeFilePath(string? path) =>
            string.IsNullOrWhiteSpace(path) ? DefaultBarcodeFilePath : path.Trim();

        [HttpGet]
        public IActionResult Get()
        {
            var config = _appConfigRepo.Get(x => x.Id == 1);
            return Ok(new
            {
                shopName = config?.ShopName ?? "",
                shopAddress = config?.ShopAddress ?? "",
                shopGSTNumber = config?.ShopGSTNumber ?? "",
                barcodeFilePath = config?.BarcodeFilePath ?? "",
                defaultBarcodeFilePath = DefaultBarcodeFilePath,
                isSetupComplete = !string.IsNullOrWhiteSpace(config?.ShopName),
            });
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] ShopSettingsDto settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ShopName))
            {
                return BadRequest("Shop name is required.");
            }

            var barcodeFilePath = settings.BarcodeFilePath?.Trim() ?? "";
            if (barcodeFilePath != "" && (!Path.IsPathFullyQualified(barcodeFilePath) || Path.GetFileName(barcodeFilePath) == ""))
            {
                return BadRequest("Barcode export file must be a full file path, for example C:\\SmartBilling\\Barcode.csv.");
            }

            var config = _appConfigRepo.Get(x => x.Id == 1);
            var isNew = config == null;
            config ??= new ApplicationConfiguration { Id = 1, Barcode = 101, Version = "1.0.0" };

            config.ShopName = settings.ShopName.Trim();
            config.ShopAddress = settings.ShopAddress?.Trim() ?? "";
            config.ShopGSTNumber = settings.ShopGSTNumber?.Trim().ToUpperInvariant() ?? "";
            config.BarcodeFilePath = barcodeFilePath;

            if (isNew)
            {
                _appConfigRepo.Add(config);
            }
            else
            {
                _appConfigRepo.Update(config);
            }

            return Ok();
        }
    }
}
