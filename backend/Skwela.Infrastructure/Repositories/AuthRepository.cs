using Microsoft.EntityFrameworkCore;
using Skwela.Infrastructure.Data;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Services;
using Skwela.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Skwela.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(AppDbContext context, AuthService authService, ILogger<AuthRepository> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task<User> SignupAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var refreshToken = _authService.GenerateRefreshToken();

        user.refreshToken = refreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return user;    
    }

    public async Task<object> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.refreshToken == refreshToken);

        if (user == null || user.refreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var newAccessToken = _authService.GenerateJwtToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        user.refreshToken = newRefreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new
        {
            accessToken = accessToken,
            refreshToken = refreshToken,
            userId = user.user_id.ToString(),
            username = user.username,
            displayName = user.display_name,
            displayImage = user.display_image,
            role = user.role.ToString()
        };
    }
}