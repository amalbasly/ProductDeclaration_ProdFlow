using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Services
{
    public class EmployeeValidationService : IEmployeeValidationService
    {
        private readonly AppDbContext _context;

        public EmployeeValidationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeValidationResult> ValidateEmployeeAsync(EmployeeValidationDto validationDto)
        {
            var parameters = new[]
            {
                new SqlParameter("@pl_matric", validationDto.Pl_Matric),
                new SqlParameter("@pl_nom", validationDto.Pl_Nom),
                new SqlParameter("@pl_prenom", validationDto.Pl_Prenom)
            };

            var result = await _context.Database
                .SqlQueryRaw<EmployeeValidationResult>(
                    "EXEC [dbo].[ValidateEmployee] @pl_matric, @pl_nom, @pl_prenom", parameters)
                .ToListAsync();

            return result.FirstOrDefault() ?? new EmployeeValidationResult();
        }
    }
}