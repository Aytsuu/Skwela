using Skwela.Domain.Entities;

namespace Skwela.Application.Interfaces;

public interface IAuthRepository
{
    Task<User> SignupAsync(User user);
    Task<object> LoginAsync(string username, string password);
    Task<object> RefreshTokenAsync(string accessToken, string refreshToken);
}