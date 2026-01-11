using Microsoft.AspNetCore.Mvc;
using Skwela.Application.Interfaces;
using Skwela.Application.UseCases.Auth;

namespace Skwela.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CreateUserUseCase _createUseCase;
    private readonly GetUserUseCase _getUseCase;
    private readonly UpdateUserUseCase _updateUseCase;

    public AuthController(CreateUserUseCase createUseCase, GetUserUseCase getUseCase, UpdateUserUseCase updateUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _updateUseCase = updateUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await _getUseCase.ExecuteLoginAsync(request);

            return Ok(token);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid credentials.");
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupRequest request)
    {   
        try
        {
            var userId = await _createUseCase.ExecuteAsync(request);
            return Ok(new { userId });
        }
        catch (InvalidDataException)
        {
            return BadRequest("Signup Failed.");
        }

    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var result = await _updateUseCase.ExecuteRefreshTokenAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid token.");
        }
    }
}