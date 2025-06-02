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
using Hangfire;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace ProdFlow.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            AppDbContext context,
            IBackgroundJobClient backgroundJobClient,
            IConfiguration configuration,
            ILogger<ProductService> logger)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
            _configuration = configuration;
            _logger = logger;
        }

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

                command.Parameters.Add(new SqlParameter("@pt_num", SqlDbType.VarChar, 18)
                {
                    Value = dto.PtNum ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib", SqlDbType.NVarChar, 96)
                {
                    Value = dto.PtLib ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib2", SqlDbType.NVarChar, 96)
                {
                    Value = dto.PtLib2 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@lp_num", SqlDbType.VarChar, 2)
                {
                    Value = dto.LpNum ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@fp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.FpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_cod", SqlDbType.VarChar, 25)
                {
                    Value = dto.SpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@tp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.TpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_Id", SqlDbType.VarChar, 6)
                {
                    Value = dto.SpId ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT14", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT14 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_poids", SqlDbType.Float)
                {
                    Value = dto.PtPoids.HasValue ? (object)dto.PtPoids.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_createur", SqlDbType.VarChar, 12)
                {
                    Value = dto.PtCreateur ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_dcreat", SqlDbType.DateTime)
                {
                    Value = DateTime.UtcNow
                });
                command.Parameters.Add(new SqlParameter("@pt_verification_deadline", SqlDbType.DateTime)
                {
                    Value = DateTime.UtcNow.AddDays(3)
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT15", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT15 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_flasher", SqlDbType.TinyInt)
                {
                    Value = dto.PtFlasher.HasValue ? (object)dto.PtFlasher.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@is_serialized", SqlDbType.Bit)
                {
                    Value = dto.IsSerialized
                });
                command.Parameters.Add(new SqlParameter("@GalliaId", SqlDbType.Int)
                {
                    Value = dto.GalliaId.HasValue ? (object)dto.GalliaId.Value : DBNull.Value
                });

                var resultParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                var productCodeParam = new SqlParameter("@ProductCode", SqlDbType.VarChar, 18)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultParam);
                command.Parameters.Add(messageParam);
                command.Parameters.Add(productCodeParam);

                await command.ExecuteNonQueryAsync();

                var productResult = new ProductResult
                {
                    Result = resultParam.Value?.ToString() ?? "Error",
                    Message = messageParam.Value?.ToString() ?? "Unknown error",
                    ProductCode = productCodeParam.Value?.ToString() ?? dto.PtNum,
                    IsSerialized = dto.IsSerialized
                };

                if (productResult.Result != "Success")
                    return productResult;

                // Create verification token
                string token = Guid.NewGuid().ToString();
                string traceabilityManagerId = GetTraceabilityManagerId();
                using (var tokenCmd = new SqlCommand("sp_InsertVerificationToken", connection))
                {
                    tokenCmd.CommandType = CommandType.StoredProcedure;
                    tokenCmd.Parameters.AddWithValue("@pt_num", dto.PtNum);
                    tokenCmd.Parameters.AddWithValue("@Token", token);
                    tokenCmd.Parameters.AddWithValue("@TraceabilityManagerId", traceabilityManagerId);
                    tokenCmd.Parameters.AddWithValue("@ExpiryDate", DateTime.UtcNow.AddDays(3));
                    await tokenCmd.ExecuteNonQueryAsync();
                }

                // Schedule automatic decline
                _backgroundJobClient.Schedule(
                    () => AutoDeclineProductAsync(dto.PtNum, token),
                    TimeSpan.FromDays(3));

                // Send verification email
                var (emailSuccess, emailError) = await SendVerificationEmailAsync(GetManagerEmail(traceabilityManagerId), dto.PtNum, token);
                if (!emailSuccess)
                {
                    _logger.LogWarning("Product created but failed to send verification email for {ProductCode}: {Error}", dto.PtNum, emailError);
                    productResult.Message += $"; Email sending failed: {emailError}";
                }
                else
                {
                    _logger.LogInformation("Email sending successful for product {ProductCode}", dto.PtNum);
                }

                return productResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductCode}", dto.PtNum);
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product creation: {ex.Message}",
                    ProductCode = dto.PtNum,
                    IsSerialized = dto.IsSerialized
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
                    Value = ptNum ?? (object)DBNull.Value
                });

                var resultParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                var productCodeParam = new SqlParameter("@ProductCode", SqlDbType.VarChar, 18)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultParam);
                command.Parameters.Add(messageParam);
                command.Parameters.Add(productCodeParam);

                await command.ExecuteNonQueryAsync();

                return new ProductResult
                {
                    Result = resultParam.Value?.ToString() ?? "Error",
                    Message = messageParam.Value?.ToString() ?? "Unknown error",
                    ProductCode = productCodeParam.Value?.ToString() ?? ptNum
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductCode}", ptNum);
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product deletion: {ex.Message}",
                    ProductCode = ptNum
                };
            }
        }

        public async Task<ProductResult> GetProductAsync(bool isApproved, string codeProduit = null, string status = null, bool? isSerialized = null)
        {
            return await GetProductsAsync(
                codeProduit: codeProduit,
                status: status,
                isSerialized: isSerialized,
                isApproved: isApproved,
                includeJustificationDetails: false
            );
        }

        public async Task<ProductResult> GetProductsAsync(
            string codeProduit = null,
            string status = null,
            bool? isSerialized = null,
            bool? isApproved = null,
            bool includeJustificationDetails = false)
        {
            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
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

                command.Parameters.Add(new SqlParameter("@is_approved", SqlDbType.Bit)
                {
                    Value = isApproved.HasValue ? (object)isApproved.Value : DBNull.Value
                });

                var products = new List<ProduitSerialiséDto>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var product = new ProduitSerialiséDto
                    {
                        PtNum = SafeGetString(reader, "CodeProduit"),
                        PtLib = SafeGetString(reader, "Libellé"),
                        PtLib2 = SafeGetString(reader, "Libellé2"),
                        LpNum = SafeGetString(reader, "Ligne"),
                        FpCod = SafeGetString(reader, "Famille"),
                        SpCod = SafeGetString(reader, "SousFamille"),
                        TpCod = SafeGetString(reader, "Type"),
                        SpId = SafeGetString(reader, "Status"),
                        PtSpecifT14 = SafeGetString(reader, "CodeProduitClientC264"),
                        PtPoids = SafeGetDouble(reader, "Poids"),
                        PtCreateur = SafeGetString(reader, "Createur"),
                        IsSerialized = SafeGetBoolean(reader, "is_serialized"),
                        PtDcreat = SafeGetDateTime(reader, "DateCreation"),
                        PtVerificationDeadline = SafeGetDateTime(reader, "VerificationDeadline"),
                        PtSpecifT15 = SafeGetString(reader, "Tolérances"),
                        PtFlasher = reader.IsDBNull(reader.GetOrdinal("Flashable")) ? null : (byte?)reader.GetByte(reader.GetOrdinal("Flashable")),
                        GalliaId = reader.IsDBNull(reader.GetOrdinal("GalliaId")) ? null : reader.GetInt32(reader.GetOrdinal("GalliaId")),
                        GalliaName = SafeGetString(reader, "GalliaName"),
                        JustificationStatus = SafeGetString(reader, "JustificationStatus"),
                        IsApproved = SafeGetBoolean(reader, "IsApproved")
                    };

                    if (includeJustificationDetails && !string.IsNullOrEmpty(product.PtNum))
                    {
                        product = await LoadJustificationDetails(product, connection);
                    }

                    products.Add(product);
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
                _logger.LogError(ex, "Error retrieving products");
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Error retrieving products: {ex.Message}",
                    ProductCode = codeProduit
                };
            }
        }

        public async Task<ProductResult> GetAllProductsAsync(bool? isSerialized = null)
        {
            return await GetProductsAsync(null, null, isSerialized, null);
        }

        public async Task<ProductResult> GetApprovedProductsAsync(bool? isSerialized = null)
        {
            return await GetProductsAsync(null, null, isSerialized, true);
        }

        public async Task<ProductResult> GetPendingApprovalProductsAsync(bool? isSerialized = null)
        {
            return await GetProductsAsync(null, null, isSerialized, false);
        }

        private async Task<ProduitSerialiséDto> LoadJustificationDetails(ProduitSerialiséDto product, SqlConnection connection)
        {
            try
            {
                using var cmd = new SqlCommand(
                    @"SELECT TOP 1 * FROM Justifications 
              WHERE ProductCode = @ProductCode
              ORDER BY SubmissionDate DESC",
                    connection);

                cmd.Parameters.AddWithValue("@ProductCode", product.PtNum);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    product.JustificationID = SafeGetInt(reader, "JustificationID");
                    product.JustificationText = SafeGetString(reader, "JustificationText");
                    product.UrgencyLevel = SafeGetString(reader, "UrgencyLevel");
                    product.SubmittedBy = SafeGetString(reader, "SubmittedBy");
                    product.SubmissionDate = SafeGetDateTime(reader, "SubmissionDate");
                    product.DecisionComments = SafeGetString(reader, "DecisionComments");
                    product.DecisionDate = SafeGetDateTime(reader, "DecisionDate");
                    product.DecidedBy = SafeGetString(reader, "DecidedBy");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading justification details for product {ProductCode}", product.PtNum);
            }

            return product;
        }

        private static int SafeGetInt(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }

        public async Task<ProductResult> UpdateProductAsync(ProduitSerialiséDto dto)
        {
            if (string.IsNullOrEmpty(dto.PtNum))
            {
                return new ProductResult
                {
                    Result = "Error",
                    Message = "Product code is required",
                    ProductCode = dto.PtNum
                };
            }

            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_produit_update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@pt_num", SqlDbType.VarChar, 18)
                {
                    Value = dto.PtNum ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib", SqlDbType.NVarChar, 96)
                {
                    Value = dto.PtLib ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_lib2", SqlDbType.NVarChar, 96)
                {
                    Value = dto.PtLib2 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@lp_num", SqlDbType.VarChar, 2)
                {
                    Value = dto.LpNum ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@fp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.FpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_cod", SqlDbType.VarChar, 25)
                {
                    Value = dto.SpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@tp_cod", SqlDbType.VarChar, 10)
                {
                    Value = dto.TpCod ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@sp_Id", SqlDbType.VarChar, 6)
                {
                    Value = dto.SpId ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT14", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT14 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_poids", SqlDbType.Float)
                {
                    Value = dto.PtPoids.HasValue ? (object)dto.PtPoids.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_createur", SqlDbType.VarChar, 12)
                {
                    Value = dto.PtCreateur ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_verification_deadline", SqlDbType.DateTime)
                {
                    Value = dto.PtVerificationDeadline.HasValue ? (object)dto.PtVerificationDeadline.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_specifT15", SqlDbType.VarChar, 50)
                {
                    Value = dto.PtSpecifT15 ?? (object)DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@pt_flasher", SqlDbType.TinyInt)
                {
                    Value = dto.PtFlasher.HasValue ? (object)dto.PtFlasher.Value : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@is_serialized", SqlDbType.Bit)
                {
                    Value = dto.IsSerialized
                });
                command.Parameters.Add(new SqlParameter("@GalliaId", SqlDbType.Int)
                {
                    Value = dto.GalliaId.HasValue ? (object)dto.GalliaId.Value : DBNull.Value
                });

                var resultParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                var productCodeParam = new SqlParameter("@ProductCode", SqlDbType.VarChar, 18)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultParam);
                command.Parameters.Add(messageParam);
                command.Parameters.Add(productCodeParam);

                await command.ExecuteNonQueryAsync();

                return new ProductResult
                {
                    Result = resultParam.Value?.ToString() ?? "Error",
                    Message = messageParam.Value?.ToString() ?? "Unknown error",
                    ProductCode = productCodeParam.Value?.ToString() ?? dto.PtNum
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductCode}", dto.PtNum);
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product update: {ex.Message}",
                    ProductCode = dto.PtNum
                };
            }
        }

        public async Task<ProductResult> VerifyProductAsync(VerifyProductDto dto, string managerId)
        {
            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_VerifyProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@pt_num", dto.ProductId);
                command.Parameters.AddWithValue("@Token", dto.Token);
                command.Parameters.AddWithValue("@IsApproved", dto.IsApproved);
                command.Parameters.AddWithValue("@DecisionComments", (object)dto.DecisionComments ?? DBNull.Value);
                command.Parameters.AddWithValue("@DecidedBy", managerId);

                var resultParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultParam);
                command.Parameters.Add(messageParam);

                await command.ExecuteNonQueryAsync();

                return new ProductResult
                {
                    Result = resultParam.Value?.ToString() ?? "Error",
                    Message = messageParam.Value?.ToString() ?? "Unknown error",
                    ProductCode = dto.ProductId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying product {ProductCode}", dto.ProductId);
                return new ProductResult
                {
                    Result = "Error",
                    Message = $"Exception during product verification: {ex.Message}",
                    ProductCode = dto.ProductId
                };
            }
        }

        public async Task AutoDeclineProductAsync(string productId, string token)
        {
            try
            {
                using var connection = (SqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_AutoDeclineProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@pt_num", productId);
                command.Parameters.AddWithValue("@Token", token);

                var resultParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultParam);
                command.Parameters.Add(messageParam);

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Auto-decline for product {ProductCode}: {Message}", productId, messageParam.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-declining product {ProductCode}", productId);
            }
        }

        private async Task<(bool Success, string ErrorMessage)> SendVerificationEmailAsync(string recipientEmail, string productId, string token)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(recipientEmail))
                {
                    _logger.LogError("Recipient email is null or empty for product {ProductCode}", productId);
                    return (false, "Recipient email cannot be null or empty");
                }
                if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogError("ProductId or token is null for product {ProductCode}", productId);
                    return (false, "ProductId or token cannot be null");
                }

                // Validate configuration
                var apiKey = _configuration["EmailSettings:ApiKey"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var frontendUrl = _configuration["FrontendUrl"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("SendGrid API key is null for product {ProductCode}", productId);
                    return (false, "SendGrid API key is not configured");
                }
                if (string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogError("Sender email is null for product {ProductCode}", productId);
                    return (false, "Sender email is not configured");
                }
                if (string.IsNullOrEmpty(senderName))
                {
                    _logger.LogWarning("Sender name is null, using default for product {ProductCode}", productId);
                    senderName = "ProdFlow";
                }
                if (string.IsNullOrEmpty(frontendUrl))
                {
                    _logger.LogError("Frontend URL is null for product {ProductCode}", productId);
                    return (false, "Frontend URL is not configured");
                }

                _logger.LogInformation("Attempting to send verification email for product {ProductCode} to {Recipient} from {Sender}",
                    productId, recipientEmail, senderEmail);

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(recipientEmail);
                var subject = "Verify Product";
                var verificationLink = $"{frontendUrl}/verify-product?productId={productId}&token={token}";
                var htmlContent = $"<p>Verify product {productId}.</p>" +
                                  $"<p><a href='{verificationLink}'>Verify Product</a></p>" +
                                  "<p>Link expires in 3 days.</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                var response = await client.SendEmailAsync(msg);
                var responseBody = await response.Body.ReadAsStringAsync();

                _logger.LogInformation("SendGrid response for product {ProductCode}: StatusCode={StatusCode}, Body={Body}",
                    productId, response.StatusCode, responseBody);

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogWarning("SendGrid failed for product {ProductCode}: StatusCode={StatusCode}, Body={Body}",
                        productId, response.StatusCode, responseBody);
                    return (false, $"SendGrid failed with status {response.StatusCode}: {responseBody}");
                }

                _logger.LogInformation("Verification email sent for product {ProductCode} to {Recipient}", productId, recipientEmail);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification email for product {ProductCode} to {Recipient}", productId, recipientEmail);
                return (false, $"Failed to send email: {ex.Message}");
            }
        }

        private string GetTraceabilityManagerId()
        {
            return _configuration["TraceabilityManagerId"] ?? "manager-id";
        }

        private string GetManagerEmail(string managerId)
        {
            var email = _configuration["TraceabilityManager:Email"];
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("TraceabilityManager:Email is not configured for managerId {ManagerId}", managerId);
                return "amalbasly92@gmail.com"; // Fallback
            }
            return email;
        }

        private static string SafeGetString(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static bool SafeGetBoolean(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return false;

            var value = reader.GetValue(ordinal);
            if (value is bool boolValue)
                return boolValue;
            if (value is int intValue)
                return intValue != 0;
            if (value is byte byteValue)
                return byteValue != 0;

            throw new InvalidCastException($"Cannot cast {value?.GetType().Name} to boolean for column {columnName}");
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

        public async Task<string> TestSendGridEmailAsync()
        {
            try
            {
                var apiKey = _configuration["EmailSettings:ApiKey"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var recipient = "amalbasly92@gmail.com";

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("SendGrid API key is null for test email");
                    return "Test email failed: SendGrid API key is not configured";
                }
                if (string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogError("Sender email is null for test email");
                    return "Test email failed: Sender email is not configured";
                }
                if (string.IsNullOrEmpty(senderName))
                {
                    _logger.LogWarning("Sender name is null, using default for test email");
                    senderName = "ProdFlow";
                }

                _logger.LogInformation("Testing SendGrid email to {Recipient}", recipient);
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(recipient);
                var subject = "Test Email from ProdFlow";
                var htmlContent = "<p>This is a test email from ProdFlow.</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = await client.SendEmailAsync(msg);
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogInformation("Test email response: StatusCode={StatusCode}, Body={Body}", response.StatusCode, responseBody);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted
                    ? "Test email sent successfully"
                    : $"Test email failed: {response.StatusCode}, {responseBody}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                return $"Test email error: {ex.Message}";
            }
        }
    }
}