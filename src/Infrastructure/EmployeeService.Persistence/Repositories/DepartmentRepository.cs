using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(IDbConnection connection) : base(connection, "Departments")
    {
    }

    public override async Task<int> AddAsync(Department department)
    {
        var sql = @"INSERT INTO Departments (CompanyId, Name, Phone) 
                   VALUES (@CompanyId, @Name, @Phone);
                   SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _connection.QuerySingleAsync<int>(sql, department);
    }

    public override async Task<bool> UpdateAsync(Department department)
    {
        var sql = @"UPDATE Departments 
                   SET CompanyId = @CompanyId, Name = @Name, Phone = @Phone 
                   WHERE Id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, department);
        return rowsAffected > 0;
    }

    public async Task<Department> GetByCompanyAndNameAsync(int companyId, string name)
    {
        var sql = "SELECT * FROM Departments WHERE CompanyId = @CompanyId AND Name = @Name";
        return await _connection.QuerySingleOrDefaultAsync<Department>(sql, new { CompanyId = companyId, Name = name });
    }

    public async Task<Department> GetOrCreateDepartmentAsync(int companyId, string name, string phone)
    {
        var existing = await GetByCompanyAndNameAsync(companyId, name);
        if (existing != null)
        {
            if (existing.Phone != phone)
            {
                existing.Phone = phone;
                await UpdateAsync(existing);
            }
            return existing;
        }

        var department = new Department
        {
            CompanyId = companyId,
            Name = name,
            Phone = phone
        };
        department.Id = await AddAsync(department);
        return department;
    }
}