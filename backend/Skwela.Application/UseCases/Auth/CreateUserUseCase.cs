using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;
using System.Security.Cryptography;

namespace Skwela.Application.UseCases.Auth;

/// <summary>
/// Use case for creating a new user account
/// Handles user signup with username and password validation and storage
/// </summary>
public class CreateUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IRedisCacheService _redisCache;
    private readonly IEmailService _emailService;
    private readonly VerifyUserUseCase _verifyUseCase;

    /// <summary>
    /// Initializes the CreateUserUseCase with the authentication repository
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    public CreateUserUseCase(IAuthRepository authRepository, IRedisCacheService redisCache, IEmailService emailService, VerifyUserUseCase verifyUseCase)
    {
        _authRepository = authRepository;
        _redisCache = redisCache;
        _emailService = emailService;
        _verifyUseCase = verifyUseCase;
    }

    /// <summary>
    /// Executes the user signup process
    /// Hashes the password using BCrypt and creates a new user in the database
    /// </summary>
    /// <param name="request">SignupRequest containing username and password</param>
    /// <returns>The newly created User entity</returns>
    /// <exception cref="InvalidDataException">Thrown if validation fails during signup</exception>
    public async Task<User> ExecuteSignupAsync(SignupRequest request)
    {

        // Hash the password using BCrypt for secure storage
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
        
        // Create a new user with domain validation
        var user = User.Build(request.name, request.email, null, hashedPassword, null);

        // Persist the user to the database
        await _authRepository.SignupAsync(user);

        // Send OTP to user email
        await _verifyUseCase.ExecuteSendOtp(user.email);

        return user;
    }
}