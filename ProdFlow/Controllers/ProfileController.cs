// Controllers/ProfileController.cs
/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProdFlow.Services.Interfaces;
using System.Data.SqlClient;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IProfileService profileService,
            ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(
            [FromQuery] int pl_matric,
            [FromForm] string? pl_nom = null,
            [FromForm] string? pl_prenom = null,
            [FromForm] IFormFile? img = null)
        {
            try
            {
                // Validate image size before processing
                if (img?.Length > 4 * 1024 * 1024) // 4MB limit
                {
                    return BadRequest("Image size exceeds 4MB");
                }

                var rowsAffected = await _profileService.UpdateProfileAsync(
                    pl_matric, pl_nom, pl_prenom, img);

                return rowsAffected > 0
                    ? Ok("Profile updated successfully")
                    : NotFound("Employee not found");
            }
            catch (SqlException ex) when (ex.Number == 8152) // Truncation error
            {
                _logger.LogError(ex, "Image data too large");
                return BadRequest("Image data is too large");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error");
                return StatusCode(500, "Database error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return StatusCode(500, "Server error");
            }
        }
    }
}*/