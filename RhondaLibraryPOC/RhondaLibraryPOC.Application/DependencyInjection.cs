using Microsoft.Extensions.DependencyInjection;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}