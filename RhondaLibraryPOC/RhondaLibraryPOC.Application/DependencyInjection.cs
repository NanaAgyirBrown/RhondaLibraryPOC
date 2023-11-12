using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhondaLibraryPOC.Application.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
    {
        //services.Configure<CheckoutSettings>(configuration.GetSection("CheckoutSettings"));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient<Extras>();

        return services;
    }
}