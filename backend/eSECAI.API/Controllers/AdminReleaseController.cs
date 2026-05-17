using System.Security.Claims;
using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Application.UseCases.Releases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace esecai.API.Controllers;

[ApiController]
[Route("api/admin/releases")]
[Authorize]
public class AdminReleaseController : ControllerBase
{
    private readonly ManageReleaseUseCase _manageReleaseUseCase;
    private readonly GetReleaseUseCase _getReleaseUseCase;
    private readonly IAuthRepository _authRepository;

    public AdminReleaseController(
        ManageReleaseUseCase manageReleaseUseCase,
        GetReleaseUseCase getReleaseUseCase,
        IAuthRepository authRepository)
    {
        _manageReleaseUseCase = manageReleaseUseCase;
        _getReleaseUseCase = getReleaseUseCase;
        _authRepository = authRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetReleases(CancellationToken cancellationToken)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck != null)
        {
            return adminCheck;
        }

        var releases = await _getReleaseUseCase.ExecuteGetAdminListAsync(cancellationToken);
        return Ok(releases);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRelease([FromBody] CreateReleaseRequest request, CancellationToken cancellationToken)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck != null)
        {
            return adminCheck;
        }

        try
        {
            var release = await _manageReleaseUseCase.ExecuteCreateDraftAsync(GetRequiredUserId(), request, cancellationToken);
            return Ok(release);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{releaseId}")]
    public async Task<IActionResult> UpdateRelease(Guid releaseId, [FromBody] CreateReleaseRequest request, CancellationToken cancellationToken)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck != null)
        {
            return adminCheck;
        }

        try
        {
            var release = await _manageReleaseUseCase.ExecuteUpdateDraftAsync(
                GetRequiredUserId(),
                new UpdateReleaseRequest(releaseId, request.version, request.title, request.summary, request.body),
                cancellationToken);

            return Ok(release);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{releaseId}/publish")]
    public async Task<IActionResult> PublishRelease(Guid releaseId, CancellationToken cancellationToken)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck != null)
        {
            return adminCheck;
        }

        try
        {
            var release = await _manageReleaseUseCase.ExecutePublishAsync(GetRequiredUserId(), releaseId, cancellationToken);
            return Ok(release);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private async Task<IActionResult?> EnsureAdminAsync()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        var user = await _authRepository.CurrentUserAsync(userId, null);
        if (!user.is_admin)
        {
            return Forbid();
        }

        return null;
    }

    private Guid GetRequiredUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out userId);
    }
}
