using Skwela.Domain.Enums;

public record UpdateUserRequest(
  Guid userId,
  string? email,
  string? password,
  string? displayName,
  string? displayImage,
  UserRole? role
);