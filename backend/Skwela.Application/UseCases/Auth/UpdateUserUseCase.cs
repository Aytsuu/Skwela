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
    private readonly IRedisCacheService _redisCache;

    /// <summary>
    /// Initializes the UpdateUserUseCase with authentication repository and service
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    /// <param name="authService">Service for generating JWT tokens</param>
    public UpdateUserUseCase(IAuthRepository authRepository, IAuthService authService, IRedisCacheService redisCache)
    {
        _authRepository = authRepository;
        _authService = authService;
        _redisCache = redisCache;
    }

    /// <summary>
    /// Executes the token refresh operation
    /// Validates the refresh token and generates a new JWT token
    /// </summary>
    /// <param name="refreshToken">User refresh token</param>
    /// <returns>RefreshTokenResponse with new JWT token and refresh token</returns>
    public async Task<RefreshTokenResponse> ExecuteRefreshTokenAsync(string refreshToken)
    {
        // Validate refresh token and retrieve user
        var user = await _authRepository.RefreshTokenAsync(refreshToken);
        
        // Generate new JWT token and return response
        return new RefreshTokenResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty
        );
    }

    /// <summary>
    /// Recover account via reset password
    /// </summary>
    /// <param name="request">ResetPasswordRequest contains email and new password</param>
    public async Task ExecuteResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _authRepository.CurrentUserAsync(null, request.email);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
        user.password = hashedPassword;

        // Remove user data from redis cache 
        var cacheKey = $"auth:user:{user.email}";
        await _redisCache.DeleteRedisCacheAsync(cacheKey);
        
        await _authRepository.UpdateUserAsync();
    }

    /// <summary>
    /// Updates user data based on non null values
    /// </summary>
    /// <param name="request">UpdateUserRequest contains nullable user attributes</param>
    public async Task ExecuteUpdateUserAsync(UpdateUserRequest request)
    {
        var user = await _authRepository.CurrentUserAsync(request.userId, null);

        // Update only non null attributes
        if (request.email != null)
        {
            user.email = request.email;
        }
        if (request.password != null)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
            user.password = hashedPassword;
        }
        if (request.displayName != null)
        {
            user.display_name = request.displayName;
        }
        if (request.displayImage != null)
        {
            user.display_image = request.displayImage;
        }
        if (request.role != null)
        {
            user.role = request.role.Value;
        }

        // Remove user data from redis cache 
        var cacheKey = $"auth:user:{user.email}";
        await _redisCache.DeleteRedisCacheAsync(cacheKey);

        // Save changes
        await _authRepository.UpdateUserAsync();
    }
}