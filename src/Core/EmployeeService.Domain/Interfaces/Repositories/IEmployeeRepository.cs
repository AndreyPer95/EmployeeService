using EmployeeService.Domain.Entities;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository 
    {
        Task<int> AddAsync(Employee employee);
        Task<bool> PartialUpdateAsync(Employee updateEmployee);
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int companyId, string departmentName);
        Task<bool>  DeleteWithPassportAsync(int id);
    }
}
