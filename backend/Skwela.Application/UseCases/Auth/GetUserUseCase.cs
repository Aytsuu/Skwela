using Skwela.Application.Interfaces;
using Skwela.Domain.Enums;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Auth;

/// <summary>
/// Use case for user authentication and login operations
/// Handles traditional login, Google OAuth signin, and token generation
/// </summary>
public class GetUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    /// <summary>
    /// Initializes the GetUserUseCase with required repositories and services
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    /// <param name="authService">Service for generating JWT and refresh tokens</param>
    /// <param name="classroomRepository">Repository for classroom operations</param>
    /// <param name="enrollmentRepository">Repository for enrollment operations</param>
    public GetUserUseCase(
        IAuthRepository authRepository, 
        IAuthService authService,
        IClassroomRepository classroomRepository,
        IEnrollmentRepository enrollmentRepository
    )
    {
        _authRepository = authRepository;
        _authService = authService;
        _classroomRepository = classroomRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    /// <summary>
    /// Executes the login process with username and password
    /// Validates credentials and generates JWT and refresh tokens upon success
    /// </summary>
    /// <param name="request">LoginRequest containing username and password</param>
    /// <returns>AuthResponse with JWT token, refresh token, and user details</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if credentials are invalid</exception>
    public async Task<AuthResponse> ExecuteLoginAsync(LoginRequest request)
    {
        // Validate credentials and retrieve user from database
        var user = await _authRepository.LoginAsync(request.username, request.password);

        // Generate authentication response with tokens
        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty,
            user.user_id,
            user.username ?? "",
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.role
        );
    }

    /// <summary>
    /// Executes the Google OAuth signin process
    /// Creates a new user account if they don't exist, otherwise retrieves existing user
    /// </summary>
    /// <param name="name">User's email address from Google OAuth</param>
    /// <param name="email">User's email address from Google OAuth</param>
    /// <returns>AuthResponse with JWT token, refresh token, and user details</returns>
    public async Task<AuthResponse> ExecuteGoogleSigninAsync(string email, string name)
    {
        // Attempt to find existing user by email
        var user = await _authRepository.GoogleSigninAsync(email);

        // If user doesn't exist, create a new account
        if (user == null)
        {
            user = await _authRepository.SignupAsync(User.Build(name, email, null, null));
        }
        
        // Generate authentication response with tokens
        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? "",
            user.user_id,
            user.username ?? "",
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.role
        );
    }
}