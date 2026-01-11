using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skwela.Infrastructure.Data;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Services;
using Skwela.Infrastructure.Repositories;

namespace Skwela.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            ));

        services.AddScoped<AuthService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IClassroomRepository, ClassroomRepository>();

        return services;
    }
}