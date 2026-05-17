using esecai.Application.Interfaces;
using esecai.Domain.Entities;
using esecai.Domain.Exceptions;
using System.Text.Json;
using esecai.Application.DTOs;

namespace esecai.Application.UseCases.Auth;

/// <summary>
/// Use case for user authentication and login operations
/// Handles traditional login, Google OAuth signin, and token generation
/// </summary>
public class GetUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;
    private readonly IClassroomRepository _classroomRepository;
    private readonly VerifyUserUseCase _verifyUseCase;
    private readonly IRedisCacheService _redisCache;

    /// <summary>
    /// Initializes the GetUserUseCase with required repositories and services
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    /// <param name="authService">Service for generating JWT and refresh tokens</param>
    /// <param name="classroomRepository">Repository for classroom operations</param>
    public GetUserUseCase(
        IAuthRepository authRepository, 
        IAuthService authService,
        IClassroomRepository classroomRepository,
        VerifyUserUseCase verifyUseCase,
        IRedisCacheService redisCache
    )
    {
        _authRepository = authRepository;
        _authService = authService;
        _classroomRepository = classroomRepository;
        _verifyUseCase = verifyUseCase;
        _redisCache = redisCache;
    }

    /// <summary>
    /// Executes the login process with username and password
    /// Validates credentials and generates JWT and refresh tokens upon success
    /// </summary>
    /// <param name="dto">LoginRequest containing username and password</param>
    /// <returns>AuthResponse with JWT token, refresh token, and user details</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if credentials are invalid</exception>
    public async Task<AuthResponse> ExecuteLoginAsync(LoginRequest dto)
    {
        var user = await _authRepository.LoginAsync(dto.email, dto.password);
        
        // Store to redis cache for faster retrieval
        var cacheKey = $"auth:user:{dto}";
        await _redisCache.SaveRedisCacheAsync(cacheKey, user, TimeSpan.FromHours(1));

        // Check if email is verified
        if (!user.is_email_verified)
        {
            await _verifyUseCase.ExecuteSendOtp(dto.email);
            throw new EmailNotVerifiedException("Email verification is required");
        }

        // Generate authentication response with tokens
        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty,
            user.user_id,
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.is_admin
        );
    }

    /// <summary>
    /// Executes the Google OAuth signin process
    /// Creates a new user account if they don't exist, otherwise retrieves existing user
    /// </summary>
    /// <param name="name">User's email address from Google OAuth</param>
    /// <param name="email">User's email address from Google OAuth</param>
    /// <returns>AuthResponse with JWT token, refresh token, and user details</returns>
    public async Task<AuthResponse> ExecuteGoogleSigninAsync(string email, string name, string image)
    {
        // Attempt to find existing user by email
        var user = await _authRepository.GoogleSigninAsync(email);

        // If user doesn't exist, create a new account
        if (user == null)
        {
            user = await _authRepository.SignupAsync(User.Build(name, email, null, true, image));
        }

        // Store to redis cache for faster retrieval
        var cacheKey = $"auth:user:{user.email}";
        await _redisCache.SaveRedisCacheAsync(cacheKey, user, TimeSpan.FromHours(1));
        
        // Generate authentication response with tokens
        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? "",
            user.user_id,
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.is_admin
        );
    }

    /// <summary>
    /// Executes the fetch process
    /// </summary>
    /// <param name="email">Email of current user</param>
    /// <returns>The complete data of current user</returns>
    public async Task<User> ExecuteGetCurrentUser(string email)
    {
        var cacheKey = $"auth:user:{email}";
        var userCache = await _redisCache.GetRedisCacheAsync(cacheKey); 
        
        if (userCache != null)
        {
            var cachedUser = JsonSerializer.Deserialize<User>(userCache);
            
            // JsonSerializer can technically return null, so we satisfy the compiler here
            if (cachedUser != null) 
            {
                if (!cachedUser.is_email_verified) 
                    throw new EmailNotVerifiedException("Email verification is required");
                    
                return cachedUser; 
            }
        }

        // Cache Miss! Fetch from PostgreSQL.
        var user = await _authRepository.CurrentUserAsync(null, email);

        // 3. Apply Business Rules
        if (!user.is_email_verified)
        {
            throw new EmailNotVerifiedException("Email verification is required");
        }

        // Save to redis for faster response
        await _redisCache.SaveRedisCacheAsync(cacheKey, JsonSerializer.Serialize(user), TimeSpan.FromHours(1));

        return user;
    }
}
