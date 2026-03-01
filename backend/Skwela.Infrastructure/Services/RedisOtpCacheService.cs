using StackExchange.Redis;
using Skwela.Application.Interfaces;

namespace Skwela.Infrastructure.Services;

public class RedisOtpCacheService : IRedisCacheService
{
  private readonly IDatabase _redisDB;

  public RedisOtpCacheService(IDatabase redisDB)
  {
    _redisDB = redisDB;
  }

  public async Task SaveOtpAsync(string email, string otpCode, TimeSpan expiration)
  {
    var cacheKey = $"email_otp:{email.ToLower()}";
    await _redisDB.StringSetAsync(
        cacheKey,
        otpCode,
        expiration
    );
  }

  public async Task<string?> GetOtpAsync(string email)
  {
    var cacheKey = $"email_otp:{email.ToLower()}"; 
    var value = await _redisDB.StringGetAsync(cacheKey);
    return value.HasValue ? value.ToString() : null;
  }

  public async Task DeleteOtpAsync(string email)
  {
    var cacheKey = $"email_otp:{email.ToLower()}"; 
    await _redisDB.KeyDeleteAsync(cacheKey);
  }
}