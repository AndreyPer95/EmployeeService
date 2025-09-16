using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(IDbConnection connection) : base(connection, "Employees")
    {
    }

    public override async Task<int> AddAsync(Employee employee)
    {
        var sql = @"INSERT INTO Employees (Name, Surname, Phone, CompanyId, DepartmentId, PassportId) 
                   VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportId);
                   SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _connection.QuerySingleAsync<int>(sql, employee);
    }

    public override async Task<bool> UpdateAsync(Employee employee)
    {
        var sql = @"UPDATE Employees 
                   SET Name = @Name, Surname = @Surname, Phone = @Phone, 
                       CompanyId = @CompanyId, DepartmentId = @DepartmentId, PassportId = @PassportId 
                   WHERE Id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, employee);
        return rowsAffected > 0;
    }

    public async Task<Employee> GetEmployeeWithDetailsAsync(int id)
    {
        var sql = @"SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.DepartmentId, e.PassportId,
                           d.Id, d.CompanyId, d.Name, d.Phone,
                           p.Id, p.Type, p.Number
                    FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    LEFT JOIN Passports p ON e.PassportId = p.Id
                    WHERE e.Id = @Id";

        var employee = await _connection.QueryAsync<Employee, Department, Passport, Employee>(
            sql,
            (emp, dept, passport) =>
            {
                emp.Department = dept;
                emp.Passport = passport;
                return emp;
            },
            new { Id = id },
            splitOn: "Id,Id");

        return employee.FirstOrDefault();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId)
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

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName)
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

    public async Task<bool> PartialUpdateEmployeeAsync(int id, Dictionary<string, object> updates)
    {
        if (!updates.Any())
            return false;

        var fieldsToUpdate = string.Join(", ", updates.Keys.Select(key => $"{key} = @{key}"));
        var sql = $"UPDATE Employees SET {fieldsToUpdate} WHERE Id = @Id";

        var parameters = new DynamicParameters();
        parameters.Add("Id", id);
        foreach (var update in updates)
        {
            parameters.Add(update.Key, update.Value);
        }

        var rowsAffected = await _connection.ExecuteAsync(sql, parameters);
        return rowsAffected > 0;
    }
}