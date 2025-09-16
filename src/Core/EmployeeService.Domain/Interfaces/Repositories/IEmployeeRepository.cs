using EmployeeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee> GetEmployeeWithDetailsAsync(int id);
        Task<bool> PartialUpdateEmployeeAsync(int id, Dictionary<string, object> updates);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName);
    }
}
