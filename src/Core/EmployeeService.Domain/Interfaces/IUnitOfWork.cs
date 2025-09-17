using EmployeeService.Domain.Interfaces.Repositories;
using System.Data;

namespace EmployeeService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        IPassportRepository Passports { get; }
        IDbTransaction Transaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
