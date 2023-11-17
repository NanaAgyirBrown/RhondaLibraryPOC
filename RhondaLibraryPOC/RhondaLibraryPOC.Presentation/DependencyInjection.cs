using RhondaLibraryPOC.Application.Interfaces.AuthService;
using RhondaLibraryPOC.Presentation.Common.Service;

namespace RhondaLibraryPOC.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddControllers();

        return services;
    }
}
