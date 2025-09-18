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
            if (dto.Passport == null) 
                throw new InvalidOperationException($"Необходимо добавить паспортные данные");

            var passport = new Passport
            {
                Type = dto.Passport.Type,
                Number = dto.Passport.Number
            };           

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department == null)
                throw new InvalidOperationException($"Отдел в компании {dto.CompanyId} не найден");

            var employee = new Employee
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Phone = dto.Phone,
                CompanyId = dto.CompanyId,
                DepartmentId = department.Id,
                PassportId = passport.Id
            };
            
            _unitOfWork.BeginTransaction();
            
            passport.Id = await _unitOfWork.Passports.AddAsync(passport);
            employee.PassportId = passport.Id;
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
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            var passportId = employee.PassportId;
            var deletedEmployee = await _unitOfWork.Employees.DeleteAsync(id);
            var deletedPassport = await _unitOfWork.Passports.DeleteAsync(passportId);

            _unitOfWork.Commit();
            return deletedEmployee;
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

    public async Task<IEnumerable<EmployeeResponseDto>> GetByDepartmentAsync(int companyId, int departmentId)
    {
        var employees = await _unitOfWork.Employees.GetByDepartmentAsync(companyId, departmentId);
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
                var passport = await _unitOfWork.Passports.GetByIdAsync(dto.PassportId);
                if (passport == null)
                    throw new InvalidOperationException($"Паспорт не найден");

                employee.PassportId = passport.Id;
            }

            if (dto.Department != null)
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
                if (department == null)
                    throw new InvalidOperationException($"Отдел {dto.Department.Name} в компании {employee.CompanyId} не найден");

                employee.DepartmentId = department.Id;
            }

            var updated = await _unitOfWork.Employees.UpdateAsync(employee);

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
