using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;              
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using esecai.Infrastructure.Data;
using esecai.Application.Interfaces;
using esecai.Application.DTOs;
using esecai.Infrastructure.Services;
using esecai.Infrastructure.Services.Prompts;
using esecai.Infrastructure.Repositories;
using NRedisStack;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;
using Minio;
using Microsoft.AspNetCore.Authentication;

namespace esecai.Infrastructure;

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

        // UnitOfWork and Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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
                // Default API Behavior: Read JWTs, and return 401s if missing
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                // Google OAuth Exception: When Google succeeds, temporarily save the identity to a Cookie
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Try to get token from Cookie (Standard for all API requests)
                        var accessToken = context.Request.Cookies["accessToken"];

                        // ONLY if Cookie is missing, check the Query String (Specific for SignalR)
                        if (string.IsNullOrEmpty(accessToken))
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/hubs/notifications"))
                            {
                                accessToken = context.Request.Query["access_token"];
                            }
                        }

                        // If we found a token in either place, assign it
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
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
                options.ClaimActions.MapJsonKey("picture", "picture");
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Events.OnRedirectToAuthorizationEndpoint = context =>
                {
                    // This intercepts the redirect to Google and forces the account chooser screen
                    context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
                    return Task.CompletedTask;
                };
            });
        
        services.AddMinio(configureSource => configureSource
            .WithEndpoint(config["Minio:Endpoint"])
            .WithCredentials(config["Minio:AccessKey"], config["Minio:SecretKey"])
            .WithSSL(false)
            .Build());
        
        services.AddSignalR();
        services.AddHttpClient();
        services.Configure<AIOptions>(config.GetSection(AIOptions.SectionName));
        services.PostConfigure<AIOptions>(options =>
        {
            // Allow non-standard env var naming if provided in .env files.
            if (string.IsNullOrWhiteSpace(options.OllamaBaseUrl))
            {
                options.OllamaBaseUrl = config["AI_OllamaBaseUrl"] ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(options.OllamaApiEndpoint))
            {
                options.OllamaApiEndpoint = config["AI_OllamaApiEndpoint"] ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(options.OllamaApiKey))
            {
                options.OllamaApiKey = config["AI_OllamaApiKey"] ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(options.GeminiApiKey))
            {
                options.GeminiApiKey = config["AI_GeminiApiKey"]
                    ?? config["Gemini__ApiKey"]
                    ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(options.GeminiModel))
            {
                options.GeminiModel = config["AI_GeminiModel"]
                    ?? config["Gemini__Model"]
                    ?? string.Empty;
            }
        });      

        // Register Repository and External Services
        services.AddScoped<AuthService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IMinioFileService, MinioFileService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IPdfService, PdfService>();  
        services.AddScoped<IAIClientService, AIClientService>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();

        return services;
    }
}
