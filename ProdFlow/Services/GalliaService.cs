using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Models.Entities;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFlow.Services
{
    public class GalliaService : IGalliaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GalliaService> _logger;

        public GalliaService(AppDbContext context, ILogger<GalliaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GalliaDto>> GetAllGalliasAsync()
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_gallia_get_all", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var gallias = new List<Gallia>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                gallias.Add(new Gallia
                                {
                                    GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                    PLIB1 = reader.IsDBNull(reader.GetOrdinal("PLIB1")) ? null : reader.GetString(reader.GetOrdinal("PLIB1")),
                                    QLIB3 = reader.IsDBNull(reader.GetOrdinal("QLIB3")) ? null : reader.GetString(reader.GetOrdinal("QLIB3")),
                                    LIB1 = reader.IsDBNull(reader.GetOrdinal("LIB1")) ? null : reader.GetString(reader.GetOrdinal("LIB1")),
                                    LIB2 = reader.IsDBNull(reader.GetOrdinal("LIB2")) ? null : reader.GetString(reader.GetOrdinal("LIB2")),
                                    LIB3 = reader.IsDBNull(reader.GetOrdinal("LIB3")) ? null : reader.GetString(reader.GetOrdinal("LIB3")),
                                    LIB4 = reader.IsDBNull(reader.GetOrdinal("LIB4")) ? null : reader.GetString(reader.GetOrdinal("LIB4")),
                                    LIB5 = reader.IsDBNull(reader.GetOrdinal("LIB5")) ? null : reader.GetString(reader.GetOrdinal("LIB5")),
                                    LIB6 = reader.IsDBNull(reader.GetOrdinal("LIB6")) ? null : reader.GetString(reader.GetOrdinal("LIB6")),
                                    LIB7 = reader.IsDBNull(reader.GetOrdinal("LIB7")) ? null : reader.GetString(reader.GetOrdinal("LIB7")),
                                    SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                                    LabelDate = reader.IsDBNull(reader.GetOrdinal("LabelDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LabelDate")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                });
                            }
                        }
                        return gallias.Select(MapToDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all gallias");
                throw;
            }
        }

        public async Task<GalliaDto> GetGalliaByIdAsync(int id)
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_gallia_get_by_id", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GalliaId", id));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new GalliaDto
                                {
                                    GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                    PLIB1 = reader.IsDBNull(reader.GetOrdinal("PLIB1")) ? null : reader.GetString(reader.GetOrdinal("PLIB1")),
                                    QLIB3 = reader.IsDBNull(reader.GetOrdinal("QLIB3")) ? null : reader.GetString(reader.GetOrdinal("QLIB3")),
                                    LIB1 = reader.IsDBNull(reader.GetOrdinal("LIB1")) ? null : reader.GetString(reader.GetOrdinal("LIB1")),
                                    LIB2 = reader.IsDBNull(reader.GetOrdinal("LIB2")) ? null : reader.GetString(reader.GetOrdinal("LIB2")),
                                    LIB3 = reader.IsDBNull(reader.GetOrdinal("LIB3")) ? null : reader.GetString(reader.GetOrdinal("LIB3")),
                                    LIB4 = reader.IsDBNull(reader.GetOrdinal("LIB4")) ? null : reader.GetString(reader.GetOrdinal("LIB4")),
                                    LIB5 = reader.IsDBNull(reader.GetOrdinal("LIB5")) ? null : reader.GetString(reader.GetOrdinal("LIB5")),
                                    LIB6 = reader.IsDBNull(reader.GetOrdinal("LIB6")) ? null : reader.GetString(reader.GetOrdinal("LIB6")),
                                    LIB7 = reader.IsDBNull(reader.GetOrdinal("LIB7")) ? null : reader.GetString(reader.GetOrdinal("LIB7")),
                                    SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                                    LabelDate = reader.IsDBNull(reader.GetOrdinal("LabelDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LabelDate")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving gallia with ID {id}");
                throw;
            }
        }

        public async Task<GalliaDto> CreateGalliaAsync(CreateGalliaDto createDto)
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_gallia_insert", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@PLIB1", createDto.PLIB1 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@QLIB3", createDto.QLIB3 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB1", createDto.LIB1 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB2", createDto.LIB2 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB3", createDto.LIB3 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB4", createDto.LIB4 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB5", createDto.LIB5 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB6", createDto.LIB6 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB7", createDto.LIB7 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@SupplierName", createDto.SupplierName ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LabelDate", createDto.LabelDate ?? (object)DBNull.Value));

                        await command.ExecuteNonQueryAsync();
                    }

                    // Get the newly created Gallia
                    using (var command = new SqlCommand("SELECT TOP 1 * FROM Gallia ORDER BY GalliaId DESC", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new GalliaDto
                                {
                                    GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                    PLIB1 = reader.IsDBNull(reader.GetOrdinal("PLIB1")) ? null : reader.GetString(reader.GetOrdinal("PLIB1")),
                                    QLIB3 = reader.IsDBNull(reader.GetOrdinal("QLIB3")) ? null : reader.GetString(reader.GetOrdinal("QLIB3")),
                                    LIB1 = reader.IsDBNull(reader.GetOrdinal("LIB1")) ? null : reader.GetString(reader.GetOrdinal("LIB1")),
                                    LIB2 = reader.IsDBNull(reader.GetOrdinal("LIB2")) ? null : reader.GetString(reader.GetOrdinal("LIB2")),
                                    LIB3 = reader.IsDBNull(reader.GetOrdinal("LIB3")) ? null : reader.GetString(reader.GetOrdinal("LIB3")),
                                    LIB4 = reader.IsDBNull(reader.GetOrdinal("LIB4")) ? null : reader.GetString(reader.GetOrdinal("LIB4")),
                                    LIB5 = reader.IsDBNull(reader.GetOrdinal("LIB5")) ? null : reader.GetString(reader.GetOrdinal("LIB5")),
                                    LIB6 = reader.IsDBNull(reader.GetOrdinal("LIB6")) ? null : reader.GetString(reader.GetOrdinal("LIB6")),
                                    LIB7 = reader.IsDBNull(reader.GetOrdinal("LIB7")) ? null : reader.GetString(reader.GetOrdinal("LIB7")),
                                    SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                                    LabelDate = reader.IsDBNull(reader.GetOrdinal("LabelDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LabelDate")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                };
                            }
                        }
                    }
                }
                throw new Exception("Failed to retrieve newly created Gallia");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating gallia");
                throw;
            }
        }

        public async Task UpdateGalliaAsync(UpdateGalliaDto updateDto)
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_gallia_update", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@GalliaId", updateDto.GalliaId));
                        command.Parameters.Add(new SqlParameter("@PLIB1", updateDto.PLIB1 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@QLIB3", updateDto.QLIB3 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB1", updateDto.LIB1 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB2", updateDto.LIB2 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB3", updateDto.LIB3 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB4", updateDto.LIB4 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB5", updateDto.LIB5 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB6", updateDto.LIB6 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LIB7", updateDto.LIB7 ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@SupplierName", updateDto.SupplierName ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LabelDate", updateDto.LabelDate ?? (object)DBNull.Value));

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"No gallia found with ID {updateDto.GalliaId} to update");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating gallia with ID {updateDto.GalliaId}");
                throw;
            }
        }

        public async Task DeleteGalliaAsync(int id)
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_gallia_delete", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@GalliaId", id));

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"No gallia found with ID {id} to delete");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting gallia with ID {id}");
                throw;
            }
        }

        private static GalliaDto MapToDto(Gallia gallia)
        {
            return new GalliaDto
            {
                GalliaId = gallia.GalliaId,
                PLIB1 = gallia.PLIB1,
                QLIB3 = gallia.QLIB3,
                LIB1 = gallia.LIB1,
                LIB2 = gallia.LIB2,
                LIB3 = gallia.LIB3,
                LIB4 = gallia.LIB4,
                LIB5 = gallia.LIB5,
                LIB6 = gallia.LIB6,
                LIB7 = gallia.LIB7,
                SupplierName = gallia.SupplierName,
                LabelDate = gallia.LabelDate,
                CreatedAt = gallia.CreatedAt
            };
        }
    }
}