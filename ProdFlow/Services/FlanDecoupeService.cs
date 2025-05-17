using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Models.Entities;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ProdFlow.Models.Requests;
using ProdFlow.Models.Responses;

namespace ProdFlow.Services
{
    public class FlanDecoupeService : IFlanDecoupeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FlanDecoupeService> _logger;
        private readonly string _connectionString;

        public FlanDecoupeService(AppDbContext context, ILogger<FlanDecoupeService> logger)
        {
            _context = context;
            _logger = logger;
            _connectionString = context.Database.GetConnectionString();
        }

        public async Task<FlanDecoupeResponseDto> DecouperEnFlanAsync(FlanDecoupeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                _logger.LogInformation("Starting DecouperEnFlan for product {PtNumOriginal}", request.PtNumOriginal);

                // Validate product exists
                if (!await _context.Produits.AnyAsync(p => p.PtNum == request.PtNumOriginal))
                    throw new KeyNotFoundException($"Product {request.PtNumOriginal} not found.");

                // Validate label
                if (!await _context.Gallias.AnyAsync(g => g.GalliaName == request.LabelUtilise && g.LabelName == "Etiquette"))
                    throw new ArgumentException($"Label {request.LabelUtilise} is invalid.");

                // Check if already cut
                if (await _context.FlanDecoupes.AnyAsync(f => f.pt_numOriginal == request.PtNumOriginal))
                    throw new InvalidOperationException($"Product {request.PtNumOriginal} already cut.");

                // Execute stored procedure
                var newIdParam = new SqlParameter("@NewFlanDecoupeId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[DecouperEnFlan] @pt_numOriginal, @NombreDeParts, @LabelUtilise, @Utilisateur, @NewFlanDecoupeId OUTPUT",
                    new SqlParameter("@pt_numOriginal", request.PtNumOriginal),
                    new SqlParameter("@NombreDeParts", request.NombreDeParts),
                    new SqlParameter("@LabelUtilise", request.LabelUtilise),
                    new SqlParameter("@Utilisateur", request.Utilisateur ?? (object)DBNull.Value),
                    newIdParam
                );

                int newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : throw new InvalidOperationException("Stored procedure did not return a valid IdDecoupe.");

                _logger.LogInformation("DecouperEnFlan created FlanDecoupe with ID {IdDecoupe}", newId);

                // Fetch the created record to verify
                var createdDecoupe = await _context.FlanDecoupes
                    .Where(f => f.IdDecoupe == newId)
                    .Select(f => new { f.DateDecoupe })
                    .FirstOrDefaultAsync();

                if (createdDecoupe == null)
                    throw new InvalidOperationException($"FlanDecoupe with ID {newId} not found after creation.");

                return new FlanDecoupeResponseDto
                {
                    Success = true,
                    IdDecoupe = newId,
                    PtNumOriginal = request.PtNumOriginal,
                    NombreDeParts = request.NombreDeParts,
                    LabelUtilise = request.LabelUtilise,
                    Utilisateur = request.Utilisateur,
                    DateDecoupe = createdDecoupe.DateDecoupe,
                    PartsCount = request.NombreDeParts,
                    Parts = Enumerable.Range(1, request.NombreDeParts).Select(i => new FlanPartieDto
                    {
                        CodePartie = $"{request.PtNumOriginal}_part{i}",
                        PtNumOriginal = request.PtNumOriginal,
                        NumeroPartie = i,
                        Label = request.LabelUtilise,
                        DateCreation = DateTime.Now
                    }).ToList()
                };
            }
            catch (SqlException ex) when (ex.Message.Contains("FlanDecoupeId"))
            {
                _logger.LogError(ex, "SQL Error in DecouperEnFlanAsync: FlanDecoupeId column issue");
                throw new InvalidOperationException("Database schema error: FlanDecoupeId column is missing or misconfigured in FlanPartie table.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecouperEnFlanAsync");
                throw;
            }
        }

        public async Task<FlanDecoupeListResponseDto> GetFlanDecoupesAsync(int? id = null)
        {
            try
            {
                _logger.LogInformation("Getting FlanDecoupes with filter: {Filter}",
                    id.HasValue ? $"ID = {id}" : "All records");

                var decoupes = new List<FlanDecoupeDto>();
                var parts = new List<FlanPartieDto>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.GetFlanDecoupes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdDecoupe", (object)id ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Read first result set (FlanDecoupe)
                            while (await reader.ReadAsync())
                            {
                                decoupes.Add(new FlanDecoupeDto
                                {
                                    IdDecoupe = reader.GetInt32(reader.GetOrdinal("IdDecoupe")),
                                    PtNumOriginal = reader.GetString(reader.GetOrdinal("pt_numOriginal")),
                                    NombreDeParts = reader.GetInt32(reader.GetOrdinal("NombreDeParts")),
                                    LabelUtilise = reader.IsDBNull(reader.GetOrdinal("LabelUtilise")) ? null : reader.GetString(reader.GetOrdinal("LabelUtilise")),
                                    DateDecoupe = reader.GetDateTime(reader.GetOrdinal("DateDecoupe")),
                                    Utilisateur = reader.IsDBNull(reader.GetOrdinal("Utilisateur")) ? null : reader.GetString(reader.GetOrdinal("Utilisateur"))
                                });
                            }

                            // Move to next result set (FlanPartie)
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    parts.Add(new FlanPartieDto
                                    {
                                        CodePartie = reader.GetString(reader.GetOrdinal("CodePartie")),
                                        PtNumOriginal = reader.GetString(reader.GetOrdinal("pt_numOriginal")),
                                        NumeroPartie = reader.GetInt32(reader.GetOrdinal("NumeroPartie")),
                                        Label = reader.IsDBNull(reader.GetOrdinal("Label")) ? null : reader.GetString(reader.GetOrdinal("Label")),
                                        DateCreation = reader.IsDBNull(reader.GetOrdinal("DateCreation")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateCreation"))
                                    });
                                }
                            }
                        }
                    }
                }

                // Group parts by pt_numOriginal
                var partsByProduct = parts.GroupBy(p => p.PtNumOriginal)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Map to response DTO
                return new FlanDecoupeListResponseDto
                {
                    Success = true,
                    FlanDecoupes = decoupes.Select(f => new FlanDecoupeResponseDto
                    {
                        Success = true,
                        IdDecoupe = f.IdDecoupe,
                        PtNumOriginal = f.PtNumOriginal,
                        NombreDeParts = f.NombreDeParts,
                        LabelUtilise = f.LabelUtilise,
                        DateDecoupe = f.DateDecoupe,
                        Utilisateur = f.Utilisateur,
                        PartsCount = partsByProduct.TryGetValue(f.PtNumOriginal, out var productParts)
                            ? productParts.Count
                            : 0,
                        Parts = partsByProduct.TryGetValue(f.PtNumOriginal, out productParts)
                            ? productParts
                            : new List<FlanPartieDto>()
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FlanDecoupes");
                return new FlanDecoupeListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving FlanDecoupes: {ex.Message}",
                    FlanDecoupes = new List<FlanDecoupeResponseDto>()
                };
            }
        }

        public async Task<List<string>> GetUncutProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching uncut products using stored procedure dbo.GetUncutProducts");

                var uncutProducts = new List<string>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.GetUncutProducts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                uncutProducts.Add(reader.GetString(reader.GetOrdinal("pt_num")));
                            }
                        }
                    }
                }

                _logger.LogInformation("Retrieved {Count} uncut products", uncutProducts.Count);
                return uncutProducts;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving uncut products: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to retrieve uncut products due to a database error.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uncut products");
                throw new InvalidOperationException("An unexpected error occurred while retrieving uncut products.", ex);
            }
        }

        // Internal DTO for FlanDecoupe to avoid EF entity issues
        private class FlanDecoupeDto
        {
            public int IdDecoupe { get; set; }
            public string PtNumOriginal { get; set; }
            public int NombreDeParts { get; set; }
            public string LabelUtilise { get; set; }
            public DateTime DateDecoupe { get; set; }
            public string Utilisateur { get; set; }
        }
    }
}