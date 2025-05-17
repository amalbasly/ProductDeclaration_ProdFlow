using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Models.Requests;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProdFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlanDecoupeController : ControllerBase
    {
        private readonly IFlanDecoupeService _service;
        private readonly ILogger<FlanDecoupeController> _logger;

        public FlanDecoupeController(IFlanDecoupeService service, ILogger<FlanDecoupeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FlanDecoupeRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for FlanDecoupe creation");
                    return BadRequest(ModelState);
                }
                var result = await _service.DecouperEnFlanAsync(request);
                _logger.LogInformation("Successfully created FlanDecoupe with ID {IdDecoupe}", result.IdDecoupe);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error during FlanDecoupe creation");
                return StatusCode(500, new { message = "An error occurred while creating the FlanDecoupe" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? id = null)
        {
            try
            {
                _logger.LogInformation("Fetching FlanDecoupes with ID filter: {Id}", id?.ToString() ?? "none");
                var result = await _service.GetFlanDecoupesAsync(id);
                if (result.Success)
                {
                    _logger.LogInformation("Successfully retrieved {Count} FlanDecoupes", result.FlanDecoupes.Count);
                    return Ok(result);
                }
                _logger.LogWarning("Failed to retrieve FlanDecoupes: {Message}", result.Message);
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error retrieving FlanDecoupes");
                return StatusCode(500, new { message = "An error occurred while retrieving FlanDecoupes" });
            }
        }

        [HttpGet("uncut-products")]
        public async Task<IActionResult> GetUncutProducts()
        {
            try
            {
                _logger.LogInformation("Fetching uncut products");
                var products = await _service.GetUncutProductsAsync();
                _logger.LogInformation("Successfully retrieved {Count} uncut products", products.Count);
                return Ok(products);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error retrieving uncut products: {Message}", ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving uncut products");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving uncut products" });
            }
        }
    }
}