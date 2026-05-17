
namespace esecai.Application.DTOs;

public record AuthResponse(
    string accessToken, 
    string refreshToken,
    Guid userId,
    string email,
    string displayName,
    string displayImage,
    bool isAdmin
);
public record LoginRequest(string email, string password);
public record RefreshTokenRequest(string accessToken, string refreshToken);
public record RefreshTokenResponse(string accessToken, string refreshToken);
public record ResendOtpRequest(string email);
public record ResetPasswordRequest(string email, string password);
public record SignupRequest(string name, string email, string password);
public record UpdateUserRequest(
  Guid userId,
  string? email,
  string? password,
  string? displayName,
  string? displayImage
);
public record ValidateEmailRequest(string email);
public record VerifyEmailRequest(string email, string otpCode, string type);
