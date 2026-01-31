using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Auth;

public class CreateUserUseCase
{
    private readonly IAuthRepository _authRepository;

    public CreateUserUseCase(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<User> ExecuteAsync(SignupRequest request)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
        var user = User.Build(null, request.username, hashedPassword);
        return await _authRepository.SignupAsync(user);
    }
}