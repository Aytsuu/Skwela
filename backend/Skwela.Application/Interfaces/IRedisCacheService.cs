
namespace Skwela.Application.Interfaces;

public interface IRedisCacheService
{
  Task SaveOtpAsync(string email, string otpCode, TimeSpan expiration);
  Task<string?> GetOtpAsync(string email);
  Task DeleteOtpAsync(string email);
}