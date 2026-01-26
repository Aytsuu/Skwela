using Skwela.Domain.Entities;

namespace Skwela.Application.Interfaces;

public interface IAuthService
{
  string GenerateJwtToken(User user);
  string GenerateRefreshToken();
}