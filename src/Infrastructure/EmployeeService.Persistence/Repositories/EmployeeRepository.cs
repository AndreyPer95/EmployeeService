using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnection _connection;

    public EmployeeRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> AddAsync(Employee employee)
    {
        var sql = @"INSERT INTO Employees (Name, Surname, Phone, CompanyId, DepartmentId, PassportId) 
                   VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportId);
                   SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _connection.QuerySingleAsync<int>(sql, employee);
    }

    public async Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId)
    {
        var sql = @"SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.DepartmentId, e.PassportId,
                           d.Id, d.CompanyId, d.Name, d.Phone,
                           p.Id, p.Type, p.Number
                    FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    LEFT JOIN Passports p ON e.PassportId = p.Id
                    WHERE e.CompanyId = @CompanyId";

        var employeeDict = new Dictionary<int, Employee>();

        await _connection.QueryAsync<Employee, Department, Passport, Employee>(
            sql,
            (emp, dept, passport) =>
            {
                if (!employeeDict.TryGetValue(emp.Id, out var employee))
                {
                    employee = emp;
                    employee.Department = dept;
                    employee.Passport = passport;
                    employeeDict.Add(emp.Id, employee);
                }
                return employee;
            },
            new { CompanyId = companyId },
            splitOn: "Id,Id");

        return employeeDict.Values;
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int companyId, string departmentName)
    {
        var sql = @"SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.DepartmentId, e.PassportId,
                           d.Id, d.CompanyId, d.Name, d.Phone,
                           p.Id, p.Type, p.Number
                    FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    LEFT JOIN Passports p ON e.PassportId = p.Id
                    WHERE e.CompanyId = @CompanyId AND d.Name = @DepartmentName";

        var employeeDict = new Dictionary<int, Employee>();

        await _connection.QueryAsync<Employee, Department, Passport, Employee>(
            sql,
            (emp, dept, passport) =>
            {
                if (!employeeDict.TryGetValue(emp.Id, out var employee))
                {
                    employee = emp;
                    employee.Department = dept;
                    employee.Passport = passport;
                    employeeDict.Add(emp.Id, employee);
                }
                return employee;
            },
            new { CompanyId = companyId, DepartmentName = departmentName },
            splitOn: "Id,Id");

        return employeeDict.Values;
    }

    public async Task<bool> PartialUpdateAsync(Employee updateEmployee)
    {
        var sql = @"UPDATE Employees 
               SET Name = @Name, Surname = @Surname, Phone = @Phone, 
                   CompanyId = @CompanyId, DepartmentId = @DepartmentId, PassportId = @PassportId 
               WHERE Id = @Id";

        var rowsAffected = await _connection.ExecuteAsync(sql, updateEmployee);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteWithPassportAsync(int id)
    {
        try
        {
            var employee = await GetByIdAsync(id);
            if (employee == null) return false;
            
            var passportDeleted = await _connection.ExecuteAsync(
                "DELETE FROM Passports WHERE Id = @PassportId",
                new { employee.PassportId });

            var employeeDeleted = await _connection.ExecuteAsync(
                "DELETE FROM Employees WHERE Id = @Id",
                new { Id = id });

            return employeeDeleted > 0;
        }

        catch
        {
            throw;
        }
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Employees WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
    }
}