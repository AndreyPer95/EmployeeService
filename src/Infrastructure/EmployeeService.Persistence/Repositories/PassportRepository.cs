using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class PassportRepository : IPassportRepository
{
    private readonly IDbConnection _connection;

    public PassportRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> AddAsync(Passport passport)
    {
        var sql = @"INSERT INTO passports (type, number) 
                    VALUES (@Type, @Number) 
                    RETURNING id";
        return await _connection.QuerySingleAsync<int>(sql, passport);
    }

    public async Task<bool> UpdateAsync(Passport passport)
    {
        var sql = @"UPDATE passports 
                    SET type = @Type, number = @Number 
                    WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, passport);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Passport>> GetAllAsync()
    {
        var sql = "SELECT * FROM passports";
        return await _connection.QueryAsync<Passport>(sql);
    }
}