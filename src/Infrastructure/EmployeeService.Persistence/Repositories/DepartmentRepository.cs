using Dapper;
using EmployeeService.Domain.Entities;
using System.Data;

namespace EmployeeService.Persistence.Repositories;

public class DepartmentRepository : BaseRepository<Department>
{
    private readonly IDbConnection _connection;
    
    static DepartmentRepository()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public DepartmentRepository(IDbConnection connection) : base(connection, "departments")
    {
        _connection = connection;
    }

    public override async Task<int> AddAsync(Department department)
    {
        var sql = @"INSERT INTO departments (name, phone) 
                    VALUES (@Name, @Phone) 
                    RETURNING id";
        return await _connection.QuerySingleAsync<int>(sql, department);
    }

    public override async Task<bool> UpdateAsync(Department department)
    {
        var sql = @"UPDATE departments 
                    SET company_id = @CompanyId, name = @Name, phone = @Phone 
                    WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, department);
        return rowsAffected > 0;
    }
}