using System.Threading.Tasks;
using ProdFlow.DTOs;

namespace ProdFlow.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductOptionResponse> GetOptionsAsync();
        Task<ProductResult> CreateProductAsync(ProduitSerialiséDto dto);
        Task<ProductResult> UpdateProductAsync(ProduitSerialiséDto dto);
        Task<ProductResult> DeleteProductAsync(string ptNum);
        Task<ProductResult> GetProductAsync(string codeProduit, string status, bool? isSerialized);
    }
}