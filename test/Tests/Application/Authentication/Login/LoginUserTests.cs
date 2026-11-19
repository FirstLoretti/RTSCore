using FluentAssertions;

using NSubstitute;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Authentication.Login;
using RTSCore.Application.Repositories;

namespace Tests.Application.Authentication.Login;

public sealed class LoginUserTests
{
    private readonly IJwtTokenProvider _jwtTokenGenerator = Substitute.For<IJwtTokenProvider>();
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = Substitute.For<IRefreshTokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private LoginUserCommandHandler CreateHandler() => new(
        _unitOfWork,
        _jwtTokenGenerator,
        _refreshTokenGenerator
    );

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveChanges_WhenUserNotExist()
    {
        var command = new LoginUserCommand("testName", "testPass");

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.InvalidCredentials);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveChanges_WhenWrongPass()
    {
        var user = new User("testName", "testPass");
        var command = new LoginUserCommand(user.Name, "wrongPass");
        _unitOfWork.UserRepository.GetAsync(user.Name, default).ReturnsForAnyArgs(user);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.InvalidCredentials);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}