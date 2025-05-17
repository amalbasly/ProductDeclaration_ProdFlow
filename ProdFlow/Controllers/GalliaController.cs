using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]/{labelType}")]
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
        /// Get all entities of the specified type (Gallia or Etiquette) with associated fields
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<GalliaDto>>> GetAll(string labelType)
        {
            try
            {
                var items = await _galliaService.GetAllGalliasAsync(labelType);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving {LabelType}s", labelType);
                return StatusCode(500, new { message = $"An error occurred while retrieving {labelType}s" });
            }
        }

        /// <summary>
        /// Get a specific entity by ID with its fields
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GalliaDto>> GetById(int id, string labelType)
        {
            try
            {
                var item = await _galliaService.GetGalliaByIdAsync(id);

                if (item == null || !string.Equals(item.LabelName, labelType, StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { message = $"{labelType} with ID {id} not found" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving {labelType} with ID {id}");
                return StatusCode(500, new { message = $"An error occurred while retrieving the {labelType}" });
            }
        }

        /// <summary>
        /// Create a new entity with multiple fields
        /// Default LabelName is set to the provided labelType
        /// </summary>
        [HttpPost("Create")]
        public async Task<ActionResult<GalliaDto>> Create(string labelType, [FromBody] CreateGalliaDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(new { message = "Request body cannot be empty" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data", errors = ModelState });
                }

                // Enforce label type consistency
                if (string.IsNullOrWhiteSpace(createDto.LabelName))
                {
                    createDto.LabelName = labelType;
                }
                else if (!string.Equals(createDto.LabelName, labelType, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"LabelName must match '{labelType}' for this endpoint." });
                }

                var created = await _galliaService.CreateGalliaAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.GalliaId, labelType }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {LabelType}", labelType);
                return StatusCode(500, new { message = $"An error occurred while creating the {labelType}" });
            }
        }

        /// <summary>
        /// Update an existing entity including its fields
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, string labelType, [FromBody] UpdateGalliaDto updateDto)
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

                // Enforce label type consistency
                if (string.IsNullOrWhiteSpace(updateDto.LabelName))
                {
                    updateDto.LabelName = labelType;
                }
                else if (!string.Equals(updateDto.LabelName, labelType, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"LabelName must be '{labelType}' for this endpoint." });
                }

                await _galliaService.UpdateGalliaAsync(updateDto);
                return Ok(new { message = $"{labelType} updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating {labelType} with ID {id}");
                return StatusCode(500, new { message = $"An error occurred while updating the {labelType}" });
            }
        }

        /// <summary>
        /// Delete an entity by ID
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id, string labelType)
        {
            try
            {
                var existing = await _galliaService.GetGalliaByIdAsync(id);

                if (existing == null || !string.Equals(existing.LabelName, labelType, StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { message = $"{labelType} with ID {id} not found" });
                }

                var result = await _galliaService.DeleteGalliaAsync(id);

                if (result)
                {
                    return Ok(new { message = $"{labelType} deleted successfully" });
                }

                return NotFound(new { message = $"{labelType} with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {labelType} with ID {id}");
                return StatusCode(500, new { message = $"An error occurred while deleting the {labelType}" });
            }
        }

        /// <summary>
        /// Save base64 encoded image for an entity
        /// </summary>
        [HttpPost("save-image")]
        public async Task<IActionResult> SaveImage(string labelType, [FromBody] LabelImageDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Base64Image))
                {
                    return BadRequest(new { message = "Invalid image data" });
                }

                string cleanBase64 = CleanBase64String(dto.Base64Image);

                try
                {
                    Convert.FromBase64String(cleanBase64); // Validate
                }
                catch (FormatException)
                {
                    return BadRequest(new { message = "Invalid base64 format" });
                }

                await _galliaService.SaveLabelImageAsync(dto.GalliaId, cleanBase64);

                bool savedToDisk = false;
                string diskError = null;

                if (!string.IsNullOrWhiteSpace(dto.SavePath))
                {
                    try
                    {
                        var folderPath = dto.SavePath.Trim('"');
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                        }

                        var fileName = $"{labelType}_{dto.GalliaId}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        var filePath = Path.Combine(folderPath, fileName);

                        await System.IO.File.WriteAllBytesAsync(filePath, Convert.FromBase64String(cleanBase64));
                        savedToDisk = true;
                    }
                    catch (Exception ex)
                    {
                        diskError = ex.Message;
                        _logger.LogError(ex, "Failed to save image to disk at path: {Path}", dto.SavePath);
                    }
                }

                return Ok(new
                {
                    message = "Image saved successfully",
                    savedToDatabase = true,
                    savedToDisk = savedToDisk,
                    diskError = diskError
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving {LabelType} image", labelType);
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        private string CleanBase64String(string base64Input)
        {
            if (base64Input.StartsWith("data:image"))
            {
                var commaIndex = base64Input.IndexOf(',');
                if (commaIndex >= 0)
                {
                    return base64Input[(commaIndex + 1)..];
                }
            }
            return base64Input;
        }
        [HttpGet("names")]
        public async Task<IActionResult> GetGalliaNames(string labelType)
        {
            var names = await _galliaService.GetGalliaNamesAsync(labelType);
            return Ok(names);
        }
    }
}