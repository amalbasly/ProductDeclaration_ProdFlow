// Controllers/ValidationController.cs
using Microsoft.AspNetCore.Mvc;

using ProdFlow.DTOs;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly IEmployeeValidationService _validationService;

        public ValidationController(IEmployeeValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpGet("ValidateEmployee")]
        public async Task<ActionResult<EmployeeValidationResult>> ValidateEmployee(
            [FromQuery] long pl_matric,
            [FromQuery] string pl_nom,
            [FromQuery] string pl_prenom)
        {
            var validationDto = new EmployeeValidationDto
            {
                Pl_Matric = pl_matric,
                Pl_Nom = pl_nom,
                Pl_Prenom = pl_prenom
            };

            var result = await _validationService.ValidateEmployeeAsync(validationDto);
            return Ok(result);
        }
    }
}