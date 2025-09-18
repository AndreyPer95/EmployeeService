using EmployeeService.Domain.Entities;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int companyId, int departmentId);
    }
}
