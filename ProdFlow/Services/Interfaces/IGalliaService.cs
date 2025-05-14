using System.Collections.Generic;
using System.Threading.Tasks;
using ProdFlow.DTOs;

namespace ProdFlow.Services.Interfaces
{
    public interface IGalliaService
    {
        Task<IEnumerable<GalliaDto>> GetAllGalliasAsync();
        Task<GalliaDto> GetGalliaByIdAsync(int id);
        Task<GalliaDto> CreateGalliaAsync(CreateGalliaDto createDto);
        Task UpdateGalliaAsync(UpdateGalliaDto updateDto);
        Task<bool> DeleteGalliaAsync(int id);
        Task SaveLabelImageAsync(int galliaId, string base64Image);

    }
}