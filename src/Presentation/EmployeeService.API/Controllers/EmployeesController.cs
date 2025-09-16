using Microsoft.AspNetCore.Mvc;
using EmployeeService.Application.DTOs;
using EmployeeService.Application.Services;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeApplicationService _employeeService;

    public EmployeesController(IEmployeeApplicationService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateEmployee(CreateEmployeeDto employeeDto)
    {
        var employeeId = await _employeeService.CreateEmployeeAsync(employeeDto);
        return Ok(employeeId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (!deleted)
            return NotFound($"Сотрудник с Id {id} не найден");

        return NoContent();
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByCompany(int companyId)
    {
        var employees = await _employeeService.GetEmployeesByCompanyAsync(companyId);
        return Ok(employees);
    }


    [HttpGet("company/{companyId}/department/{departmentName}")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByDepartment(int companyId, string departmentName)
    {
        var employees = await _employeeService.GetEmployeesByDepartmentAsync(companyId, departmentName);
        return Ok(employees);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateDto)
    {
        var updated = await _employeeService.UpdateEmployeeAsync(id, updateDto);
        if (!updated)
            return NotFound($"Сотрудник с Id {id} не найден или нет данных для обновления");

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound($"Сотрудник с Id {id} не найден");

        return Ok(employee);
    }
}