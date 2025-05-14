using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProdFlow.Data;
using ProdFlow.DTOs;
using ProdFlow.Models.Entities;
using ProdFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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
            var gallias = new List<GalliaDto>();
            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(@"
            SELECT g.*, gi.LabelImage 
            FROM Gallia g
            LEFT JOIN GalliaImages gi ON g.GalliaId = gi.GalliaId
            ORDER BY g.CreatedAt DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            gallias.Add(new GalliaDto
                            {
                                GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                LabelDate = reader.IsDBNull(reader.GetOrdinal("LabelDate")) ?
                                    (DateTime?)null :
                                    reader.GetDateTime(reader.GetOrdinal("LabelDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                LabelImage = reader.IsDBNull(reader.GetOrdinal("LabelImage")) ?
                                    null :
                                    reader.GetString(reader.GetOrdinal("LabelImage")),
                                Fields = new List<GalliaFieldDto>()
                            });
                        }
                    }
                }

                foreach (var gallia in gallias)
                {
                    using (var fieldCmd = new SqlCommand(
                        "SELECT * FROM GalliaField WHERE GalliaId = @GalliaId ORDER BY DisplayOrder",
                        connection))
                    {
                        fieldCmd.Parameters.AddWithValue("@GalliaId", gallia.GalliaId);
                        using (var fieldReader = await fieldCmd.ExecuteReaderAsync())
                        {
                            while (await fieldReader.ReadAsync())
                            {
                                gallia.Fields.Add(new GalliaFieldDto
                                {
                                    GalliaFieldId = fieldReader.GetInt32(fieldReader.GetOrdinal("GalliaFieldId")),
                                    GalliaId = fieldReader.GetInt32(fieldReader.GetOrdinal("GalliaId")),
                                    FieldName = fieldReader.IsDBNull(fieldReader.GetOrdinal("FieldName")) ?
                                        null :
                                        fieldReader.GetString(fieldReader.GetOrdinal("FieldName")),
                                    FieldValue = fieldReader.GetString(fieldReader.GetOrdinal("FieldValue")),
                                    DisplayOrder = fieldReader.GetInt32(fieldReader.GetOrdinal("DisplayOrder")),
                                    VisualizationType = fieldReader.GetString(fieldReader.GetOrdinal("VisualizationType"))
                                });
                            }
                        }
                    }
                }
            }

            return gallias;
        }

        public async Task<GalliaDto> GetGalliaByIdAsync(int id)
        {
            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                GalliaDto galliaDto = null;

                using (var command = new SqlCommand("sp_gallia_get_by_id", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@GalliaId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // First result set - main gallia data
                        if (await reader.ReadAsync())
                        {
                            galliaDto = new GalliaDto
                            {
                                GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                LabelDate = reader.IsDBNull(reader.GetOrdinal("LabelDate")) ?
                                    (DateTime?)null :
                                    reader.GetDateTime(reader.GetOrdinal("LabelDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                LabelImage = reader.IsDBNull(reader.GetOrdinal("LabelImage")) ?
                                    null :
                                    reader.GetString(reader.GetOrdinal("LabelImage")),
                                Fields = new List<GalliaFieldDto>()
                            };
                        }

                        // Move to next result set - fields data
                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                        {
                            galliaDto.Fields.Add(new GalliaFieldDto
                            {
                                GalliaFieldId = reader.GetInt32(reader.GetOrdinal("GalliaFieldId")),
                                GalliaId = reader.GetInt32(reader.GetOrdinal("GalliaId")),
                                FieldName = reader.IsDBNull(reader.GetOrdinal("FieldName")) ?
                                    null :
                                    reader.GetString(reader.GetOrdinal("FieldName")),
                                FieldValue = reader.GetString(reader.GetOrdinal("FieldValue")),
                                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                                VisualizationType = reader.GetString(reader.GetOrdinal("VisualizationType"))
                            });
                        }
                    }
                }

                return galliaDto;
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
                    int galliaId;

                    using (var cmdInsert = new SqlCommand("sp_gallia_insert", connection))
                    {
                        cmdInsert.CommandType = CommandType.StoredProcedure;
                        cmdInsert.Parameters.AddWithValue("@LabelDate", (object?)createDto.LabelDate ?? DBNull.Value);
                        galliaId = Convert.ToInt32(await cmdInsert.ExecuteScalarAsync());
                    }

                    if (createDto.Fields != null && createDto.Fields.Count > 0)
                    {
                        foreach (var field in createDto.Fields)
                        {
                            using (var cmdField = new SqlCommand("sp_galliafield_insert", connection))
                            {
                                cmdField.CommandType = CommandType.StoredProcedure;
                                cmdField.Parameters.AddWithValue("@GalliaId", galliaId);
                                cmdField.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? "");
                                cmdField.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                                cmdField.Parameters.AddWithValue("@VisualizationType", field.VisualizationType ?? "qrcode");
                                cmdField.Parameters.AddWithValue("@FieldName", (object?)field.FieldName ?? DBNull.Value);

                                await cmdField.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    return await GetGalliaByIdAsync(galliaId);
                }
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

                    using (var cmdUpdate = new SqlCommand("sp_gallia_update", connection))
                    {
                        cmdUpdate.CommandType = CommandType.StoredProcedure;
                        cmdUpdate.Parameters.AddWithValue("@GalliaId", updateDto.GalliaId);
                        cmdUpdate.Parameters.AddWithValue("@LabelDate", (object?)updateDto.LabelDate ?? DBNull.Value);
                        await cmdUpdate.ExecuteNonQueryAsync();
                    }

                    using (var deleteCmd = new SqlCommand("DELETE FROM GalliaField WHERE GalliaId = @GalliaId", connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@GalliaId", updateDto.GalliaId);
                        await deleteCmd.ExecuteNonQueryAsync();
                    }

                    if (updateDto.Fields != null && updateDto.Fields.Count > 0)
                    {
                        foreach (var field in updateDto.Fields)
                        {
                            using (var cmdField = new SqlCommand("sp_galliafield_insert", connection))
                            {
                                cmdField.CommandType = CommandType.StoredProcedure;
                                cmdField.Parameters.AddWithValue("@GalliaId", updateDto.GalliaId);
                                cmdField.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? "");
                                cmdField.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                                cmdField.Parameters.AddWithValue("@VisualizationType", field.VisualizationType ?? "qrcode");
                                cmdField.Parameters.AddWithValue("@FieldName", (object?)field.FieldName ?? DBNull.Value);

                                await cmdField.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating Gallia with ID {updateDto.GalliaId}");
                throw;
            }
        }

        public async Task<bool> DeleteGalliaAsync(int id)
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
                        command.Parameters.AddWithValue("@GalliaId", id);

                        // ExecuteNonQueryAsync returns the number of rows affected
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        // Stored procedure deletes from two tables, so we check if at least 1 row was affected
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Gallia with ID {id}");
                throw new Exception("An error occurred while deleting the Gallia", ex);
            }
        }



        // GalliaService.cs - Update the SaveLabelImageAsync method
        public async Task SaveLabelImageAsync(int galliaId, string base64Image)
        {
            try
            {
                await using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("sp_galliaimage_insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@GalliaId", galliaId);
                command.Parameters.AddWithValue("@LabelImage", base64Image);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error saving image for Gallia {GalliaId}", galliaId);
                throw;
            }
        }
    }
}