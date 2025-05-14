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
                // Call the service to delete Gallia
                var result = await _galliaService.DeleteGalliaAsync(id);

                // Check if the delete was successful
                if (result)
                {
                    return Ok(new { message = "Gallia deleted successfully" });
                }
                else
                {
                    // If no rows were affected, return not found
                    return NotFound(new { message = $"Gallia with ID {id} not found" });
                }
            }
            catch (Exception ex)
            {
                // Log and return a general error message
                _logger.LogError(ex, $"Error occurred while deleting Gallia with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the Gallia" });
            }
        }
        // GalliaController.cs
        [HttpPost("save-image")]
        public async Task<IActionResult> SaveImage([FromBody] LabelImageDto dto)
        {
            try
            {
                // Validate input
                if (dto == null)
                {
                    return BadRequest(new { message = "Request body cannot be empty" });
                }

                if (string.IsNullOrWhiteSpace(dto.Base64Image))
                {
                    return BadRequest(new { message = "Base64 image data cannot be empty" });
                }

                // Clean base64 prefix
                string cleanBase64;
                try
                {
                    cleanBase64 = CleanBase64String(dto.Base64Image);

                    // Test conversion to ensure it's valid base64
                    var testBytes = Convert.FromBase64String(cleanBase64);
                    if (testBytes.Length == 0)
                    {
                        return BadRequest(new { message = "Invalid image data (empty after conversion)" });
                    }
                }
                catch (FormatException)
                {
                    return BadRequest(new { message = "Invalid base64 image format" });
                }

                // Save to database
                await _galliaService.SaveLabelImageAsync(dto.GalliaId, cleanBase64);

                // Save to disk if path provided
                bool savedToDisk = false;
                string diskError = null;
                if (!string.IsNullOrWhiteSpace(dto.SavePath))
                {
                    try
                    {
                        var folderPath = dto.SavePath.Trim();

                        // Remove any surrounding quotes
                        folderPath = folderPath.Trim('"');

                        // Create directory if needed
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                        }

                        var fileName = $"Gallia_{dto.GalliaId}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        var filePath = Path.Combine(folderPath, fileName);

                        // Explicitly use System.IO.File to avoid ambiguity
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
                _logger.LogError(ex, "Error saving Gallia image");
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    details = ex.Message
                });
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
    }
}
