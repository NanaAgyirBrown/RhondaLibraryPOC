using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Interfaces.Persistence;
using RhondaLibraryPOC.Infrastructure.ConnectionFactory;
using RhondaLibraryPOC.Persistence.Repositories;

namespace RhondaLibraryPOC.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
    {
        services.AddScoped<IBookRepository, LibraryRepository>();
        services.AddScoped<IUserRepository, LibraryRepository>();
        services.AddScoped<ICheckoutRepository, LibraryRepository>();
        services.AddScoped<IExtrasRepository, LibraryRepository>();

        services.AddTransient<IDataSource>(_ => new DbDataSource(configuration.GetValue<string>("ConnectionStrings:PostgresConnection")));

        return services;
    }
}
