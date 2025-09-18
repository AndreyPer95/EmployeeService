using System.Data;
using Dapper;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;

namespace EmployeeService.Persistence.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    private readonly IDbConnection _connection;

    static EmployeeRepository()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public EmployeeRepository(IDbConnection connection) : base(connection, "employees")
    {
        _connection = connection;
    }

    public override async Task<int> AddAsync(Employee employee)
    {
        var sql = @"INSERT INTO employees (name, surname, phone, company_id, department_id, passport_id) 
                    VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportId) 
                    RETURNING id";
        return await _connection.QuerySingleAsync<int>(sql, employee);
    }

    public override async Task<bool> UpdateAsync(Employee updateEmployee)
    {
        var sql = @"UPDATE employees 
                    SET name = @Name, surname = @Surname, phone = @Phone, 
                        company_id = @CompanyId, department_id = @DepartmentId, passport_id = @PassportId 
                    WHERE id = @Id";

        var rowsAffected = await _connection.ExecuteAsync(sql, updateEmployee);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId)
    {
        var sql = @"SELECT e.id, e.name, e.surname, e.phone, e.company_id, e.department_id, e.passport_id,
                       d.id, d.company_id, d.name, d.phone,
                       p.id, p.type, p.number
                FROM employees e
                LEFT JOIN departments d ON e.department_id = d.id
                LEFT JOIN passports p ON e.passport_id = p.id
                WHERE e.company_id = @CompanyId";

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
            splitOn: "id,id");

        return employeeDict.Values;
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int companyId, int departmentId)
    {
        var sql = @"SELECT e.id, e.name, e.surname, e.phone, e.company_id, e.department_id, e.passport_id,
                       d.id, d.company_id, d.name, d.phone,
                       p.id, p.type, p.number
                    FROM employees e
                    LEFT JOIN departments d ON e.department_id = d.id
                    LEFT JOIN passports p ON e.passport_id = p.id
                    WHERE e.company_id = @CompanyId AND d.id = @DepartmentId";

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
            new { CompanyId = companyId, DepartmentId = departmentId },
            splitOn: "id,id");

        return employeeDict.Values;
    }
}