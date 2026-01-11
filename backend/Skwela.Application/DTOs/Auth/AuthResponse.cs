public record AuthResponse(
    string accessToken, 
    string refreshToken,
    string userId,
    string username,
    string displayName,
    string displayImage,
    string role
 );