// Controllers/SynoptiqueController.cs
using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Models.Requests;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;
using ProdFlow.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SynoptiqueController : ControllerBase
    {
        private readonly ISynoptiqueService _synoptiqueService;
        private readonly ILogger<SynoptiqueController> _logger;

        public SynoptiqueController(
            ISynoptiqueService synoptiqueService,
            ILogger<SynoptiqueController> logger)
        {
            _synoptiqueService = synoptiqueService;
            _logger = logger;
        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<string>>> GetProducts()
        {
            try
            {
                var products = await _synoptiqueService.GetSerializedProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving serialized products");
                return StatusCode(500, "Error retrieving product list");
            }
        }

        [HttpGet("modes")]
        [ProducesResponseType(typeof(IEnumerable<ModeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ModeDto>>> GetAllModes()
        {
            try
            {
                var modes = await _synoptiqueService.GetAllModesAsync();
                return Ok(modes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modes");
                return StatusCode(500, "Error retrieving mode list");
            }
        }

        [HttpGet("{ptNum}")]
        [ProducesResponseType(typeof(IEnumerable<SynoptiqueEntryDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SynoptiqueEntryDto>>> GetSynoptiqueForProduct(
            [Required] string ptNum)
        {
            try
            {
                var entries = await _synoptiqueService.GetSynoptiqueForProductAsync(ptNum);
                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving synoptique for product {ProductCode}", ptNum);
                return StatusCode(500, $"Error retrieving synoptique: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SynoptiqueSaveResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveSynoptique(
            [FromBody, Required] SynoptiqueSaveRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for synoptique save");
                return BadRequest(ModelState);
            }

            if (request.Entries == null || !request.Entries.Any())
            {
                _logger.LogWarning("No entries provided for product {ProductCode}", request.PtNum);
                return BadRequest("No ranking entries provided");
            }

            try
            {
                var result = await _synoptiqueService.SaveSynoptiqueAsync(request);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save synoptique for product {ProductCode}: {Message}",
                        request.PtNum, result.Message);
                    return NotFound(result);
                }

                _logger.LogInformation("Successfully saved synoptique for product {ProductCode}", request.PtNum);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving synoptique for product {ProductCode}", request.PtNum);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error saving synoptique",
                    ProductCode = request.PtNum,
                    Error = ex.Message
                });
            }
        }
    }
}