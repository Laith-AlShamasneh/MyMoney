using Dapper;
using Domain.Interfaces.Authentication;
using Domain.Interfaces.Shared;
using Domain.Shared;
using Infrastructure.Helpers;
using Infrastructure.Repositories.Authentication;
using Infrastructure.Repositories.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // 1. Dapper Type Handlers
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHelper());

        // 2. Shared Services (UnitOfWork & UserContext)
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Authentication Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}