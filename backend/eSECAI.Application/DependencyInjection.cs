using Microsoft.Extensions.DependencyInjection;
using esecai.Application.UseCases.Classrooms;
using esecai.Application.UseCases.Auth;
using esecai.Application.UseCases.Assessments;
using esecai.Application.UseCases.Questions;
using esecai.Application.UseCases.Releases;

namespace esecai.Application;

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
        services.AddScoped<UpdateClassroomUseCase>();
        services.AddScoped<ManageStudentUseCase>();

        // Register Auth Use Cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<VerifyUserUseCase>();

        // Register Assessment Use Cases
        services.AddScoped<CreateAssessmentUseCase>();
        services.AddScoped<GetAssessmentUseCase>();

        // Register Question Use Cases
        services.AddScoped<GetQuestionUseCase>();

        // Register Release Use Cases
        services.AddScoped<GetReleaseUseCase>();
        services.AddScoped<ManageReleaseUseCase>();

        return services;
    }
}
