// Services/Interfaces/IEmployeeService.cs
using ProdFlow.Models.Entities;
using ProdFlow.DTOs;

namespace ProdFlow.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<PersonnelTracaDto>> GetAllEmployeesAsync();
        Task<PersonnelTracaDto> GetEmployeeByMatriculeAsync(int pl_matric);
        Task<int> DeleteEmployeeAsync(long? pl_matric, string? pl_nom, string? pl_prenom);
        Task<int> UpdateEmployeeAsync(int pl_matric, string pl_fonc);
        Task<(long pl_matric, int rowsAffected)> AddEmployeeAsync(
            string pl_nom, string pl_prenom, long pl_badge,
            string pl_fonc, string img, string descriptionGrp);
    }
}