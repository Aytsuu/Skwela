public record AuthResponse(
    string accessToken, 
    string refreshToken,
    Guid userId,
    string email,
    string displayName,
    string displayImage
 );