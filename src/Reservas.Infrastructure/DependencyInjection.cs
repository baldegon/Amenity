using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reservas.Application.Auth.Repositories;
using Reservas.Application.Auth.Services;
using Reservas.Application.Properties.Repositories;
using Reservas.Infrastructure.Auth;
using Reservas.Infrastructure.Data;
using Reservas.Infrastructure.Properties;

namespace Reservas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.Issuer)
                && !string.IsNullOrWhiteSpace(options.Audience)
                && !string.IsNullOrWhiteSpace(options.Key)
                && options.Key.Length >= 32,
                "Jwt configuration is invalid. Ensure Issuer, Audience and a Key with at least 32 characters are configured.")
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();

        return services;
    }
}
