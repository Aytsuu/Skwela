using Microsoft.Extensions.DependencyInjection;
using Skwela.Application.UseCases.Classrooms;
using Skwela.Application.UseCases.Auth;
using Skwela.Application.UseCases.Enrollments;

namespace Skwela.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        // Classroom Use Cases
        services.AddScoped<CreateClassroomUseCase>();
        services.AddScoped<GetClassroomUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();

        // Enrollment Use Cases
        services.AddScoped<CreateEnrollmentUseCase>();
        services.AddScoped<GetEnrollmentUseCase>();
        services.AddScoped<UpdateEnrollmentUseCase>();
        services.AddScoped<DeleteClassroomUseCase>();

        return services;
    }
}