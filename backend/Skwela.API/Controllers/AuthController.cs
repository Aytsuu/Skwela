using Microsoft.AspNetCore.Mvc;
using Skwela.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace Skwela.API.Controllers;

/// <summary>
/// Authentication Controller
/// Handles user login, signup, token refresh, and Google OAuth authentication
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CreateUserUseCase _createUseCase;
    private readonly GetUserUseCase _getUseCase;
    private readonly UpdateUserUseCase _updateUseCase;

    /// <summary>
    /// Initializes the AuthController with required use cases
    /// </summary>
    /// <param name="createUseCase">Use case for user creation/signup</param>
    /// <param name="getUseCase">Use case for user login and retrieval</param>
    /// <param name="updateUseCase">Use case for user updates and token refresh</param>
    public AuthController(CreateUserUseCase createUseCase, GetUserUseCase getUseCase, UpdateUserUseCase updateUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _updateUseCase = updateUseCase;
    }

    /// <summary>
    /// Authenticates a user with username and password
    /// </summary>
    /// <param name="request">Login request containing username and password</param>
    /// <returns>AuthResponse with JWT token, refresh token, and user details if successful</returns>
    /// <response code="200">User successfully authenticated</response>
    /// <response code="401">Invalid credentials provided</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var user = await _getUseCase.ExecuteLoginAsync(request);

            return Ok(user);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid credentials.");
        }
    }

    /// <summary>
    /// Creates a new user account with username and password
    /// </summary>
    /// <param name="request">Signup request containing username and password</param>
    /// <returns>UserId of the newly created user</returns>
    /// <response code="200">User account successfully created</response>
    /// <response code="400">Invalid signup data provided</response>
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

    /// <summary>
    /// Refreshes an expired JWT token using a valid refresh token
    /// </summary>
    /// <param name="request">RefreshTokenRequest containing the access token and refresh token</param>
    /// <returns>RefreshTokenResponse with new JWT token and refresh token</returns>
    /// <response code="200">Token successfully refreshed</response>
    /// <response code="401">Invalid or expired refresh token</response>
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

    /// <summary>
    /// Initiates Google OAuth 2.0 authentication flow
    /// Redirects user to Google's login page
    /// </summary>
    /// <returns>Challenge result that redirects to Google authentication</returns>
    [HttpGet("login-google")]
    public IActionResult SigninWithGoogle()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse")},
            "Google"
        );
    }

    /// <summary>
    /// Google OAuth 2.0 callback handler
    /// Processes the response from Google after user authentication
    /// Creates a new user account if they don't exist, then redirects to frontend with tokens
    /// </summary>
    /// <returns>Redirect to frontend with access token and refresh token in query parameters</returns>
    /// <response code="302">Successful authentication and redirect</response>
    /// <response code="400">Google authentication failed or email extraction failed</response>
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        // Authenticate the user based on the cookie set by Google auth
        var result = await HttpContext.AuthenticateAsync("Cookies");

        if (!result.Succeeded) return BadRequest("Google Auth Failed.");

        // Extract claims from the authenticated principal
        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        
        // Get the email claim from Google's response
        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email)) {
            return BadRequest("Failed to extract email.");
        }

        // Get or create user and generate tokens
        var tokens = await _getUseCase.ExecuteGoogleSigninAsync(email);
        
        // Redirect to frontend with tokens in query string
        return Redirect($"http://skwela.local:3000/authentication/callback?token={tokens.accessToken}&refresh={tokens.refreshToken}");
    }
}