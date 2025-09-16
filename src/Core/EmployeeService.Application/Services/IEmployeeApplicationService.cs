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
        Task<int> CreateEmployeeAsync(CreateEmployeeDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByCompanyAsync(int companyId);
        Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByDepartmentAsync(int companyId, string departmentName);
        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto);
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
    }
}
