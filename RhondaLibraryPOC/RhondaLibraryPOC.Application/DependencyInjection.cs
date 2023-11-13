using Microsoft.Extensions.DependencyInjection;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

namespace RhondaLibraryPOC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient<Extras>();

        return services;
    }
}