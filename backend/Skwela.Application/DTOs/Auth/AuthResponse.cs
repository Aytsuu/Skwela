using Skwela.Domain.Enums;

public record AuthResponse(
    string accessToken, 
    string refreshToken,
    Guid userId,
    string username,
    string displayName,
    string displayImage,
    UserRole role
 );