using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Auth;

public class UpdateUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;

    public UpdateUserUseCase(IAuthRepository authRepository, IAuthService authService)
    {
        _authRepository = authRepository;
        _authService = authService;
    }

    public async Task<RefreshTokenResponse> ExecuteRefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _authRepository.RefreshTokenAsync(request.accessToken, request.refreshToken);
        
        return new RefreshTokenResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty
        );
    }
}