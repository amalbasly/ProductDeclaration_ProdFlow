// Services/EmployeeService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using ProdFlow.Services.Interfaces;
using ProdFlow.Data;
using ProdFlow.DTOs;

namespace ProdFlow.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PersonnelTracaDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.StoredProcedureResults
                .FromSqlRaw("EXEC GetAllEmployees")
                .ToListAsync();

            return employees.Select(e => new PersonnelTracaDto
            {
                pl_matric = e.pl_matric,
                pl_badge = e.pl_badge,
                pl_nom = e.pl_nom,
                pl_prenom = e.pl_prenom,
                pl_fonc = e.pl_fonc,
                idGrp = e.IDGrp,
                img = e.img,
                Groupe = e.Groupe_IDGrp != null ? new GroupeDto
                {
                    IDGrp = e.Groupe_IDGrp.Value,
                    descriptionGrp = e.Groupe_descriptionGrp
                } : null
            }).ToList();
        }

        public async Task<PersonnelTracaDto> GetEmployeeByMatriculeAsync(int pl_matric)
        {
            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SelectEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@pl_matric", pl_matric));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new PersonnelTracaDto
                            {
                                pl_matric = reader.GetInt64(reader.GetOrdinal("pl_matric")),
                                pl_badge = reader.GetInt64(reader.GetOrdinal("pl_badge")),
                                pl_nom = reader.GetString(reader.GetOrdinal("pl_nom")),
                                pl_prenom = reader.GetString(reader.GetOrdinal("pl_prenom")),
                                pl_fonc = reader.GetString(reader.GetOrdinal("pl_fonc")),
                                idGrp = reader.GetInt32(reader.GetOrdinal("IDGrp")),
                                img = reader.GetString(reader.GetOrdinal("img")),
                                Groupe = !reader.IsDBNull(reader.GetOrdinal("Groupe_IDGrp")) ? new GroupeDto
                                {
                                    IDGrp = reader.GetInt32(reader.GetOrdinal("Groupe_IDGrp")),
                                    descriptionGrp = reader.GetString(reader.GetOrdinal("Groupe_descriptionGrp"))
                                } : null
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int> DeleteEmployeeAsync(long? pl_matric, string? pl_nom, string? pl_prenom)
        {
            var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                if (pl_matric.HasValue)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[DeleteEmployee] @pl_matric = @p0, @pl_nom = NULL, @pl_prenom = NULL, @RowsAffected = @RowsAffected OUT",
                        new SqlParameter("@p0", pl_matric.Value),
                        rowsAffectedParam
                    );
                    return (int)rowsAffectedParam.Value;
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[DeleteEmployee] @pl_matric = NULL, @pl_nom = @p0, @pl_prenom = @p1, @RowsAffected = @RowsAffected OUT",
                    new SqlParameter("@p0", pl_nom),
                    new SqlParameter("@p1", pl_prenom),
                    rowsAffectedParam
                );

                return (int)rowsAffectedParam.Value;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> UpdateEmployeeAsync(int pl_matric, string pl_fonc)
        {
            var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[UpdateEmployee] @pl_matric, @pl_fonc, @RowsAffected OUTPUT",
                new SqlParameter("@pl_matric", pl_matric),
                new SqlParameter("@pl_fonc", pl_fonc),
                rowsAffectedParam
            );

            return (int)rowsAffectedParam.Value;
        }

        public async Task<(long pl_matric, int rowsAffected)> AddEmployeeAsync(
            string pl_nom, string pl_prenom, long pl_badge,
            string pl_fonc, string img, string descriptionGrp)
        {
            var plMatricParam = new SqlParameter("@pl_matric", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };

            var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[AddEmployee] @pl_nom, @pl_prenom, @pl_badge, @pl_fonc, @img, @descriptionGrp, @pl_matric OUTPUT, @RowsAffected OUTPUT",
                new SqlParameter("@pl_nom", pl_nom),
                new SqlParameter("@pl_prenom", pl_prenom),
                new SqlParameter("@pl_badge", pl_badge),
                new SqlParameter("@pl_fonc", pl_fonc),
                new SqlParameter("@img", img),
                new SqlParameter("@descriptionGrp", descriptionGrp),
                plMatricParam,
                rowsAffectedParam
            );

            return ((long)plMatricParam.Value, (int)rowsAffectedParam.Value);
        }
    }
}