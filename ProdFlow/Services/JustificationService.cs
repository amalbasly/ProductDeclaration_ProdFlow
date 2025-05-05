// Services/JustificationService.cs
using Microsoft.Data.SqlClient;
using System.Data;
using ProdFlow.Services.Interfaces;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;

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

        
    }
}