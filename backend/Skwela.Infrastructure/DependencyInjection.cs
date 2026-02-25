using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;              
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skwela.Infrastructure.Data;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Services;
using Skwela.Infrastructure.Repositories;
using NRedisStack;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;

namespace Skwela.Infrastructure;

/// <summary>
/// Dependency Injection configuration for Infrastructure layer
/// Registers database context, authentication, repositories, and external services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure layer services with the dependency injection container
    /// Sets up database, authentication, Redis cache, repositories, and services
    /// </summary>
    /// <param name="services">The IServiceCollection to register services into</param>
    /// <param name="config">Application configuration for database and authentication settings</param>
    /// <returns>The updated IServiceCollection for chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Configure PostgreSQL Database Context
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )); 

        // Configure Redis Cache Connection
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(
                config.GetConnectionString("Redis")!
            )
        );

        // Register Redis database interface
        services.AddScoped<IDatabase>(cfg => {
            var muxer = cfg.GetRequiredService<IConnectionMultiplexer>();
            return muxer.GetDatabase();
        });

        // Configure Authentication Schemes (Cookie + JWT + Google OAuth)
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            // Configure JWT Bearer Token validation
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
                    )
                };
            })
            // Configure Cookie Authentication (for temporary auth state)
            .AddCookie()
            // Configure Google OAuth 2.0 Authentication
            .AddGoogle(options => 
            {
                options.ClientId = config["Authentication:Google:ClientId"]!;
                options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
                options.CallbackPath = "/api/auth/signin-google";
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

        // Register Repository and External Services
        services.AddScoped<AuthService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}