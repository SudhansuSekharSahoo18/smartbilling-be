using Microsoft.AspNetCore.Mvc;
using Models.Dtos;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        //private readonly IBarcodeRepository _BarcodeRepo;
        private readonly ILogger<BarcodeController> _logger; 
        //private readonly IConfiguration _configuration;
        private string fileName = "C:\\Sudhansu\\Barcode.csv";
        public BarcodeController(ILogger<BarcodeController> logger)
        {
            //_BarcodeRepo = db;
            _logger = logger;
            //_configuration = configuration;
            //fileName = _configuration["appsettings:ConfigSettings:BarcodeGenerationFilePath"];
        }

        [HttpPost("GenerateBarcode")]
        public IActionResult GenerateBarcodeDocument([FromBody] List<Barcode> barcodeList)
        {
            var content = "";
            content += $"ItemCode,ItemName,Price,Quantity\n";
            foreach (var barcode in barcodeList)
            {
                content += $"{barcode.ItemCode},{barcode.ItemName},{barcode.Price},{barcode.Quantity}\n";
            }

            System.IO.File.WriteAllText(fileName, content);
            return Ok(barcodeList);
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
