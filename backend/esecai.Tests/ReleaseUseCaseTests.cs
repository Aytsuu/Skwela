using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Application.UseCases.Releases;
using esecai.Domain.Entities;
using Moq;

namespace esecai.Tests;

public class ReleaseUseCaseTests
{
    private readonly Mock<IReleaseRepository> _releaseRepository = new();
    private readonly Mock<IReleaseNotificationService> _releaseNotificationService = new();
    private readonly GetReleaseUseCase _getReleaseUseCase;
    private readonly ManageReleaseUseCase _manageReleaseUseCase;

    public ReleaseUseCaseTests()
    {
        _getReleaseUseCase = new GetReleaseUseCase(_releaseRepository.Object);
        _manageReleaseUseCase = new ManageReleaseUseCase(_releaseRepository.Object, _releaseNotificationService.Object);
    }

    [Fact]
    public async Task ExecuteCreateDraftAsync_CreatesDraftRelease()
    {
        var adminUserId = Guid.NewGuid();
        var request = new CreateReleaseRequest("2.0.0", "New export flow", "CSV export is live", "Full release note body");
        Release? capturedRelease = null;

        _releaseRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()))
            .Callback<Release, CancellationToken>((release, _) => capturedRelease = release)
            .Returns(Task.CompletedTask);

        var response = await _manageReleaseUseCase.ExecuteCreateDraftAsync(adminUserId, request, CancellationToken.None);

        Assert.NotNull(capturedRelease);
        Assert.Equal("draft", capturedRelease!.release_status);
        Assert.Equal(adminUserId, capturedRelease.created_by_user_id);
        Assert.Equal(request.title, response.title);
        _releaseRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutePublishAsync_PublishesOnce_AndBroadcasts()
    {
        var adminUserId = Guid.NewGuid();
        var release = Release.Build(adminUserId, "2.0.0", "Dashboard filters", "Smarter filters", "Detailed body");

        _releaseRepository
            .Setup(repository => repository.GetByIdAsync(release.release_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var response = await _manageReleaseUseCase.ExecutePublishAsync(adminUserId, release.release_id, CancellationToken.None);

        Assert.Equal("published", response.status);
        Assert.NotNull(release.release_published_at);
        _releaseNotificationService.Verify(
            service => service.NotifyReleasePublishedAsync(It.Is<ReleaseRealtimePayload>(payload => payload.releaseId == release.release_id), It.IsAny<CancellationToken>()),
            Times.Once);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _manageReleaseUseCase.ExecutePublishAsync(adminUserId, release.release_id, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteGetPublishedAsync_ExcludesReadReleases()
    {
        var userId = Guid.NewGuid();
        var publishedRelease = Release.Build(Guid.NewGuid(), "2.0.0", "Feature A", "Summary A", "Body A");
        publishedRelease.Publish(Guid.NewGuid(), DateTime.UtcNow);

        var unreadRelease = Release.Build(Guid.NewGuid(), "2.1.0", "Feature B", "Summary B", "Body B");
        unreadRelease.Publish(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(1));

        _releaseRepository
            .Setup(repository => repository.GetPublishedForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ReleaseWithReadState(publishedRelease, true),
                new ReleaseWithReadState(unreadRelease, false)
            ]);

        var results = await _getReleaseUseCase.ExecuteGetPublishedAsync(userId, CancellationToken.None);

        Assert.Collection(results,
            release => Assert.True(release.isRead),
            release => Assert.False(release.isRead));
    }

    [Fact]
    public async Task ExecuteMarkAsReadAsync_IsIdempotent()
    {
        var userId = Guid.NewGuid();
        var release = Release.Build(Guid.NewGuid(), "2.0.0", "Feature A", "Summary A", "Body A");
        release.Publish(Guid.NewGuid(), DateTime.UtcNow);

        _releaseRepository
            .Setup(repository => repository.GetByIdAsync(release.release_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        _releaseRepository
            .Setup(repository => repository.MarkAsReadAsync(userId, release.release_id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _getReleaseUseCase.ExecuteMarkAsReadAsync(userId, release.release_id, CancellationToken.None);
        await _getReleaseUseCase.ExecuteMarkAsReadAsync(userId, release.release_id, CancellationToken.None);

        _releaseRepository.Verify(repository => repository.MarkAsReadAsync(userId, release.release_id, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteMarkAllAsReadAsync_DelegatesToRepository()
    {
        var userId = Guid.NewGuid();

        _releaseRepository
            .Setup(repository => repository.MarkAllAsReadAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _getReleaseUseCase.ExecuteMarkAllAsReadAsync(userId, CancellationToken.None);

        _releaseRepository.Verify(repository => repository.MarkAllAsReadAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteDismissAsync_DismissesPublishedRelease()
    {
        var userId = Guid.NewGuid();
        var release = Release.Build(Guid.NewGuid(), "2.0.0", "Feature A", "Summary A", "Body A");
        release.Publish(Guid.NewGuid(), DateTime.UtcNow);

        _releaseRepository
            .Setup(repository => repository.GetByIdAsync(release.release_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        _releaseRepository
            .Setup(repository => repository.DismissAsync(userId, release.release_id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _getReleaseUseCase.ExecuteDismissAsync(userId, release.release_id, CancellationToken.None);

        _releaseRepository.Verify(repository => repository.DismissAsync(userId, release.release_id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteClearAllAsync_DelegatesToRepository()
    {
        var userId = Guid.NewGuid();

        _releaseRepository
            .Setup(repository => repository.DismissAllAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _getReleaseUseCase.ExecuteClearAllAsync(userId, CancellationToken.None);

        _releaseRepository.Verify(repository => repository.DismissAllAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
