using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Authentication;
using RTSCore.Application.Authentication.Register;
using RTSCore.Application.Repositories;
using RTSCore.Infrastructure.Authentication;

namespace Tests.Application.Authentication.Register;

public sealed class RegisterUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IJwtTokenProvider _jwtTokenProvider = Substitute.For<IJwtTokenProvider>();
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = Substitute.For<IRefreshTokenGenerator>();

    private RegisterUserCommandHandler CreateHandler() => new(_unitOfWork, _jwtTokenProvider, _refreshTokenGenerator);

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExist()
    {
        _unitOfWork.UserRepository.ExistAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateHandler().Handle(new RegisterUserCommand("name", "pass"), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthErrors.UserAlreadyExist("name"));
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}