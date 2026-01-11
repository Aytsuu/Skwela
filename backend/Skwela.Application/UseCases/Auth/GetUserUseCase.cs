using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Auth;

public class GetUserUseCase
{
    private readonly IAuthRepository _authRepository;

    public GetUserUseCase(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<object> ExecuteLoginAsync(LoginRequest request)
    {
        return await _authRepository.LoginAsync(request.username, request.password);
    }
}