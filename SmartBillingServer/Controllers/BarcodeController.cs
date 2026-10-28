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

                // A path sent by the client (older config.json setups) wins; otherwise the one saved in Setting.
                var filePath = SettingsController.ResolveBarcodeFilePath(
                    string.IsNullOrWhiteSpace(barcodeDto.FilePath) ? _appConfigRepo.Get(x => x.Id == 1)?.BarcodeFilePath : barcodeDto.FilePath);
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
                return BadRequest(ex.Message);
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





        //[HttpGet("{id}")]
        //public IActionResult GetById(int? id)
        //{
        //    //if (id == null || id == 0)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //var category = _BarcodeRepo.Get(x => x.Id == id);
        //    //if (category == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //return Ok(category);
        //}


        //[HttpPost(Name = "Create")]
        //public IActionResult Create([FromBody] Barcode category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _BarcodeRepo.Add(category);
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //    return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        //}


        //[HttpPost(Name = "Update")]
        //public IActionResult Update([FromBody] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _categoryRepo.Update(category);
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //    return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        //}


        //[HttpPost(Name = "Delete")]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var category = _categoryRepo.Get(x => x.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    _categoryRepo.Remove(category);

        //    return Ok(category);
        //}



        //[HttpPost(Name = "Edit")]
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var objCategory = _db.Categories.FirstOrDefault(x => x.Id == id); 
        //    if (objCategory == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(objCategory);
        //}
    }
}
