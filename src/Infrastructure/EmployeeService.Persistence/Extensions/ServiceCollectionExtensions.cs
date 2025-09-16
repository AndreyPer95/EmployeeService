using EmployeeService.Application.Services;
using EmployeeService.Application.Services.Implementations;
using EmployeeService.Domain.Interfaces;
using EmployeeService.Domain.Interfaces.Repositories;
using EmployeeService.Persistence.Factories;
using EmployeeService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>(provider =>
        {
            var factory = provider.GetRequiredService<IDbConnectionFactory>();
            var connection = factory.CreateConnection();
            return new EmployeeRepository(connection);
        });

        services.AddScoped<IDepartmentRepository, DepartmentRepository>(provider =>
        {
            var factory = provider.GetRequiredService<IDbConnectionFactory>();
            var connection = factory.CreateConnection();
            return new DepartmentRepository(connection);
        });

        services.AddScoped<IPassportRepository, PassportRepository>(provider =>
        {
            var factory = provider.GetRequiredService<IDbConnectionFactory>();
            var connection = factory.CreateConnection();
            return new PassportRepository(connection);
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeApplicationService, EmployeeApplicationService>();
        return services;
    }
}
