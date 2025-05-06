using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalliaController : ControllerBase
    {
        private readonly IGalliaService _galliaService;
        private readonly ILogger<GalliaController> _logger;

        public GalliaController(IGalliaService galliaService, ILogger<GalliaController> logger)
        {
            _galliaService = galliaService;
            _logger = logger;
        }

        [HttpGet("Get_Gallia")]
        public async Task<ActionResult<IEnumerable<GalliaDto>>> GetAll()
        {
            try
            {
                var gallias = await _galliaService.GetAllGalliasAsync();
                return Ok(gallias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gallias");
                return StatusCode(500, new { message = "An error occurred while retrieving gallias" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GalliaDto>> GetById(int id)
        {
            try
            {
                var gallia = await _galliaService.GetGalliaByIdAsync(id);
                if (gallia == null)
                {
                    return NotFound(new { message = $"Gallia with ID {id} not found" });
                }
                return Ok(gallia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the gallia" });
            }
        }

        [HttpPost("Create_Gallia")]
        public async Task<ActionResult<GalliaDto>> Create([FromBody] CreateGalliaDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data", errors = ModelState });
                }

                var createdGallia = await _galliaService.CreateGalliaAsync(createDto);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdGallia.GalliaId },
                    createdGallia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating gallia");
                return StatusCode(500, new { message = "An error occurred while creating the gallia" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGalliaDto updateDto)
        {
            try
            {
                if (id != updateDto.GalliaId)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data", errors = ModelState });
                }

                await _galliaService.UpdateGalliaAsync(updateDto);
                return Ok(new { message = "Gallia updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while updating the gallia" });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _galliaService.DeleteGalliaAsync(id);
                return Ok(new { message = "Gallia deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the gallia" });
            }
        }
    }
}