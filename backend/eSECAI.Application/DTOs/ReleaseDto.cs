namespace esecai.Application.DTOs;

public record CreateReleaseRequest(
    string? version,
    string title,
    string summary,
    string body
);

public record UpdateReleaseRequest(
    Guid releaseId,
    string? version,
    string title,
    string summary,
    string body
);

public record ReleaseResponse(
    Guid releaseId,
    string? version,
    string title,
    string summary,
    string body,
    string status,
    DateTime? publishedAt,
    DateTime createdAt,
    bool isRead
);

public record ReleaseRealtimePayload(
    Guid releaseId,
    string? version,
    string title,
    string summary,
    string body,
    DateTime publishedAt
);

public record ReleaseWithReadState(
    esecai.Domain.Entities.Release release,
    bool isRead
);
