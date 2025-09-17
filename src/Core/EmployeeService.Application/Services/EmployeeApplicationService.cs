using EmployeeService.Domain.Interfaces;
using EmployeeService.Domain.Entities;
using EmployeeService.Application.DTOs;

namespace EmployeeService.Application.Services.Implementations;

public class EmployeeApplicationService : IEmployeeApplicationService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeApplicationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateAsync(CreateEmployeeDto dto)
    {
        try
        {
            _unitOfWork.BeginTransaction();
            if (dto.Passport == null) 
                throw new InvalidOperationException($"Необходимо добавить паспортные данные");

            var passport = new Passport
            {
                Type = dto.Passport.Type,
                Number = dto.Passport.Number
            };
            passport.Id = await _unitOfWork.Passports.AddAsync(passport);

            var departments = await _unitOfWork.Departments.GetAllAsync();
            var department = departments.FirstOrDefault(d => d.CompanyId == dto.CompanyId && d.Name == dto.Department.Name);
            if (department == null)
                throw new InvalidOperationException($"Отдел {dto.Department.Name} в компании {dto.CompanyId} не найден");

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

    public async Task<bool> DeleteAsync(int id)
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

    public async Task<IEnumerable<EmployeeResponseDto>> GetByCompanyAsync(int companyId)
    {
        var employees = await _unitOfWork.Employees.GetByCompanyAsync(companyId);
        return employees.Select(MapToDto);
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetByDepartmentAsync(int companyId, string departmentName)
    {
        var employees = await _unitOfWork.Employees.GetByDepartmentAsync(companyId, departmentName);
        return employees.Select(MapToDto);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return false;

            if (dto.Name != null)
                employee.Name = dto.Name;

            if (dto.Surname != null)
                employee.Surname = dto.Surname;

            if (dto.Phone != null)
                employee.Phone = dto.Phone;

            if (dto.CompanyId.HasValue)
                employee.CompanyId = dto.CompanyId.Value;

            if (dto.Passport != null)
            {
                var passports = await _unitOfWork.Passports.GetAllAsync();
                var passport = passports.FirstOrDefault(p => p.Type == dto.Passport.Type && p.Number == dto.Passport.Number);
                if (passport == null)
                    throw new InvalidOperationException($"Паспорт {dto.Passport.Type} {dto.Passport.Number} не найден");

                employee.PassportId = passport.Id;
            }

            if (dto.Department != null)
            {
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var department = departments.FirstOrDefault(d => d.CompanyId == employee.CompanyId && d.Name == dto.Department.Name);
                if (department == null)
                    throw new InvalidOperationException($"Отдел {dto.Department.Name} в компании {employee.CompanyId} не найден");

                employee.DepartmentId = department.Id;
            }

            var updated = await _unitOfWork.Employees.PartialUpdateAsync(employee);

            _unitOfWork.Commit();
            return updated;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
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
