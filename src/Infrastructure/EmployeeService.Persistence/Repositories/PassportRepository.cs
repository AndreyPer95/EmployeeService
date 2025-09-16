using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class PassportRepository : BaseRepository<Passport>, IPassportRepository
{
    public PassportRepository(IDbConnection connection) : base(connection, "Passports")
    {
    }

    public override async Task<int> AddAsync(Passport passport)
    {
        var sql = @"INSERT INTO Passports (Type, Number) 
                   VALUES (@Type, @Number);
                   SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _connection.QuerySingleAsync<int>(sql, passport);
    }

    public override async Task<bool> UpdateAsync(Passport passport)
    {
        var sql = @"UPDATE Passports 
                   SET Type = @Type, Number = @Number 
                   WHERE Id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, passport);
        return rowsAffected > 0;
    }

    public async Task<Passport> GetByTypeAndNumberAsync(string type, string number)
    {
        var sql = "SELECT * FROM Passports WHERE Type = @Type AND Number = @Number";
        return await _connection.QuerySingleOrDefaultAsync<Passport>(sql, new { Type = type, Number = number });
    }

    public async Task<Passport> GetOrCreatePassportAsync(string type, string number)
    {
        var existing = await GetByTypeAndNumberAsync(type, number);
        if (existing != null)
            return existing;

        var passport = new Passport
        {
            Type = type,
            Number = number
        };
        passport.Id = await AddAsync(passport);
        return passport;
    }
}