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
        private readonly IProductRepository _productRepo;
        private readonly IApplicationConfigurationRepository _appConfigRepo;
        private readonly ILogger<ItemController> _logger;
        private int barcode = 110; 
        public ItemController(IProductRepository db, ILogger<ItemController> logger, IApplicationConfigurationRepository appConfigRepo)
        {
            _productRepo = db;
            _logger = logger;
            _appConfigRepo = appConfigRepo;
        }

        [HttpGet(Name = "Item")]
        public IEnumerable<Item> Get()
        {
            var objCategoryList = _productRepo.GetAll();
            return objCategoryList;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _productRepo.Get(x => x.Id == id);
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

            if(string.IsNullOrEmpty(product.Barcode))
            {
                var appConfig = _appConfigRepo.Get(x => x.Id == 1);
                if(appConfig != null)
                {
                    product.Barcode = appConfig.Barcode.ToString();
                    appConfig.Barcode++;
                    _appConfigRepo.Update(appConfig);
                }
            }
            product.CreatedById = 1;
            product.CreatedDateTime = DateTime.Now;
            _productRepo.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] Item product)
        {
            if (ModelState.IsValid)
            {
                _productRepo.Update(product);
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
