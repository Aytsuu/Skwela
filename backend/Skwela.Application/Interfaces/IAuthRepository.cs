using Skwela.Domain.Entities;

namespace Skwela.Application.Interfaces;

public interface IAuthRepository
{
    Task<User> SignupAsync(User user);
    Task<User> LoginAsync(string username, string password);
    Task<User> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<User> GoogleSigninAsync(string email);
}