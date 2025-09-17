using EmployeeService.Domain.Entities;

namespace EmployeeService.Domain.Interfaces.Repositories
{
    public interface IPassportRepository 
    {
        Task<int> AddAsync(Passport passport);
        Task<bool> UpdateAsync(Passport passport);
        Task<IEnumerable<Passport>> GetAllAsync();
    }
}
