using Microsoft.AspNetCore.Mvc;
using EmployeeService.Application.DTOs;
using EmployeeService.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Управление сотрудниками")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeApplicationService _employeeService;

    public EmployeesController(IEmployeeApplicationService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Создать сотрудника")]
    public async Task<ActionResult<int>> CreateEmployee([FromBody] CreateEmployeeDto employeeDto)
    {
        var employeeId = await _employeeService.CreateAsync(employeeDto);
        return Ok(employeeId);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Удалить сотрудника")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);
        if (!deleted)
            return NotFound($"Сотрудник с Id {id} не найден");

        return NoContent();
    }

    [HttpGet("companies/{companyId}")]
    [SwaggerOperation(Summary = "Получить сотрудников компании")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByCompany(int companyId)
    {
        var employees = await _employeeService.GetByCompanyAsync(companyId);
        return Ok(employees);
    }

    [HttpGet("departments/{departmentId}/employees")]
    [SwaggerOperation(Summary = "Получить всех сотрудников отдела")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployeesByDepartment(int companyId, int departmentId)
    {
        var employees = await _employeeService.GetByDepartmentAsync(companyId, departmentId);
        return Ok(employees);
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Обновить данные сотрудника")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
    {
        var updated = await _employeeService.UpdateAsync(id, updateDto);
        if (!updated)
            return NotFound($"Сотрудник с Id {id} не найден или нет данных для обновления");

        return NoContent();
    }
}