using esecai.Application.DTOs;
using esecai.Domain.Entities;

namespace esecai.Application.Interfaces;

public interface IReleaseRepository
{
    Task AddAsync(Release release, CancellationToken cancellationToken);
    Task<Release?> GetByIdAsync(Guid releaseId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Release>> GetAdminListAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ReleaseWithReadState>> GetPublishedForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken);
    Task MarkAsReadAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken);
    Task DismissAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken);
    Task DismissAllAsync(Guid userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
