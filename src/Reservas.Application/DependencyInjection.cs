using Microsoft.Extensions.DependencyInjection;
using Reservas.Application.Auth.Services;
using Reservas.Application.Properties.Services;

namespace Reservas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPropertyService, PropertyService>();

        return services;
    }
}
