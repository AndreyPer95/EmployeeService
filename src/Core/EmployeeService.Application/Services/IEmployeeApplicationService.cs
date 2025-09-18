using EmployeeService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Application.Services
{
    public interface IEmployeeApplicationService
    {
        Task<int> CreateAsync(CreateEmployeeDto employeeDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<EmployeeResponseDto>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<EmployeeResponseDto>> GetByDepartmentAsync(int companyId, int departmentId);
        Task<bool> UpdateAsync(int id, UpdateEmployeeDto updateDto);
    }
}
