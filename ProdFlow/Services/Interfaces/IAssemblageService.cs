using ProdFlow.DTOs;
using ProdFlow.Models.Responses;

namespace ProdFlow.Services.Interfaces
{
    public interface IAssemblageService
    {
        Task<int> CreateAssemblageAsync(CreateAssemblageDto dto);
        Task<AssemblageDto> GetAssemblageByIdAsync(int id);
        Task<List<AssemblageDto>> GetAllAssemblagesAsync();
        Task UpdateAssemblageAsync(int id, UpdateAssemblageDto dto);
        Task DeleteAssemblageAsync(int id);
    }
}
