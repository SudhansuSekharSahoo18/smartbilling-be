using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository db, ILogger<ProductController> logger)
        {
            _productRepo = db;
            _logger = logger;
        }

        [HttpGet(Name = "Product")]
        public IEnumerable<Product> Get()
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


        //[HttpPost(Name = "Create")]
        //public IActionResult Create([FromBody] Product category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _productRepo.Add(category);
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
