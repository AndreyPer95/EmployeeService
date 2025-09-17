using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;
using System.Data;

namespace EmployeeService.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly IDbConnection _connection;
    
    static DepartmentRepository()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public DepartmentRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        var sql = @"UPDATE departments 
                    SET company_id = @CompanyId, name = @Name, phone = @Phone 
                    WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, department);
        return rowsAffected > 0;
    }

    public async Task<Department> GetByCompanyAndNameAsync(int companyId, string name)
    {
        var sql = "SELECT * FROM departments WHERE company_id = @CompanyId AND name = @Name";
        return await _connection.QuerySingleOrDefaultAsync<Department>(sql, new { CompanyId = companyId, Name = name });
    }
    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        var sql = "SELECT * FROM departments";
        return await _connection.QueryAsync<Department>(sql);
    }
}