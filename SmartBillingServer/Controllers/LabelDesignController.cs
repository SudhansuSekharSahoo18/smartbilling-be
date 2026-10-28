using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using System.Text.Json;

namespace SmartBillingServer.Controllers
{
    /// <summary>
    /// The shop's single barcode label design, shared by every PC.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LabelDesignController : ControllerBase
    {
        private const int MaxDesignLength = 100_000;

        private readonly ILabelDesignRepository _labelDesignRepo;

        public LabelDesignController(ILabelDesignRepository labelDesignRepo)
        {
            _labelDesignRepo = labelDesignRepo;
        }

        // Returns 204 No Content when no design has been saved yet; the client then uses its default design.
        [HttpGet]
        public IActionResult Get()
        {
            var design = _labelDesignRepo.Get(x => x.Id == 1);
            if (design == null)
            {
                return NoContent();
            }
            return Content(design.DesignJson, "application/json");
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] JsonElement design)
        {
            if (design.ValueKind != JsonValueKind.Object)
            {
                return BadRequest("The label design must be a JSON object.");
            }

            var designJson = design.GetRawText();
            if (designJson.Length > MaxDesignLength)
            {
                return BadRequest("The label design is too large.");
            }

            var existing = _labelDesignRepo.Get(x => x.Id == 1);
            if (existing == null)
            {
                _labelDesignRepo.Add(new LabelDesign { Id = 1, DesignJson = designJson, UpdatedDateTime = DateTime.Now });
            }
            else
            {
                existing.DesignJson = designJson;
                existing.UpdatedDateTime = DateTime.Now;
                _labelDesignRepo.Update(existing);
            }

            return Ok();
        }
    }
}
