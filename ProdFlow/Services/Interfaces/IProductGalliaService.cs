using ProdFlow.DTOs;

namespace ProdFlow.Services.Interfaces
{
    public interface IProductGalliaService
    {
        Task AssociateProductWithGallia(CreateProductGalliaDto dto);
        Task<List<ProductGalliaAssociationDto>> GetAllAssociations();
        Task<ProductGalliaAssociationDto> GetAssociationById(int id);
        Task DeleteAssociation(int id);
    }
}