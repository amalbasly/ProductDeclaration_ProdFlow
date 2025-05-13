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

        /// <summary>
        /// Get all Gallias with associated fields
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
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

        /// <summary>
        /// Get a specific Gallia by ID with its fields
        /// </summary>
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
                _logger.LogError(ex, $"Error retrieving Gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the Gallia" });
            }
        }

        /// <summary>
        /// Create a new Gallia with multiple fields
        /// </summary>
        [HttpPost("Create")]
        public async Task<ActionResult<GalliaDto>> Create([FromBody] CreateGalliaDto createDto)
        {
            var created = await _galliaService.CreateGalliaAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.GalliaId }, created);
        }

        /// <summary>
        /// Update an existing Gallia including its fields
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
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
                _logger.LogError(ex, $"Error updating Gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while updating the Gallia" });
            }
        }

        /// <summary>
        /// Delete a Gallia by ID
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _galliaService.DeleteGalliaAsync(id);
                return Ok(new { message = "Gallia deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the Gallia" });
            }
        }
        [HttpPost("save-image")]
        public async Task<IActionResult> SaveLabelImage([FromBody] LabelImageDto dto)
        {
            await _galliaService.SaveLabelImageAsync(dto.GalliaId, dto.Base64Image);
            return Ok(new { message = "Image saved" });
        }
    }
}