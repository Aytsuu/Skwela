using Microsoft.AspNetCore.Mvc;
using Skwela.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using System.Security.Claims; 
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

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
    private readonly VerifyUserUseCase _verifyUseCase;

    /// <summary>
    /// Initializes the AuthController with required use cases
    /// </summary>
    /// <param name="createUseCase">Use case for user creation/signup</param>
    /// <param name="getUseCase">Use case for user login and retrieval</param>
    /// <param name="updateUseCase">Use case for user updates and token refresh</param>
    public AuthController(CreateUserUseCase createUseCase, GetUserUseCase getUseCase, UpdateUserUseCase updateUseCase, VerifyUserUseCase verifyUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _updateUseCase = updateUseCase;
        _verifyUseCase = verifyUseCase;
    }

    /// <summary>
    /// /me endpoint for user state management
    /// </summary>
    /// <returns>AuthResponse with JWT token, refresh token, and user details if successful</returns>
    /// <response code="200">Fresh user data</response>
    /// <response code="401">Acceess token expired/invalid</response>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> CurrentUser() 
    {
        try
        {
            //Extract the claims
            // We check the .NET mapped type first, then fallback to the raw JWT name
            var email = User.FindFirstValue(ClaimTypes.Email) 
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

            var freshData = await _getUseCase.ExecuteGetCurrentUser(email!);

            return Ok (new {
                userId = freshData.user_id,
                email = freshData.email,
                displayName = freshData.display_name,
                role = freshData.role
            });
        }
        catch (EmailNotVerifiedException)
        {
            return Forbid();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid Token.");
        }
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

            // Set cookies securely from the backend
            SetTokenCookies(user.accessToken, user.refreshToken);

            return Ok(new {
                userId = user.userId,
                email = user.email,
                name = user.displayName,
                role = user.role
            });
        }
        catch (EmailNotVerifiedException)
        {

            return Forbid();
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
            var userId = await _createUseCase.ExecuteSignupAsync(request);
            return Ok();
        }
        catch (EmailAlreadyExistException eaEx) 
        {
            return BadRequest(new {
                field = "email",
                message = eaEx.Message
            });
        }
        catch (InvalidDataException)
        {
            return BadRequest("Signup Failed.");
        }

    }

    /// <summary>
    /// Generate new 6-digit otp
    /// Store to redis cache with 5-minutes TTL (Time-To-Live) and send to user email
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <response code="200">OTP code resend successfully</response>
    /// <response code="400">Failed to resend OTP</response>
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpRequest request) 
    {
        try 
        {
            await _verifyUseCase.ExecuteSendOtp(request.email);
            return Ok();
        }
        catch (InvalidDataException)
        {
            return BadRequest("Failed to resend otp");
        }
    }

    /// <summary>
    /// Generate new 6-digit otp
    /// Store to redis cache with 5-minutes TTL (Time-To-Live) and send to user email
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="otpCode">OTP code input from user</param>
    /// <response code="200">Email has been verified</response>
    /// <response code="400">Invalid OTP</response>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request) 
    {
        try 
        {
            var user = await _verifyUseCase.ExecuteVerifyEmail(request.email, request.otpCode);

            // Set cookies securely from the backend
            SetTokenCookies(user.accessToken, user.refreshToken);

            return Ok(new {
                userId = user.userId,
                email = user.email,
                name = user.displayName,
                role = user.role
            });
        }
        catch (InvalidDataException idEx)
        {
            return BadRequest(new {
                message = idEx.Message
            });
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
    public async Task<IActionResult> RefreshTokenAsync()
    {
        try
        {
            var currentRefreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(currentRefreshToken))
            {
                return Unauthorized("Refresh token cookie is missing.");
            }

            try {
                var tokens = await _updateUseCase.ExecuteRefreshTokenAsync(currentRefreshToken);

                // Set cookies securely from the backend
                SetTokenCookies(tokens.accessToken, tokens.refreshToken);
                return Ok(new {message = "Token refreshed succesfully"});
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
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
        var displayName = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(email)) {
            return BadRequest("Failed to extract email.");
        }

        if (string.IsNullOrWhiteSpace(displayName)) {
            return BadRequest("Failed to extract name in email.");
        }

        // Get or create user and generate tokens
        var user = await _getUseCase.ExecuteGoogleSigninAsync(email, displayName);

        // Delete the temporary Google cookie so it doesn't clutter the browser
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Set cookies securely from the backend
        SetTokenCookies(user.accessToken, user.refreshToken);
        
        // Redirect to frontend with tokens in query string
        return Redirect($"http://localhost:3000/authentication/callback?userId={user.userId}&email={email}&displayName={displayName}&role={user.role}");
    }

    /// <summary>
    /// Logout http endpoint
    /// Clear cookies to remove current user's access token and refresh token
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult logout() 
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax
        };

        // This tells the browser to instantly destroy these cookies
        Response.Cookies.Delete("accessToken", cookieOptions);
        Response.Cookies.Delete("refreshToken", cookieOptions);

        return Ok(new { Message = "Logged out successfully" });
    }

    /// <summary>
    /// Takes access token and refresh token as params
    /// Set the cookies from the backend securely
    /// </summary>
    /// <param name="accessToken">The access token of the logged user</param>
    /// <param name="refreshToken">The refresh token of the logged user</param>
    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        // Access Token Cookie (short-lived)
        var accessOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(60)
        };
        Response.Cookies.Append("accessToken", accessToken, accessOptions);

        var refreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, refreshOptions);
    }

}