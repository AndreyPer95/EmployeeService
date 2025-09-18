using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Application.DTOs
{
    public class UpdateEmployeeDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Phone { get; set; }
        public int? CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int PassportId { get; set; }
        public PassportDto? Passport { get; set; }
        public DepartmentDto? Department { get; set; }
    }
}
