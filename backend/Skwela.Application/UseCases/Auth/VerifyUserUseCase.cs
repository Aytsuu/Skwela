using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;
using System.Security.Cryptography;

public class VerifyUserUseCase
{
    private readonly IRedisCacheService _redisCache;
    private readonly IEmailService _emailService;
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes the VerifyUserUseCase with auth repo, redis cache and email service
    /// </summary>
    /// <param name="authRepository">Repository for user authentication operations</param>
    /// <param name="authService">Service for auth</param>
    /// <param name="emailService">Service for sending email</param>
    /// <param name="redisCache">Service for redis caching</param>
    public VerifyUserUseCase(IRedisCacheService redisCache, IEmailService emailService, IAuthRepository authRepository, IAuthService authService)
    {
        _redisCache = redisCache;
        _emailService = emailService;
        _authRepository = authRepository;
        _authService = authService;
    }

    /// <summary>
    /// Verifying user email
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="otpCode">Otp code input from user</param>
    public async Task<AuthResponse> ExecuteVerifyEmail(string email, string otpCode)
    {
        // Get the cached otp
        var cachedOtp = await _redisCache.GetOtpAsync(email);
        
        if (cachedOtp == null) // Throw invalid exception if otp is expired
        {
        throw new InvalidDataException("Code has expired");
        }

        if (cachedOtp != otpCode) // Throw invalid exception if otp doesn't match
        {
        throw new InvalidDataException("Invalid OTP");
        }
        
        // Get user data
        var user = await _authRepository.CurrentUserAsync(email);

        // Update user is_email_verified to true
        user.is_email_verified = true;
        await _authRepository.UpdateUserAsync();

        // Remove OTP from redis cache
        await _redisCache.DeleteOtpAsync(email);

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

    /// <summary>
    /// Send otp to user email
    /// </summary>
    /// <param name="email">Email of the user</param>
    public async Task ExecuteSendOtp(string email)
        {
            // Using RandomNuberGenerator to generate six digits OTP code
            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // Store to Redis with a 5-minutes TTL (Time-To-Live)
            await _redisCache.SaveOtpAsync(email, otpCode, TimeSpan.FromMinutes(5));

            // Send OTP to email via SmtpServer
            await _emailService.SendOtpEmailAsync(email, otpCode);
        }
}