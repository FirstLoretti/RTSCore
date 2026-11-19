using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;
using NSubstitute.ReceivedExtensions;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Authentication.Refresh;
using RTSCore.Application.Repositories;

namespace Tests.Application.Authentication.Refresh;

public sealed class RefreshTokensHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IJwtTokenProvider _jwtTokenProvider = Substitute.For<IJwtTokenProvider>();
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = Substitute.For<IRefreshTokenGenerator>();

    private RefreshTokensCommandHandler CreateHandler() => new(_unitOfWork, _refreshTokenGenerator, _jwtTokenProvider);

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccessTokenValidationFailure()
    {
        _jwtTokenProvider.ValidateExpiredToken(Arg.Any<string>()).Returns(AuthErrors.Unauthorized("Invalid token."));

        var result = await CreateHandler().Handle(new RefreshTokensCommand("invalidToken", "refreshToken"), default);

        result.IsFailure.Should().BeTrue();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        _jwtTokenProvider.ValidateExpiredToken(Arg.Any<string>()).Returns(UserId.New());

        var action = () => CreateHandler().Handle(new RefreshTokensCommand("acessToken", "refreshToken"), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRefreshTokenNotFound()
    {
        _jwtTokenProvider.ValidateExpiredToken(Arg.Any<string>()).Returns(UserId.New());
        _unitOfWork.UserRepository.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(new User("name", "pass"));

        var result = await CreateHandler().Handle(new RefreshTokensCommand("acessToken", "refreshToken"), default);

        result.IsFailure.Should().BeTrue();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndDeleteRefreshTokens_WhenRefreshTokenCompromized()
    {
        var refreshToken = TestData.CreateRefreshToken();
        _jwtTokenProvider.ValidateExpiredToken(Arg.Any<string>()).Returns(UserId.New());
        _unitOfWork.UserRepository.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(new User("name", "pass"));
        _unitOfWork.RefreshTokenRepository.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(refreshToken);

        var result = await CreateHandler().Handle(new RefreshTokensCommand("acessToken", "refreshToken"), default);

        result.IsFailure.Should().BeTrue();
        await _unitOfWork.RefreshTokenRepository.Received(1).RemoveTokensAsync(refreshToken.UserId, default);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}