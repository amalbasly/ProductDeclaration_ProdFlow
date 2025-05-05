// Services/Interfaces/ISynoptiqueService.cs
using ProdFlow.DTOs;
using ProdFlow.Models.Requests;
using ProdFlow.Models.Responses;
using ProdFlow.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdFlow.Services.Interfaces
{
    public interface ISynoptiqueService
    {
        Task<IEnumerable<string>> GetSerializedProductsAsync();
        Task<IEnumerable<ModeDto>> GetAllModesAsync();
        Task<IEnumerable<SynoptiqueEntryDto>> GetSynoptiqueForProductAsync(string ptNum);
        Task<SynoptiqueSaveResult> SaveSynoptiqueAsync(SynoptiqueSaveRequest request);
    }

}