using ProdFlow.DTOs;
using ProdFlow.Models.Responses;

namespace ProdFlow.Services.Interfaces
{
    public interface IJustificationService
    {
        Task<JustificationResponse> SubmitJustificationAsync(CreateJustificationDto justificationDto);
        Task<bool> ProductExistsAsync(string productCode);
    }
}