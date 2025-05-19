using Dapper;
using ProdFlow.DTOs;
using ProdFlow.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace ProdFlow.Services
{
    public class AssemblageService : IAssemblageService
    {
        private readonly string _connectionString;

        public AssemblageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateAssemblageAsync(CreateAssemblageDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@NomAssemblage", dto.NomAssemblage);
            parameters.Add("@MainProduitPtNum", dto.MainProduitPtNum);
            parameters.Add("@GalliaName", dto.GalliaName);
            parameters.Add("@SecondaryProduits", string.Join(",", dto.SecondaryProduitPtNums ?? new List<string>()));
            parameters.Add("@AssemblageId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CreateAssemblage", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@AssemblageId");
        }

        public async Task<AssemblageDto> GetAssemblageByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(
                "GetAssemblageById",
                new { AssemblageId = id },
                commandType: CommandType.StoredProcedure);

            var assemblage = await multi.ReadSingleOrDefaultAsync<AssemblageDto>();
            if (assemblage == null) return null;

            assemblage.SecondaryProduits = (await multi.ReadAsync<SecondaryProduitDto>()).ToList();
            return assemblage;
        }

        public async Task<List<AssemblageDto>> GetAllAssemblagesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var assemblages = await connection.QueryAsync<AssemblageDto>(
                "GetAllAssemblages",
                commandType: CommandType.StoredProcedure);
            return assemblages.ToList();
        }

        public async Task UpdateAssemblageAsync(int id, UpdateAssemblageDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@AssemblageId", id);
            parameters.Add("@NomAssemblage", dto.NomAssemblage);
            parameters.Add("@MainProduitPtNum", dto.MainProduitPtNum);
            parameters.Add("@GalliaName", dto.GalliaName);
            parameters.Add("@SecondaryProduits", string.Join(",", dto.SecondaryProduitPtNums ?? new List<string>()));

            await connection.ExecuteAsync(
                "UpdateAssemblage",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAssemblageAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "DeleteAssemblage",
                new { AssemblageId = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}