using EmployeeService.Application.Services;
using EmployeeService.Application.Services.Implementations;
using EmployeeService.Domain.Interfaces;
using EmployeeService.Domain.Interfaces.Repositories;
using EmployeeService.Persistence.Factories;
using EmployeeService.Persistence.Repositories;
using EmployeeService.Persistence.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, PostgreSqlConnectionFactory>();
        services.AddScoped<IUnitOfWork, DataUnitOfWork>();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeApplicationService, EmployeeApplicationService>();
        return services;
    }
}
