using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
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
        private int barcode = 110;
        public ItemController(IItemRepository db, ILogger<ItemController> logger, IApplicationConfigurationRepository appConfigRepo,
            IBarcodeRepository barcodeRepo)
        {
            _itemRepo = db;
            _logger = logger;
            _appConfigRepo = appConfigRepo;
            _barcodeRepo = barcodeRepo;
        }

        [HttpGet(Name = "Item")]
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
                return BadRequest();
            }

            if (string.IsNullOrEmpty(product.Barcode))
            {
                var appConfig = _appConfigRepo.Get(x => x.Id == 1);
                if (appConfig != null)
                {
                    product.Barcode = appConfig.Barcode.ToString();
                    appConfig.Barcode++;
                    _appConfigRepo.Update(appConfig);
                }
            }
            product.CreatedById = 1;
            product.CreatedDateTime = DateTime.Now;
            _itemRepo.Add(product);
            var barcode = new Barcode() { ItemCode = product.Barcode, ItemName = product.ItemName,
                Price = (int)product.SellPrice, Quantity = product.Quantity };
            _barcodeRepo.Add(barcode);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] Item product)
        {
            if (ModelState.IsValid)
            {
                _itemRepo.Update(product);
            }
            else
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }


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
