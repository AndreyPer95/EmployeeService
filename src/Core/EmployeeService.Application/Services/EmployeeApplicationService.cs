using EmployeeService.Application.DTOs;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Interfaces;

namespace EmployeeService.Application.Services.Implementations;

public class EmployeeApplicationService : IEmployeeApplicationService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeApplicationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        try
        {
            _unitOfWork.BeginTransaction();
            var passport = await _unitOfWork.Passports.GetOrCreatePassportAsync(dto.Passport.Type, dto.Passport.Number);
            var department = await _unitOfWork.Departments.GetOrCreateDepartmentAsync(dto.CompanyId, dto.Department.Name, dto.Department.Phone);

            var employee = new Employee
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Phone = dto.Phone,
                CompanyId = dto.CompanyId,
                DepartmentId = department.Id,
                PassportId = passport.Id
            };

            var employeeId = await _unitOfWork.Employees.AddAsync(employee);

            _unitOfWork.Commit();
            return employeeId;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var deleted = await _unitOfWork.Employees.DeleteAsync(id);

            _unitOfWork.Commit();
            return deleted;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByCompanyAsync(int companyId)
    {
        var employees = await _unitOfWork.Employees.GetEmployeesByCompanyAsync(companyId);
        return employees.Select(MapToDto);
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByDepartmentAsync(int companyId, string departmentName)
    {
        var employees = await _unitOfWork.Employees.GetEmployeesByDepartmentAsync(companyId, departmentName);
        return employees.Select(MapToDto);
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var updates = new Dictionary<string, object>();

            if (dto.Name != null) updates["Name"] = dto.Name;
            if (dto.Surname != null) updates["Surname"] = dto.Surname;
            if (dto.Phone != null) updates["Phone"] = dto.Phone;
            if (dto.CompanyId.HasValue) updates["CompanyId"] = dto.CompanyId.Value;

            if (dto.Passport != null)
            {
                var passport = await _unitOfWork.Passports.GetOrCreatePassportAsync(dto.Passport.Type, dto.Passport.Number);
                updates["PassportId"] = passport.Id;
            }

            if (dto.Department != null)
            {
                var companyId = dto.CompanyId ?? (await _unitOfWork.Employees.GetByIdAsync(id))?.CompanyId ?? 0;
                var department = await _unitOfWork.Departments.GetOrCreateDepartmentAsync(companyId, dto.Department.Name, dto.Department.Phone);
                updates["DepartmentId"] = department.Id;
            }

            var updated = await _unitOfWork.Employees.PartialUpdateEmployeeAsync(id, updates);

            _unitOfWork.Commit();
            return updated;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(id);
        return employee == null ? null : MapToDto(employee);
    }

    private EmployeeResponseDto MapToDto(Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Surname = employee.Surname,
            Phone = employee.Phone,
            CompanyId = employee.CompanyId,
            Passport = new PassportDto
            {
                Type = employee.Passport.Type,
                Number = employee.Passport.Number
            },
            Department = new DepartmentDto
            {
                Name = employee.Department.Name,
                Phone = employee.Department.Phone
            }
        };
    }
}
