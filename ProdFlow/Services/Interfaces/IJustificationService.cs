// Services/Interfaces/IJustificationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;

namespace ProdFlow.Services.Interfaces
{
    public interface IJustificationService
    {
        Task<JustificationResponse> SubmitJustificationAsync(CreateJustificationDto justificationDto);
        Task<List<JustificationDto>> GetJustificationsAsync(
            string productCode = null,
            string status = null,
            string submittedBy = null);
        Task<bool> ProductExistsAsync(string productCode);
    }
}