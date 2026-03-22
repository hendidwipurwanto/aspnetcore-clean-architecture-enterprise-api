using EnterpriseOrderSystem.Application.Common.Exceptions;
using EnterpriseOrderSystem.Application.Features.Auth.Commands;
using EnterpriseOrderSystem.Application.Features.Auth.Handlers;
using EnterpriseOrderSystem.Application.Interfaces;
using EnterpriseOrderSystem.Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _jwtServiceMock = new Mock<IJwtService>();

        _handler = new RefreshTokenHandler(_contextMock.Object, _jwtServiceMock.Object);
    }

    // 1. REUSE DETECTION (SUDAH ADA)
    [Fact]
    public async Task Should_Throw_When_RefreshToken_Reused()
    {
        var userId = Guid.NewGuid();

        var reusedToken = new RefreshToken(userId, "reused-token", DateTime.UtcNow.AddDays(7));
        reusedToken.Revoke();

        var tokens = new List<RefreshToken> { reusedToken };

        _contextMock.Setup(x => x.RefreshTokens)
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        var command = new RefreshTokenCommand("reused-token");

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Refresh token reuse detected. Please login again.");
    }

    // 2. SUCCESS CASE
    public async Task Should_Return_New_Tokens_When_RefreshToken_Valid()
    {
        var userId = Guid.NewGuid();

        var validToken = new RefreshToken(userId, "valid-token", DateTime.UtcNow.AddDays(7));

        var roleId = Guid.NewGuid();

        var user = new User("test@mail.com", "hashed", roleId);

        typeof(User).GetProperty("Id").SetValue(user, userId);

        
        var role = new Role("User");
        typeof(User).GetProperty("Role").SetValue(user, role);

        var tokens = new List<RefreshToken> { validToken };
        var users = new List<User> { user };

        _contextMock.Setup(x => x.RefreshTokens)
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.Users)
            .Returns(users.AsQueryable().BuildMockDbSet().Object);

        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("new-jwt");

        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh");

        var command = new RefreshTokenCommand("valid-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Token.Should().Be("new-jwt");
        result.RefreshToken.Should().Be("new-refresh");
    }

    // 3. INVALID TOKEN
    [Fact]
    public async Task Should_Throw_When_Token_Not_Found()
    {
        var tokens = new List<RefreshToken>();

        _contextMock.Setup(x => x.RefreshTokens)
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        var command = new RefreshTokenCommand("not-exist");

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invalid refresh token");
    }

    // 4. EXPIRED TOKEN
    [Fact]
    public async Task Should_Throw_When_Token_Expired()
    {
        var userId = Guid.NewGuid();

        var expiredToken = new RefreshToken(userId, "expired-token", DateTime.UtcNow.AddDays(-1));

        var tokens = new List<RefreshToken> { expiredToken };

        _contextMock.Setup(x => x.RefreshTokens)
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        var command = new RefreshTokenCommand("expired-token");

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Refresh token expired");
    }
}