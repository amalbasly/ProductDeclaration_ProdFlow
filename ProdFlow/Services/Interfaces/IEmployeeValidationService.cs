using ProdFlow.DTOs;
using ProdFlow.Models.Responses;


namespace ProdFlow.Services.Interfaces
{
    public interface IEmployeeValidationService
    {
        Task<EmployeeValidationResult> ValidateEmployeeAsync(EmployeeValidationDto validationDto);
    }
}
