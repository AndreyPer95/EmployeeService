using System.Data;

namespace EmployeeService.Persistence.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
