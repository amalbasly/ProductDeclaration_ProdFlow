using Dapper;
using Microsoft.Data.SqlClient;
using ProdFlow.DTOs;
using ProdFlow.Models.Responses;
using ProdFlow.Services.Interfaces;
using System.Data;

namespace ProdFlow.Services
{
    public class ClientReferenceService : IClientReferenceService
    {
        private readonly string _connectionString;

        public ClientReferenceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        }

        public async Task<ClientReferenceResponse> CreateAsync(string ptNum, ClientReferenceCreateDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("pt_num", ptNum);
            parameters.Add("client_reference", dto.ClientReference);
            parameters.Add("client_index", dto.ClientIndex);
            parameters.Add("client", dto.Client);
            parameters.Add("referentiel", dto.Referentiel);

            try
            {
                await connection.ExecuteAsync(
                    "InsertClientReference",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return await GetByPtNumAsync(ptNum);
            }
            catch (SqlException ex)
            {
                throw HandleSqlException(ex, ptNum);
            }
        }

        public async Task<ClientReferenceResponse> UpdateAsync(string ptNum, ClientReferenceUpdateDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("pt_num", ptNum);
            parameters.Add("client_reference", dto.ClientReference);
            parameters.Add("client_index", dto.ClientIndex);
            parameters.Add("client", dto.Client);
            parameters.Add("referentiel", dto.Referentiel);

            try
            {
                await connection.ExecuteAsync(
                    "UpdateClientReference",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return await GetByPtNumAsync(ptNum);
            }
            catch (SqlException ex)
            {
                throw HandleSqlException(ex, ptNum);
            }
        }

        public async Task<string> DeleteAsync(string ptNum)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                int affectedRows = await connection.ExecuteAsync(
                    "DeleteClientReference",
                    new { pt_num = ptNum },
                    commandType: CommandType.StoredProcedure);

                if (affectedRows == 0)
                {
                    throw new KeyNotFoundException($"No reference found for product {ptNum}");
                }

                return $"Product with pt_num = {ptNum} deleted successfully";
            }
            catch (SqlException ex)
            {
                throw HandleSqlException(ex, ptNum);
            }
        }

        public async Task<ClientReferenceResponse> GetByPtNumAsync(string ptNum)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ClientReferenceResponse>(
                "GetClientReferenceById",
                new { pt_num = ptNum },
                commandType: CommandType.StoredProcedure);
        }

        private Exception HandleSqlException(SqlException ex, string ptNum)
        {
            return ex.Number switch
            {
                50000 => new InvalidOperationException($"Operation failed for product {ptNum}: {ex.Message}"),
                _ => new Exception("Database error occurred", ex)
            };
        }
    }
}