using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFlow.Services
{
    public class ProductGalliaService : IProductGalliaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductGalliaService> _logger;

        public ProductGalliaService(AppDbContext context, ILogger<ProductGalliaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AssociateProductWithGallia(CreateProductGalliaDto dto)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_product_gallia_associate 
                        {dto.GalliaId}, {dto.Pt_Num}, {dto.ProductName},
                        {dto.Quantity}, {dto.SupplierReference}, {dto.LabelNumber},
                        {dto.Description}, {dto.SupplierName}, {dto.ProductionDate}
                ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error associating product with Gallia");
                throw;
            }
        }

        public async Task<List<ProductGalliaAssociationDto>> GetAllAssociations()
        {
            try
            {
                // First materialize the query with ToListAsync, then work with the in-memory collection
                var results = await _context.Set<ProductGalliaAssociationDto>()
                    .FromSqlRaw("EXEC sp_product_gallia_get_all")
                    .AsNoTracking()
                    .ToListAsync();

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product-Gallia associations");
                throw;
            }
        }

        public async Task<ProductGalliaAssociationDto> GetAssociationById(int id)
        {
            try
            {
                // First materialize the query with ToListAsync, then work with the in-memory collection
                var result = (await _context.Set<ProductGalliaAssociationDto>()
                    .FromSqlRaw("EXEC sp_product_gallia_get_by_id @ProductGalliaId",
                        new SqlParameter("@ProductGalliaId", id))
                    .AsNoTracking()
                    .ToListAsync())
                    .FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving association {id}");
                throw;
            }
        }

        public async Task DeleteAssociation(int id)
        {
            try
            {
                int affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC sp_product_gallia_delete {id}");

                if (affectedRows == 0)
                {
                    throw new KeyNotFoundException($"Association with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting association {id}");
                throw;
            }
        }
    }
}