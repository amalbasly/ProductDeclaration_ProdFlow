using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientReferencesController : ControllerBase
    {
        private readonly IClientReferenceService _service;

        public ClientReferencesController(IClientReferenceService service)
        {
            _service = service;
        }

        [HttpGet("{ptNum}")]
        public async Task<ActionResult<ClientReferenceResponse>> Get(string ptNum)
        {
            try
            {
                var result = await _service.GetByPtNumAsync(ptNum);
                return result != null ? Ok(result) : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("{ptNum}")]
        public async Task<IActionResult> Create(string ptNum, [FromBody] ClientReferenceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateAsync(ptNum, dto);

                // Option 1: Return 200 OK with the created resource
                return Ok(result);

                // Option 2: Fix CreatedAtAction by ensuring route exists
                // return CreatedAtAction(
                //     actionName: nameof(Get),
                //     routeValues: new { ptNum = result.PtNum },
                //     value: result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPut("{ptNum}")]
        public async Task<IActionResult> Update(string ptNum, [FromBody] ClientReferenceUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(ptNum, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpDelete("{ptNum}")]
        public async Task<IActionResult> Delete(string ptNum)
        {
            try
            {
                string message = await _service.DeleteAsync(ptNum);
                return Ok(new
                {
                    Success = true,
                    Message = message,
                    PtNum = ptNum
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}