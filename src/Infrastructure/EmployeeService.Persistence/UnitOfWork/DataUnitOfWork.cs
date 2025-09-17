using System.Data;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces;
using EmployeeService.Domain.Interfaces.Repositories;
using EmployeeService.Persistence.Factories;
using EmployeeService.Persistence.Repositories;

namespace EmployeeService.Persistence.UnitOfWork;

public class DataUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private IEmployeeRepository _employeeRepository;
    private IDepartmentRepository _departmentRepository;
    private IPassportRepository _passportRepository;

    public DataUnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _connection.Open();
    }

    public IEmployeeRepository Employees => _employeeRepository ??= new EmployeeRepository(_connection);
    public IDepartmentRepository Departments => _departmentRepository ??= new DepartmentRepository(_connection);
    public IPassportRepository Passports => _passportRepository ??= new PassportRepository(_connection);

    public IDbTransaction Transaction => _transaction;
    public void BeginTransaction() => _transaction = _connection.BeginTransaction();
    public void Commit() => _transaction?.Commit();
    public void Rollback() => _transaction?.Rollback();
    public void Dispose() => _connection?.Dispose();
}