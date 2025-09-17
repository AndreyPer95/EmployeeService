using EmployeeService.Domain.Entities;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IDepartmentRepository 
    {
        Task<bool> UpdateAsync(Department department);
        Task<Department> GetByCompanyAndNameAsync(int companyId, string name);
        Task<IEnumerable<Department>> GetAllAsync();
    }
}

