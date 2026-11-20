using FluentAssertions;

using RTSCore.Application;
using RTSCore.Application.Authentication;

using static Tests.TestData;

namespace Tests.Application.Authentication;

public sealed class RefreshTokenTests
{
    [Fact]
    public void Use_ShouldReturnFailure_WhenUserIdDoesNotMatch()
    {
        var id = UserId.New();
        var token = CreateRefreshToken();

        var result = token.Use(id);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.RefreshTokenCompromized);
    }

    [Fact]
    public void Use_ShouldReturnFailure_WhenAlreadyUsed()
    {
        var id = UserId.New();
        var token = CreateRefreshToken(userId: id);
        token.Use(id);

        var result = token.Use(id);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.RefreshTokenCompromized);
    }

    [Fact]
    public void Use_ShouldReturnFailure_WhenRevoked()
    {
        var id = UserId.New();
        var token = CreateRefreshToken(userId: id);
        token.Revoke(id);

        var result = token.Use(id);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.RefreshTokenCompromized);
    }

    [Fact]
    public void Use_ShouldReturnFailure_WhenExpired()
    {
        var id = UserId.New();
        var token = CreateRefreshToken(userId: id, expiryTime: DateTime.MinValue);

        var result = token.Use(id);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(AuthErrors.Unauthorized(string.Empty).Code);
    }

    [Fact]
    public void Use_ShouldReturnSuccess()
    {
        var id = UserId.New();
        var token = CreateRefreshToken(userId: id);

        var result = token.Use(id);

        result.IsSuccess.Should().BeTrue();
        token.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ShouldReturnFailure_WhenUserIdDoesNotMatch()
    {
        var token = CreateRefreshToken();

        var result = token.Revoke(UserId.New());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.RefreshTokenCompromized);
    }

    [Fact]
    public void Revoke_ShouldReturnSuccess()
    {
        var token = CreateRefreshToken();

        var result = token.Revoke(token.UserId);

        result.IsSuccess.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();
    }
}