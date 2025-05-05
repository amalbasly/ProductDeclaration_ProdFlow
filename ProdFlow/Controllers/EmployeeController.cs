// Controllers/EmployeeController.cs
using Microsoft.AspNetCore.Mvc;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("Read_All_Employees")]
        public async Task<ActionResult<List<PersonnelTracaDto>>> GetAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        [HttpGet("GetEmployeeByMatricule/{pl_matric}")]
        public async Task<ActionResult<PersonnelTracaDto>> GetEmployeeByMatricule(int pl_matric)
        {
            var employee = await _employeeService.GetEmployeeByMatriculeAsync(pl_matric);
            return employee != null ? Ok(employee) : NotFound();
        }

        [HttpDelete("DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployee(long? pl_matric, string? pl_nom, string? pl_prenom)
        {
            if (!pl_matric.HasValue && (string.IsNullOrEmpty(pl_nom) || string.IsNullOrEmpty(pl_prenom)))
            {
                return BadRequest("Please provide either Employee Matricule or both FirstName and LastName.");
            }

            var rowsAffected = await _employeeService.DeleteEmployeeAsync(pl_matric, pl_nom, pl_prenom);
            return rowsAffected > 0
                ? Ok("Employee deleted successfully.")
                : NotFound("Employee not found.");
        }

        [HttpPut("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(int pl_matric, string pl_fonc)
        {
            var rowsAffected = await _employeeService.UpdateEmployeeAsync(pl_matric, pl_fonc);
            return rowsAffected > 0
                ? Ok("Employee function updated successfully.")
                : NotFound("Employee not found.");
        }

        [HttpPost("AddEmployee")]
        public async Task<ActionResult> AddEmployee(
            string pl_nom, string pl_prenom, long pl_badge,
            string pl_fonc, string img, string descriptionGrp)
        {
            var (pl_matric, rowsAffected) = await _employeeService.AddEmployeeAsync(
                pl_nom, pl_prenom, pl_badge, pl_fonc, img, descriptionGrp);

            return rowsAffected > 0
                ? Ok(new { Message = "Employee added successfully.", pl_matric })
                : BadRequest("Failed to add employee.");
        }
    }
}