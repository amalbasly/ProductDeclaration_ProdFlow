using ProdFlow.DTOs;

namespace ProdFlow.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductOptionResponse> GetOptionsAsync();
        Task<ProductResult> CreateProductAsync(ProduitSerialiséDto dto);
        Task<ProductResult> UpdateProductAsync(ProduitSerialiséDto dto);
        Task<ProductResult> DeleteProductAsync(string ptNum);
        Task<ProductResult> GetProductAsync(bool isApproved, string codeProduit = null, string status = null, bool? isSerialized = null);
        Task<ProductResult> VerifyProductAsync(VerifyProductDto dto, string managerId);
        Task AutoDeclineProductAsync(string productId, string token);
        Task<string> TestSendGridEmailAsync();
        Task<ProductResult> GetProductsAsync(
            string codeProduit = null,
            string status = null,
            bool? isSerialized = null,
            bool? isApproved = null,
            bool includeJustificationDetails = false);
        Task<ProductResult> GetAllProductsAsync(bool? isSerialized = null);
        Task<ProductResult> GetApprovedProductsAsync(bool? isSerialized = null);
        Task<ProductResult> GetPendingApprovalProductsAsync(bool? isSerialized = null);
    }
}