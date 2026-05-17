using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Domain.Entities;
using esecai.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace esecai.Infrastructure.Repositories;

public class ReleaseRepository : IReleaseRepository
{
    private readonly AppDbContext _context;

    public ReleaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Release release, CancellationToken cancellationToken)
    {
        await _context.Releases.AddAsync(release, cancellationToken);
    }

    public Task<Release?> GetByIdAsync(Guid releaseId, CancellationToken cancellationToken)
    {
        return _context.Releases.FirstOrDefaultAsync(release => release.release_id == releaseId, cancellationToken);    
    }

    public async Task<IReadOnlyList<Release>> GetAdminListAsync(CancellationToken cancellationToken)
    {
        return await _context.Releases  
            .AsNoTracking()
            .OrderByDescending(release => release.release_created_at)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReleaseWithReadState>> GetPublishedForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var reads = _context.UserReleaseReads.Where(read => read.user_id == userId);
        var dismissedIds = _context.UserReleaseDismisses
            .Where(d => d.user_id == userId)
            .Select(d => d.release_id);

        return await _context.Releases  
            .AsNoTracking()
            .Where(release => release.release_status == "published" && !dismissedIds.Contains(release.release_id))
            .OrderByDescending(release => release.release_published_at)
            .Select(release => new ReleaseWithReadState(
                release,
                reads.Any(read => read.release_id == release.release_id)))      
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dismissedIds = _context.UserReleaseDismisses
            .Where(d => d.user_id == userId)
            .Select(d => d.release_id);

        return _context.Releases.CountAsync(
            release => release.release_status == "published"
                && !dismissedIds.Contains(release.release_id)
                && !_context.UserReleaseReads.Any(read => read.user_id == userId && read.release_id == release.release_id),
            cancellationToken);
    }

    public Task MarkAsReadAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""UserReleaseReads"" (""user_release_read_id"", ""user_id"", ""release_id"", ""read_at"")
            VALUES ({Guid.NewGuid()}, {userId}, {releaseId}, {now})
            ON CONFLICT (""user_id"", ""release_id"") DO NOTHING;
        ", cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""UserReleaseReads"" (""user_release_read_id"", ""user_id"", ""release_id"", ""read_at"")
            SELECT gen_random_uuid(), {userId}, r.""release_id"", {now}
            FROM ""Releases"" r
            LEFT JOIN ""UserReleaseReads"" urr
                ON urr.""user_id"" = {userId} AND urr.""release_id"" = r.""release_id""
            LEFT JOIN ""UserReleaseDismisses"" urd
                ON urd.""user_id"" = {userId} AND urd.""release_id"" = r.""release_id""
            WHERE r.""release_status"" = 'published'
              AND urr.""user_release_read_id"" IS NULL
              AND urd.""user_release_dismiss_id"" IS NULL
            ON CONFLICT (""user_id"", ""release_id"") DO NOTHING;
        ", cancellationToken);
    }

    public Task DismissAsync(Guid userId, Guid releaseId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""UserReleaseDismisses"" (""user_release_dismiss_id"", ""user_id"", ""release_id"", ""dismissed_at"")
            VALUES ({Guid.NewGuid()}, {userId}, {releaseId}, {now})
            ON CONFLICT (""user_id"", ""release_id"") DO NOTHING;
        ", cancellationToken);
    }

    public async Task DismissAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO ""UserReleaseDismisses"" (""user_release_dismiss_id"", ""user_id"", ""release_id"", ""dismissed_at"")
            SELECT gen_random_uuid(), {userId}, r.""release_id"", {now}
            FROM ""Releases"" r
            LEFT JOIN ""UserReleaseDismisses"" urd
                ON urd.""user_id"" = {userId} AND urd.""release_id"" = r.""release_id""
            WHERE r.""release_status"" = 'published'
              AND urd.""user_release_dismiss_id"" IS NULL
            ON CONFLICT (""user_id"", ""release_id"") DO NOTHING;
        ", cancellationToken);
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
