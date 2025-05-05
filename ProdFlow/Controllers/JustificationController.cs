// Controllers/JustificationController.cs
using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JustificationController : ControllerBase
    {
        private readonly IJustificationService _justificationService;
        private readonly ILogger<JustificationController> _logger;

        public JustificationController(
            IJustificationService justificationService,
            ILogger<JustificationController> logger)
        {
            _justificationService = justificationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitJustification([FromBody] CreateJustificationDto justificationDto)
        {
            try
            {
                Validator.ValidateObject(justificationDto, new ValidationContext(justificationDto), true);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Message}", ex.Message);
                return BadRequest(new
                {
                    Error = "Validation failed",
                    Details = ex.Message
                });
            }

            try
            {
                var result = await _justificationService.SubmitJustificationAsync(justificationDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Error = "Invalid product code",
                    Details = ex.Message
                });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while submitting justification");
                return StatusCode(500, new
                {
                    Error = "Database error",
                    Details = ex.Message,
                    SqlErrorNumber = ex.Number
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while submitting justification");
                return StatusCode(500, new
                {
                    Error = "Server error",
                    Details = ex.Message
                });
            }
        }
    }
}