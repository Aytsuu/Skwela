using Microsoft.EntityFrameworkCore;
using Skwela.Infrastructure.Data;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Services;
using Skwela.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Skwela.Infrastructure.Repositories;

/// <summary>
/// Authentication Repository
/// Data access layer for user authentication operations
/// Handles user signup, login, token refresh, and Google OAuth signin
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly ILogger<AuthRepository> _logger;

    /// <summary>
    /// Initializes the AuthRepository with database context, auth service, and logger
    /// </summary>
    /// <param name="context">Entity Framework database context</param>
    /// <param name="authService">Service for token generation</param>
    /// <param name="logger">Logger for debugging and error tracking</param>
    public AuthRepository(AppDbContext context, AuthService authService, ILogger<AuthRepository> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account in the system
    /// Generates refresh token and sets expiry time (7 days) if email is provided
    /// </summary>
    /// <param name="user">User entity to be created</param>
    /// <returns>The created user entity with generated IDs and tokens</returns>
    public async Task<User> SignupAsync(User user)
    {
        // Generate refresh token only if email is provided
        if (!string.IsNullOrWhiteSpace(user.email)) {
            user.refreshToken = _authService.GenerateRefreshToken();
            user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        }

        // Add user to database
        _context.Users.Add(user);    
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Authenticates a user by username and password
    /// Generates a new refresh token upon successful login
    /// </summary>
    /// <param name="username">The username to authenticate</param>
    /// <param name="password">The password to verify</param>
    /// <returns>The authenticated User entity with updated tokens</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if user not found or password is invalid</exception>
    public async Task<User> LoginAsync(string username, string password)
    {
        // Find user by username
        var user = _context.Users.FirstOrDefault(u => u.username == username);

        // Validate user exists and password is correct
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // Generate new refresh token and update expiry
        var refreshToken = _authService.GenerateRefreshToken();
        user.refreshToken = refreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return user;    
    }

    /// <summary>
    /// Refreshes an expired JWT token using a valid refresh token
    /// Generates a new refresh token after validation
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate</param>
    /// <returns>The user entity with newly generated tokens</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if refresh token is invalid or expired</exception>
    public async Task<User> RefreshTokenAsync(string refreshToken)
    {
        // Find user with the provided refresh token
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.refreshToken == refreshToken);

        // Validate token exists and hasn't expired
        if (user == null || user.refreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // Generate new refresh token
        var newRefreshToken = _authService.GenerateRefreshToken();
        user.refreshToken = newRefreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return user;
    }
    
    /// <summary>
    /// Authenticates or retrieves a user via Google OAuth
    /// Finds user by email address
    /// </summary>
    /// <param name="email">The email address from Google OAuth</param>
    /// <returns>The User entity if found, otherwise null</returns>
    public async Task<User> GoogleSigninAsync(string email)
    {
        // Find user by email (case-sensitive query)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.email == email);

        return user!;
    }
}