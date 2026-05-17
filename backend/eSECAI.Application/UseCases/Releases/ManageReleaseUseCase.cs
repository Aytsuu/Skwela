using esecai.Application.DTOs;
using esecai.Application.Interfaces;

namespace esecai.Application.UseCases.Releases;

public class ManageReleaseUseCase
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IReleaseNotificationService _releaseNotificationService;

    public ManageReleaseUseCase(IReleaseRepository releaseRepository, IReleaseNotificationService releaseNotificationService)
    {
        _releaseRepository = releaseRepository;
        _releaseNotificationService = releaseNotificationService;
    }

    public async Task<ReleaseResponse> ExecuteCreateDraftAsync(Guid adminUserId, CreateReleaseRequest request, CancellationToken cancellationToken)
    {
        var release = Domain.Entities.Release.Build(adminUserId, request.version, request.title, request.summary, request.body);
        await _releaseRepository.AddAsync(release, cancellationToken);
        await _releaseRepository.SaveChangesAsync(cancellationToken);

        return Map(release);
    }

    public async Task<ReleaseResponse> ExecuteUpdateDraftAsync(Guid adminUserId, UpdateReleaseRequest request, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetByIdAsync(request.releaseId, cancellationToken)
            ?? throw new KeyNotFoundException("Release not found.");

        release.Update(request.version, request.title, request.summary, request.body);
        await _releaseRepository.SaveChangesAsync(cancellationToken);

        return Map(release);
    }

    public async Task<ReleaseResponse> ExecutePublishAsync(Guid adminUserId, Guid releaseId, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetByIdAsync(releaseId, cancellationToken)
            ?? throw new KeyNotFoundException("Release not found.");

        release.Publish(adminUserId, DateTime.UtcNow);
        await _releaseRepository.SaveChangesAsync(cancellationToken);

        await _releaseNotificationService.NotifyReleasePublishedAsync(
            new ReleaseRealtimePayload(
                release.release_id,
                release.release_version,
                release.release_title,
                release.release_summary,
                release.release_body,
                release.release_published_at ?? DateTime.UtcNow),
            cancellationToken);

        return Map(release);
    }

    private static ReleaseResponse Map(Domain.Entities.Release release)
    {
        return new ReleaseResponse(
            release.release_id,
            release.release_version,
            release.release_title,
            release.release_summary,
            release.release_body,
            release.release_status,
            release.release_published_at,
            release.release_created_at,
            false);
    }
}
