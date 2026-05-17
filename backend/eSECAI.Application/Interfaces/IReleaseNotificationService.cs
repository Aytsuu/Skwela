using esecai.Application.DTOs;

namespace esecai.Application.Interfaces;

public interface IReleaseNotificationService
{
    Task NotifyReleasePublishedAsync(ReleaseRealtimePayload payload, CancellationToken cancellationToken);
}
