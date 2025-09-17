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
        var employeeId = await _employeeService.CreateAsync(employeeDto);
        return Ok(employeeId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);
        if (!deleted)
            return NotFound($"Сотрудник с Id {id} не найден");

        return NoContent();
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByCompany(int companyId)
    {
        var employees = await _employeeService.GetByCompanyAsync(companyId);
        return Ok(employees);
    }


    [HttpGet("company/{companyId}/department/{departmentName}")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByDepartment(int companyId, string departmentName)
    {
        var employees = await _employeeService.GetByDepartmentAsync(companyId, departmentName);
        return Ok(employees);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateDto)
    {
        var updated = await _employeeService.UpdateAsync(id, updateDto);
        if (!updated)
            return NotFound($"Сотрудник с Id {id} не найден или нет данных для обновления");

        return NoContent();
    }
}