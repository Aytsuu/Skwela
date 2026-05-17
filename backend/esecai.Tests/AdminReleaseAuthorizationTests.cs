using System.Security.Claims;
using esecai.API.Controllers;
using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Application.UseCases.Releases;
using esecai.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace esecai.Tests;

public class AdminReleaseAuthorizationTests
{
    [Fact]
    public async Task GetReleases_ReturnsUnauthorized_WhenUserIdClaimIsMissing()
    {
        var controller = BuildController(
            BuildAuthRepository(User.Build("Admin", "admin@example.com", "hash", true, null), isAdmin: true));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var result = await controller.GetReleases(CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetReleases_ReturnsForbid_WhenUserIsNotAdmin()
    {
        var nonAdmin = User.Build("Teacher", "teacher@example.com", "hash", true, null);
        var controller = BuildController(BuildAuthRepository(nonAdmin, isAdmin: false));
        controller.ControllerContext = BuildControllerContext(nonAdmin.user_id);

        var result = await controller.GetReleases(CancellationToken.None);

        Assert.IsType<ForbidResult>(result);
    }

    private static AdminReleaseController BuildController(Mock<IAuthRepository> authRepository)
    {
        var releaseRepository = new Mock<IReleaseRepository>();
        var releaseNotificationService = new Mock<IReleaseNotificationService>();
        var manageReleaseUseCase = new ManageReleaseUseCase(releaseRepository.Object, releaseNotificationService.Object);
        var getReleaseUseCase = new GetReleaseUseCase(releaseRepository.Object);

        return new AdminReleaseController(manageReleaseUseCase, getReleaseUseCase, authRepository.Object);
    }

    private static Mock<IAuthRepository> BuildAuthRepository(User user, bool isAdmin)
    {
        user.is_admin = isAdmin;

        var repository = new Mock<IAuthRepository>();
        repository
            .Setup(auth => auth.CurrentUserAsync(user.user_id, null))
            .ReturnsAsync(user);

        return repository;
    }

    private static ControllerContext BuildControllerContext(Guid userId)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                ], "Test"))
            }
        };
    }
}
