using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductGalliaController : ControllerBase
    {
        private readonly IProductGalliaService _service;

        public ProductGalliaController(IProductGalliaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssociation([FromBody] CreateProductGalliaDto dto)
        {
            try
            {
                await _service.AssociateProductWithGallia(dto);
                return Ok(new { message = "Product-Gallia association created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductGalliaAssociationDto>>> GetAllAssociations()
        {
            try
            {
                return Ok(await _service.GetAllAssociations());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGalliaAssociationDto>> GetAssociation(int id)
        {
            try
            {
                var association = await _service.GetAssociationById(id);
                return association != null ? Ok(association) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssociation(int id)
        {
            try
            {
                await _service.DeleteAssociation(id);
                return Ok(new { message = "Association deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}