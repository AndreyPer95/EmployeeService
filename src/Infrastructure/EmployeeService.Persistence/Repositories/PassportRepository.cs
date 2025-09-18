using Dapper;
using EmployeeService.Domain.Entities;
using System.Data;

namespace EmployeeService.Persistence.Repositories;

public class PassportRepository : BaseRepository<Passport>
{
    private readonly IDbConnection _connection;
    static PassportRepository()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public PassportRepository(IDbConnection connection) : base(connection, "passports")
    {
        _connection = connection;
    }

    public override async Task<int> AddAsync(Passport passport)
    {
        var sql = @"INSERT INTO passports (type, number) 
                    VALUES (@Type, @Number) 
                    RETURNING id";
        return await _connection.QuerySingleAsync<int>(sql, passport);
    }

    public override async Task<bool> UpdateAsync(Passport passport)
    {
        var sql = @"UPDATE passports 
                    SET type = @Type, number = @Number 
                    WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, passport);
        return rowsAffected > 0;
    }

}