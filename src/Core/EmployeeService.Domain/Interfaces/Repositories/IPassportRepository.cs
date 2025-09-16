using EmployeeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IPassportRepository : IRepository<Passport>
    {
        Task<Passport> GetOrCreatePassportAsync(string type, string number);
        Task<Passport> GetByTypeAndNumberAsync(string type, string number);
    }
}
