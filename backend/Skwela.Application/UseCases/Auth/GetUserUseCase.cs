using Skwela.Application.Interfaces;
using Skwela.Domain.Enums;
using Skwela.Domain.Entities;
using System;

namespace Skwela.Application.UseCases.Auth;

public class GetUserUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthService _authService;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetUserUseCase(
        IAuthRepository authRepository, 
        IAuthService authService,
        IClassroomRepository classroomRepository,
        IEnrollmentRepository enrollmentRepository
    )
    {
        _authRepository = authRepository;
        _authService = authService;
        _classroomRepository = classroomRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<AuthResponse> ExecuteLoginAsync(LoginRequest request)
    {
        var user = await _authRepository.LoginAsync(request.username, request.password);

        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? string.Empty,
            user.user_id,
            user.username ?? "",
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.role
        );
    }

    public async Task<AuthResponse> ExecuteGoogleSigninAsync(string email)
    {
        var user = await _authRepository.GoogleSigninAsync(email);

        if (user == null)
        {
            user = await _authRepository.SignupAsync(User.Build(email, null, null));
        }

        Console.WriteLine(user.refreshToken);
        return new AuthResponse(
            _authService.GenerateJwtToken(user),
            user.refreshToken ?? "",
            user.user_id,
            user.username ?? "",
            user.email ?? "",
            user.display_name,
            user.display_image,
            user.role
        );
    }
}