using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Auth;

public class UpdateUserUseCase
{
    private readonly IAuthRepository _authRepository;

    public UpdateUserUseCase(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<object> ExecuteRefreshTokenAsync(RefreshTokenRequest request)
    {
        return await _authRepository.RefreshTokenAsync(request.accessToken, request.refreshToken);
    }
}