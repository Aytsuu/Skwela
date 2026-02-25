using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Skwela.Domain.Entities;
using System.Security.Cryptography;
using Skwela.Application.Interfaces;
using System.Text.Json;

namespace Skwela.Infrastructure.Services;

/// <summary>
/// Authentication Service
/// Handles JWT token generation and refresh token creation
/// Provides cryptographic operations for secure token generation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes the AuthService with application configuration
    /// </summary>
    /// <param name="config">Application configuration containing JWT settings</param>
    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token
    /// Uses a 32-byte random number encoded as Base64
    /// </summary>
    /// <returns>A Base64-encoded refresh token string</returns>
    public string GenerateRefreshToken()
    {
        // Create a 32-byte random number
        var randomNumber = new byte[32]; 
        
        // Use cryptographically secure random number generator
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        // Convert to Base64 string for storage and transmission
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Generates a JWT (JSON Web Token) for the authenticated user
    /// Includes user claims and is signed with the configured secret key
    /// Token validity is configured in the appsettings
    /// </summary>
    /// <param name="user">The authenticated User entity</param>
    /// <returns>A signed JWT token string</returns>
    public string GenerateJwtToken(User user)
    {
        // Create claims to include in the token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.username),
            new Claim(JwtRegisteredClaimNames.Email, user.email),
            new Claim(JwtRegisteredClaimNames.Name, user.display_name),
            new Claim("display_image", user.display_image),
            new Claim(ClaimTypes.Role, user.role.ToString())
        };

        // Create signing key from configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // Create signing credentials using HMAC SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token with issuer, audience, and expiration
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                int.Parse(_config["Jwt:ExpiresInMinutes"]!)
            ),
            signingCredentials: creds
        );

        // Convert token to string format
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}