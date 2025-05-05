using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ProdFlow.Services.Interfaces;

namespace ProdFlow.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context) => _context = context;

        public async Task<ProductOptionResponse> GetOptionsAsync()
        {
            var options = new ProductOptionResponse
            {
                Lignes = await GetDistinctValues(nameof(Produit.LpNum)),
                Famille = await GetDistinctValues(nameof(Produit.FpCod)),
                SousFamilles = await GetDistinctValues(nameof(Produit.SpCod)),
                Types = await GetDistinctValues(nameof(Produit.TpCod)),
                Statuts = await GetDistinctValues(nameof(Produit.SpId))
            };
            return options;
        }

        private async Task<List<string>> GetDistinctValues(string propertyName)
        {
            var entityType = _context.Model.FindEntityType(typeof(Produit));
            var property = entityType?.FindProperty(propertyName);
            if (property == null) return new List<string>();

            var columnName = property.GetColumnName();
            var sql = $"SELECT DISTINCT [{columnName}] FROM produit WHERE [{columnName}] IS NOT NULL";
            return await _context.Database.SqlQueryRaw<string>(sql).ToListAsync();
        }

        public async Task<ProductResult> CreateProductAsync(ProduitSerialiséDto dto)
        {
            if (await _context.Produits.AnyAsync(p => p.PtNum == dto.PtNum))
            {
                return new ProductResult
                {
                    Result = "Error",
                    Message = "Le code produit existe déjà",
                    ProductCode = dto.PtNum
                };
            }

            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_produit_insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Fixed null coalescing issue by using explicit null checks
                command.Parameters.Add(new SqlParameter("@pt_num", SqlDbType.VarChar, 18)
                {
                    Value = dto.PtNum != null ? (object)dto.PtNum : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib", SqlDbType.VarChar, 96)
                {
                    Value = dto.PtLib != null ? (object)dto.PtLib : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib2", SqlDbType.VarChar, 96)
                {
                    Value = dto.PtLib2 != null ? (object)dto.PtLib2 : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@lp_num", SqlDbType.VarChar, 2)
                {
                    Value = dto.LpNum != null ? (object)dto.LpNum : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@fp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.FpCod != null ? (object)dto.FpCod : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_cod", SqlDbType.VarChar, 25)
                {
                    Value = dto.SpCod != null ? (object)dto.SpCod : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@tp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.TpCod != null ? (object)dto.TpCod : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_Id", SqlDbType.VarChar, 6)
                {
                    Value = dto.SpId != null ? (object)dto.SpId : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT14", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT14 != null ? (object)dto.PtSpecifT14 : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_poids", SqlDbType.Float)
                {
                    Value = dto.PtPoids.HasValue ? (object)dto.PtPoids.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_createur", SqlDbType.VarChar, 12)
                {
                    Value = dto.PtCreateur != null ? (object)dto.PtCreateur : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_dcreat", SqlDbType.DateTime)
                {
                    Value = dto.PtDcreat.HasValue ? (object)dto.PtDcreat.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT15", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT15 != null ? (object)dto.PtSpecifT15 : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_flasher", SqlDbType.TinyInt)
                {
                    Value = dto.PtFlasher.HasValue ? (object)dto.PtFlasher.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@is_serialized", SqlDbType.Bit)
                {
                    Value = dto.IsSerialized
                });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ProductResult
                    {
                        Result = reader["Result"]?.ToString() ?? "Error",
                        Message = reader["Message"]?.ToString() ?? "Unknown error",
                        ProductCode = reader["ProductCode"]?.ToString() ?? dto.PtNum
                    };
                }

                return new ProductResult
                {
                    Result = "Error",
                    Message = "No results returned from stored procedure",
                    ProductCode = dto.PtNum
                };
            }
            catch (Exception ex)
            {
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product creation: {ex.Message}",
                    ProductCode = dto.PtNum
                };
            }
        }

        public async Task<ProductResult> DeleteProductAsync(string ptNum)
        {
            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_produit_delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@pt_num", SqlDbType.VarChar, 18)
                {
                    Value = ptNum != null ? (object)ptNum : DBNull.Value
                });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ProductResult
                    {
                        Result = reader["Result"]?.ToString() ?? "Error",
                        Message = reader["Message"]?.ToString() ?? "Unknown error",
                        ProductCode = reader["ProductCode"]?.ToString() ?? ptNum
                    };
                }

                return new ProductResult
                {
                    Result = "Error",
                    Message = "No results returned from stored procedure",
                    ProductCode = ptNum
                };
            }
            catch (Exception ex)
            {
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product deletion: {ex.Message}",
                    ProductCode = ptNum
                };
            }
        }

        public async Task<ProductResult> GetProductAsync(string codeProduit, string status, bool? isSerialized)
        {
            try
            {
                var products = new List<ProduitSerialiséDto>();
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                var command = new SqlCommand("sp_produit_get", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@pt_num", SqlDbType.VarChar, 18)
                {
                    Value = !string.IsNullOrEmpty(codeProduit) ? (object)codeProduit : DBNull.Value
                });

                command.Parameters.Add(new SqlParameter("@sp_id", SqlDbType.VarChar, 6)
                {
                    Value = !string.IsNullOrEmpty(status) ? (object)status : DBNull.Value
                });

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(new ProduitSerialiséDto
                    {
                        PtNum = SafeGetString(reader, "CodeProduit"),
                        PtLib = SafeGetString(reader, "Libellé"),
                        FpCod = SafeGetString(reader, "Famille"),
                        SpCod = SafeGetString(reader, "SousFamille"),
                        SpId = SafeGetString(reader, "Status"),
                        IsSerialized = SafeGetBoolean(reader, "is_serialized"),
                        PtPoids = SafeGetDouble(reader, "Poids"),
                        PtDcreat = SafeGetDateTime(reader, "DateCreation")
                    });
                }

                if (isSerialized.HasValue)
                {
                    products = products.Where(p => p.IsSerialized == isSerialized.Value).ToList();
                }

                return new ProductResult
                {
                    Result = "Success",
                    Message = "Products retrieved successfully",
                    ProductCode = codeProduit,
                    Products = products
                };
            }
            catch (Exception ex)
            {
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Error retrieving product: {ex.Message}"
                };
            }
        }

        private static string SafeGetString(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static bool SafeGetBoolean(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return !reader.IsDBNull(ordinal) && reader.GetBoolean(ordinal);
        }

        private static double? SafeGetDouble(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
        }

        private static DateTime? SafeGetDateTime(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }
    }
}