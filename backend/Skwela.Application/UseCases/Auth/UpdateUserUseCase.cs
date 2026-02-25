using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Auth;

/// <summary>
/// Use case for updating user information
/// Currently handles JWT token refresh operations
/// </summary>
public class UpdateUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes the UpdateUserUseCase with authentication repository and service
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    /// <param name="authService">Service for generating JWT tokens</param>
    public UpdateUserUseCase(IAuthRepository authRepository, IAuthService authService)
    {
        _authRepository = authRepository;
        _authService = authService;
    }

    /// <summary>
    /// Executes the token refresh operation
    /// Validates the refresh token and generates a new JWT token
    /// </summary>
    /// <param name="request">RefreshTokenRequest containing current access token and refresh token</param>
    /// <returns>RefreshTokenResponse with new JWT token and refresh token</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if refresh token is invalid or expired</exception>
    public async Task<RefreshTokenResponse> ExecuteRefreshTokenAsync(RefreshTokenRequest request)
    {
        // Validate refresh token and retrieve user
        var user = await _authRepository.RefreshTokenAsync(request.accessToken, request.refreshToken);
        
        // Generate new JWT token and return response
        return new RefreshTokenResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty
        );
    }
}