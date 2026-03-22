using EnterpriseOrderSystem.Domain.Entities;
using Xunit;
using FluentAssertions;
public class RefreshTokenTests
{
    //1. TEST DEFAULT VALUES
    [Fact]
    public void Should_Initialize_With_Default_Values()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var token = new RefreshToken(userId, "token", DateTime.UtcNow.AddDays(7));

        // Assert
        token.UserId.Should().Be(userId);
        token.Token.Should().Be("token");
        token.IsUsed.Should().BeFalse();
        token.IsRevoked.Should().BeFalse();
    }

    // 2. TEST REVOKE
    [Fact]
    public void Should_Set_IsRevoked_When_Revoke_Called()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(7));

        // Act
        token.Revoke();

        // Assert
        token.IsRevoked.Should().BeTrue();
    }

    // 3. TEST MARK AS USED
    [Fact]
    public void Should_Set_IsUsed_When_MarkAsUsed_Called()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(7));

        // Act
        token.MarkAsUsed();

        // Assert
        token.IsUsed.Should().BeTrue();
    }

    // 4. TEST IDEMPOTENCY
    [Fact]
    public void Revoke_Should_Be_Idempotent()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(7));

        // Act
        token.Revoke();
        token.Revoke(); // call twice

        // Assert
        token.IsRevoked.Should().BeTrue();
    }

    // 5. TEST IDEMPOTENCY
    [Fact]
    public void MarkAsUsed_Should_Be_Idempotent()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(7));

        // Act
        token.MarkAsUsed();
        token.MarkAsUsed(); // call twice

        // Assert
        token.IsUsed.Should().BeTrue();
    }
}
