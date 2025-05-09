using ProdFlow.DTOs;
using ProdFlow.Models.Responses;

namespace ProdFlow.Services.Interfaces
{
    public interface IClientReferenceService
    {
        Task<ClientReferenceResponse> CreateAsync(string ptNum, ClientReferenceCreateDto dto);
        Task<ClientReferenceResponse> UpdateAsync(string ptNum, ClientReferenceUpdateDto dto);
        Task<string> DeleteAsync(string ptNum);
        Task<ClientReferenceResponse> GetByPtNumAsync(string ptNum);
    }
}
