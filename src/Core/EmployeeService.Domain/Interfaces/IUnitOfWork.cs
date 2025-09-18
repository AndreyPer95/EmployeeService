using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces.Repositories;
using System.Data;

namespace EmployeeService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IRepository<Department> Departments { get; }
        IRepository<Passport> Passports { get; }
        IDbTransaction Transaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
