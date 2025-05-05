/*// Services/ProfileService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using ProdFlow.Services.Interfaces;
using System.Data;
namespace ProdFlow.Services
{
    public class ProfileService : IProfileService
    {
        private readonly string _connectionString;

        public ProfileService(IConfiguration config)
        {
            // Fix for CS8601: Ensure GetConnectionString does not return null
            _connectionString = config.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        public async Task<int> UpdateProfileAsync(
            int pl_matric,
            string? pl_nom = null,
            string? pl_prenom = null,
            IFormFile? img = null)
        {
            string base64Image = null;
            if (img != null && img.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await img.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("UpdateUserProfile", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@pl_matric", pl_matric);
            command.Parameters.AddWithValue("@pl_nom", string.IsNullOrEmpty(pl_nom) ? DBNull.Value : (object)pl_nom);
            command.Parameters.AddWithValue("@pl_prenom", string.IsNullOrEmpty(pl_prenom) ? DBNull.Value : (object)pl_prenom);
            command.Parameters.AddWithValue("@img", string.IsNullOrEmpty(base64Image) ? DBNull.Value : (object)base64Image);

            var returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            await command.ExecuteNonQueryAsync();
            return (int)returnParam.Value;
        }
    }
}*/