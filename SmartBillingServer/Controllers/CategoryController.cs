using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using SmartBillingServer.Models;

namespace SmartBillingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryRepository db, ILogger<CategoryController> logger)
        {
            _categoryRepo = db;
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult<IEnumerable<Category>> Get() // Impliment async await
        {
            var objCategoryList = _categoryRepo.GetAll();
            return Ok(objCategoryList);
        }

        [HttpGet("GetById/{id}")]
        public ActionResult<Category> GetById(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _categoryRepo.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category);
            }
            else
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }


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
