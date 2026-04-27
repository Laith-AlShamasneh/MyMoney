using Application.DTOs.Shared;
using Application.Implementations.Shared;
using Application.Interfaces.Authentication;
using Application.Interfaces.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Shared Services
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IMailService, MailService>();

        // Authentication Services
        services.AddScoped<IAuthService, AuthService>();

        // System Services


        return services;
    }
}
