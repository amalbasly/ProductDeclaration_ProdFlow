// Services/SynoptiqueService.cs
using Microsoft.Data.SqlClient;
using ProdFlow.DTOs;
using ProdFlow.Models.Requests;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;
using ProdFlow.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProdFlow.Services
{
    public class SynoptiqueService : ISynoptiqueService
    {
        private readonly string _connectionString;

        public SynoptiqueService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ApplicationException("Database connection string is not configured");
        }

        public async Task<IEnumerable<string>> GetSerializedProductsAsync()
        {
            var products = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT pt_num FROM produit WHERE is_serialized = 1", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return products;
        }

        public async Task<IEnumerable<ModeDto>> GetAllModesAsync()
        {
            var modes = new List<ModeDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetAllModes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            modes.Add(new ModeDto
                            {
                                ID = reader.GetInt32(0),
                                NomMode = reader.IsDBNull(1) ? null : reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return modes;
        }

        public async Task<IEnumerable<SynoptiqueEntryDto>> GetSynoptiqueForProductAsync(string ptNum)
        {
            var entries = new List<SynoptiqueEntryDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetSynoptiqueByProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pt_num", ptNum);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            entries.Add(new SynoptiqueEntryDto
                            {
                                ModeID = reader.GetInt32(0),
                                PtNum = reader.GetString(1),
                                NomMvt = reader.GetString(3),
                                Ordre = reader.GetInt32(4),
                                Matricule = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return entries;
        }

        public async Task<SynoptiqueSaveResult> SaveSynoptiqueAsync(SynoptiqueSaveRequest request)
        {
            var result = new SynoptiqueSaveResult { ProductCode = request.PtNum };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Verify product exists and is serialized
                using (var checkCmd = new SqlCommand("sp_CheckProductType", connection))
                {
                    checkCmd.CommandType = CommandType.StoredProcedure;
                    checkCmd.Parameters.AddWithValue("@pt_num", request.PtNum);

                    using (var reader = await checkCmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                        {
                            result.Success = false;
                            result.Message = $"Product {request.PtNum} not found";
                            return result;
                        }

                        if (!Convert.ToBoolean(reader["IsSerialized"]))
                        {
                            result.Success = false;
                            result.Message = $"Product {request.PtNum} is not serialized";
                            return result;
                        }
                    }
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete existing entries
                        var deleteCmd = new SqlCommand(
                            "DELETE FROM SynoptiqueProd WHERE pt_num = @PtNum",
                            connection,
                            transaction);
                        deleteCmd.Parameters.AddWithValue("@PtNum", request.PtNum);
                        result.DeletedEntries = await deleteCmd.ExecuteNonQueryAsync();

                        // Insert new entries
                        result.InsertedEntries = 0;
                        foreach (var entry in request.Entries.OrderBy(e => e.Ordre))
                        {
                            var insertCmd = new SqlCommand(
                                "sp_InsertSynoptiqueEntry",
                                connection,
                                transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            insertCmd.Parameters.AddWithValue("@ID", entry.ModeID);
                            insertCmd.Parameters.AddWithValue("@pt_num", request.PtNum);
                            insertCmd.Parameters.AddWithValue("@NomMvt", entry.NomMvt ?? string.Empty);
                            insertCmd.Parameters.AddWithValue("@Ordre", entry.Ordre);
                            insertCmd.Parameters.AddWithValue("@Matricule", request.Matricule ?? "SYSTEM");

                            result.InsertedEntries += await insertCmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        result.Success = true;
                        result.Message = "Synoptique saved successfully";
                        return result;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}