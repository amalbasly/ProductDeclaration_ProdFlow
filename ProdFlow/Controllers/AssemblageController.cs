using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssemblageController : ControllerBase
    {
        private readonly IAssemblageService _assemblageService;

        public AssemblageController(IAssemblageService assemblageService)
        {
            _assemblageService = assemblageService;
        }

        [HttpPost("Create_Assemblage")]
        public async Task<IActionResult> CreateAssemblage([FromBody] CreateAssemblageDto dto)
        {
            var assemblageId = await _assemblageService.CreateAssemblageAsync(dto);
            return CreatedAtAction(nameof(GetAssemblage), new { id = assemblageId }, new { AssemblageId = assemblageId });
        }

        [HttpGet("Get_Assemblage_by/{id}")]
        public async Task<IActionResult> GetAssemblage(int id)
        {
            var assemblage = await _assemblageService.GetAssemblageByIdAsync(id);
            if (assemblage == null) return NotFound();
            return Ok(assemblage);
        }

        [HttpGet("Get_Assemblage")]
        public async Task<IActionResult> GetAllAssemblages()
        {
            var assemblages = await _assemblageService.GetAllAssemblagesAsync();
            return Ok(assemblages);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssemblage(int id, [FromBody] UpdateAssemblageDto dto)
        {
            await _assemblageService.UpdateAssemblageAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssemblage(int id)
        {
            await _assemblageService.DeleteAssemblageAsync(id);
            return NoContent();
        }
    }
}

