using Microsoft.Extensions.DependencyInjection;
using Skwela.Application.UseCases.Classrooms;
using Skwela.Application.UseCases.Auth;
using Skwela.Application.UseCases.Enrollments;

namespace Skwela.Application;

/// <summary>
/// Dependency Injection configuration for Application layer
/// Registers all use cases and business logic services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application layer services with the dependency injection container
    /// Called from Program.cs to set up all use cases
    /// </summary>
    /// <param name="services">The IServiceCollection to register services into</param>
    /// <returns>The updated IServiceCollection for chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Classroom Use Cases
        services.AddScoped<CreateClassroomUseCase>();
        services.AddScoped<GetClassroomUseCase>();
        services.AddScoped<DeleteClassroomUseCase>();
        
        // Register Auth Use Cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();

        // Register Enrollment Use Cases
        services.AddScoped<CreateEnrollmentUseCase>();
        services.AddScoped<GetEnrollmentUseCase>();
        services.AddScoped<UpdateEnrollmentUseCase>();
        services.AddScoped<VerifyUserUseCase>();

        return services;
    }
}