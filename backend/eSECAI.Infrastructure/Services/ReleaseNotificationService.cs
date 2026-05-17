using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace esecai.Infrastructure.Services;

public class ReleaseNotificationService : IReleaseNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public ReleaseNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyReleasePublishedAsync(ReleaseRealtimePayload payload, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.All.SendAsync("ReleasePublished", payload, cancellationToken);
    }
}
