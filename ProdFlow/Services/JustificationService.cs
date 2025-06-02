using Microsoft.Data.SqlClient;
using System.Data;
using ProdFlow.Services.Interfaces;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;
using System.Collections.Generic;

namespace ProdFlow.Services
{
    public class JustificationService : IJustificationService
    {
        private readonly string _connectionString;
        private readonly ILogger<JustificationService> _logger;

        public JustificationService(
            IConfiguration configuration,
            ILogger<JustificationService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<List<JustificationDto>> GetJustificationsAsync(
    string productCode = null,
    string status = null,
    string submittedBy = null)
        {
            var justifications = new List<JustificationDto>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_GetJustifications", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@ProductCode", SqlDbType.VarChar, 18).Value =
                            !string.IsNullOrEmpty(productCode) ? (object)productCode : DBNull.Value;

                        command.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value =
                            !string.IsNullOrEmpty(status) ? (object)status : DBNull.Value;

                        command.Parameters.Add("@SubmittedBy", SqlDbType.VarChar, 50).Value =
                            !string.IsNullOrEmpty(submittedBy) ? (object)submittedBy : DBNull.Value;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var submissionDateOrdinal = reader.GetOrdinal("SubmissionDate");
                                var decisionDateOrdinal = reader.GetOrdinal("DecisionDate");

                                justifications.Add(new JustificationDto
                                {
                                    JustificationID = reader.GetInt32(reader.GetOrdinal("JustificationID")),
                                    ProductCode = SafeGetString(reader, "ProductCode"),
                                    ProductName = SafeGetString(reader, "ProductName"),
                                    JustificationText = SafeGetString(reader, "JustificationText"),
                                    UrgencyLevel = SafeGetString(reader, "UrgencyLevel"),
                                    Status = SafeGetString(reader, "Status"),
                                    SubmittedBy = SafeGetString(reader, "SubmittedBy"),
                                    SubmissionDate = reader.GetDateTime(submissionDateOrdinal), // Non-nullable
                                    DecisionComments = SafeGetString(reader, "DecisionComments"),
                                    DecisionDate = reader.IsDBNull(decisionDateOrdinal) ?
                                        null : (DateTime?)reader.GetDateTime(decisionDateOrdinal), // Nullable
                                    DecidedBy = SafeGetString(reader, "DecidedBy")
                                });
                            }
                        }
                    }
                }

                return justifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving justifications");
                throw;
            }
        }

        public async Task<JustificationResponse> SubmitJustificationAsync(CreateJustificationDto justificationDto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    if (!await ProductExistsAsync(justificationDto.ProductCode))
                    {
                        throw new ArgumentException($"Product {justificationDto.ProductCode} not found");
                    }

                    using (var command = new SqlCommand("sp_SubmitJustification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@ProductCode", SqlDbType.VarChar, 18).Value = justificationDto.ProductCode;
                        command.Parameters.Add("@JustificationText", SqlDbType.NVarChar, -1).Value = justificationDto.JustificationText;
                        command.Parameters.Add("@UrgencyLevel", SqlDbType.VarChar, 20).Value = justificationDto.UrgencyLevel;
                        command.Parameters.Add("@SubmittedBy", SqlDbType.VarChar, 50).Value = justificationDto.SubmittedBy;

                        var justificationId = await command.ExecuteScalarAsync();

                        _logger.LogInformation("Justification submitted successfully. ID: {JustificationID}", justificationId);

                        return new JustificationResponse
                        {
                            JustificationID = Convert.ToInt32(justificationId),
                            ProductCode = justificationDto.ProductCode,
                            Status = "Pending",
                            SubmissionDate = DateTime.UtcNow,
                            Message = "Justification submitted successfully"
                        };
                    }
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while submitting justification");
                throw;
            }
        }

        public async Task<bool> ProductExistsAsync(string productCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "SELECT 1 FROM [dbo].[produit] WHERE pt_num = @ProductCode",
                    connection))
                {
                    command.Parameters.Add("@ProductCode", SqlDbType.VarChar, 18).Value = productCode;
                    return await command.ExecuteScalarAsync() != null;
                }
            }
        }
        private static string SafeGetString(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static DateTime? SafeGetDateTime(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }
    }


}
