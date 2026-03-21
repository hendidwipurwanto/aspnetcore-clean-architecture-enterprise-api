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

    [Fact]
    public async Task Should_Throw_When_RefreshToken_Reused()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var reusedToken = new RefreshToken(
            userId,
            "reused-token",
            DateTime.UtcNow.AddDays(7)
        );

        // simulate reused → revoked
        reusedToken.Revoke();

        var tokens = new List<RefreshToken> { reusedToken };

        _contextMock.Setup(x => x.RefreshTokens)
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        var command = new RefreshTokenCommand("reused-token");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Refresh token reuse detected. Please login again.");
    }
}