using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Domain.Entities;

namespace esecai.Application.UseCases.Releases;

public class GetReleaseUseCase
{
    private readonly IReleaseRepository _releaseRepository;

    public GetReleaseUseCase(IReleaseRepository releaseRepository)
    {
        _releaseRepository = releaseRepository;
    }

    public async Task<IReadOnlyList<ReleaseResponse>> ExecuteGetPublishedAsync(Guid userId, CancellationToken cancellationToken)
    {
        var releases = await _releaseRepository.GetPublishedForUserAsync(userId, cancellationToken);
        return releases.Select(item => Map(item.release, item.isRead)).ToList();
    }

    public async Task<IReadOnlyList<ReleaseResponse>> ExecuteGetAdminListAsync(CancellationToken cancellationToken)
    {
        var releases = await _releaseRepository.GetAdminListAsync(cancellationToken);
        return releases.Select(release => Map(release, isRead: false)).ToList();
    }

    public Task<int> ExecuteGetUnreadCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _releaseRepository.GetUnreadCountAsync(userId, cancellationToken);
    }

    public async Task ExecuteMarkAsReadAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetByIdAsync(releaseId, cancellationToken);
        if (release == null || release.release_status != "published")
        {
            throw new KeyNotFoundException("Release not found.");
        }

        await _releaseRepository.MarkAsReadAsync(userId, releaseId, cancellationToken);
    }

    public async Task ExecuteMarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _releaseRepository.MarkAllAsReadAsync(userId, cancellationToken);
    }

    public async Task ExecuteDismissAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetByIdAsync(releaseId, cancellationToken);
        if (release == null || release.release_status != "published")
        {
            throw new KeyNotFoundException("Release not found.");
        }

        await _releaseRepository.DismissAsync(userId, releaseId, cancellationToken);
    }

    public async Task ExecuteClearAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _releaseRepository.DismissAllAsync(userId, cancellationToken);
    }
    private static ReleaseResponse Map(Release release, bool isRead)
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
            isRead);
    }
}
