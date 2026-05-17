using System.Security.Claims;
using esecai.Application.UseCases.Releases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace esecai.API.Controllers;

[ApiController]
[Route("api/releases")]
[Authorize]
public class ReleaseController : ControllerBase
{
    private readonly GetReleaseUseCase _getReleaseUseCase;

    public ReleaseController(GetReleaseUseCase getReleaseUseCase)
    {
        _getReleaseUseCase = getReleaseUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetReleases(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        var releases = await _getReleaseUseCase.ExecuteGetPublishedAsync(userId, cancellationToken);
        return Ok(releases);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        var unreadCount = await _getReleaseUseCase.ExecuteGetUnreadCountAsync(userId, cancellationToken);
        return Ok(new { unreadCount });
    }

    [HttpPost("{releaseId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid releaseId, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        try
        {
            await _getReleaseUseCase.ExecuteMarkAsReadAsync(userId, releaseId, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        await _getReleaseUseCase.ExecuteMarkAllAsReadAsync(userId, cancellationToken);
        return Ok();
    }

    [HttpPost("{releaseId}/dismiss")]
    public async Task<IActionResult> DismissRelease(Guid releaseId, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        try
        {
            await _getReleaseUseCase.ExecuteDismissAsync(userId, releaseId, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpPost("clear-all")]
    public async Task<IActionResult> ClearAll(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }

        await _getReleaseUseCase.ExecuteClearAllAsync(userId, cancellationToken);
        return Ok();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out userId);
    }
}
