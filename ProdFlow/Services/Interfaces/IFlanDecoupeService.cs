using ProdFlow.DTOs;
using ProdFlow.Models.Requests;
using ProdFlow.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdFlow.Services.Interfaces
{
    public interface IFlanDecoupeService
    {
        Task<FlanDecoupeResponseDto> DecouperEnFlanAsync(FlanDecoupeRequestDto request);
        Task<FlanDecoupeListResponseDto> GetFlanDecoupesAsync(int? id = null);
        Task<List<string>> GetUncutProductsAsync();
    }
}