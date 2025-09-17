using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EmployeeService.Persistence.Factories;

public class PostgreSqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgreSqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}