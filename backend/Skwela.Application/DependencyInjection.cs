using Microsoft.Extensions.DependencyInjection;
using Skwela.Application.UseCases.Classrooms;
using Skwela.Application.UseCases.Auth;

namespace Skwela.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateClassroomUseCase>();
        services.AddScoped<GetClassroomUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();

        return services;
    }
}