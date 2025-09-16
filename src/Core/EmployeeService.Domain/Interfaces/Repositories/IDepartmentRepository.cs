using EmployeeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department> GetOrCreateDepartmentAsync(int companyId, string name, string phone);
        Task<Department> GetByCompanyAndNameAsync(int companyId, string name);
    }
}
