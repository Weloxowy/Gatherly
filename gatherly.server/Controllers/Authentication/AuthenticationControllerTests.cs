using gatherly.server.Controllers.Authentication;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AuthenticationControllerTests
{
    private readonly Mock<IBlacklistTokenService> _blacklistTokenServiceMock;
    private readonly AuthenticationController _controller;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<ISsoSessionService> _ssoSessionServiceMock;
    private readonly Mock<ITokenEntityService> _tokenEntityServiceMock;
    private readonly Mock<IMailEntityService> _mailEntityServiceMock;
    private readonly Mock<IRecoverySessionService> _recoverySessionServiceMock;
    private readonly Mock<IUserEntityService> _userServiceMock;

    public AuthenticationControllerTests()
    {
        _userServiceMock = new Mock<IUserEntityService>();
        _ssoSessionServiceMock = new Mock<ISsoSessionService>();
        _tokenEntityServiceMock = new Mock<ITokenEntityService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _blacklistTokenServiceMock = new Mock<IBlacklistTokenService>();
        _mailEntityServiceMock = new Mock<IMailEntityService>();
        _recoverySessionServiceMock = new Mock<IRecoverySessionService>();

        _controller = new AuthenticationController(
            _userServiceMock.Object,
            _ssoSessionServiceMock.Object,
            _tokenEntityServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _blacklistTokenServiceMock.Object,
            _mailEntityServiceMock.Object,
            _recoverySessionServiceMock.Object
        );
    }

    [Fact]
    public void Test_AddEntities()
    {
        // Act
        AddTestEntities();

        // Assert
        _userServiceMock.Verify(service => service.CreateNewUser(It.IsAny<UserEntityDTOCreate>()), Times.Exactly(2));
        _refreshTokenServiceMock.Verify(service => service.GenerateRefreshToken(It.IsAny<Guid>()), Times.Exactly(2));
    }

    [Fact]
    public void SendSsoCode_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _userServiceMock.Setup(service => service.GetUserInfo(email)).Returns((UserEntity)null);

        // Act
        var result = _controller.SendSsoCode(email);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void SendSsoCode_Success_ReturnsOk()
    {
        // Arrange
        var email = "user1@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email };
        var ssoCode = "SSO123";

        _userServiceMock.Setup(service => service.GetUserInfo(email)).Returns(user);
        //_ssoSessionServiceMock.Setup(service => service.CreateSso(user.Id, email)).Returns(ssoCode);

        // Act
        var result = _controller.SendSsoCode(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Email was send", okResult.Value);
    }

    [Fact]
    public void VerifySsoCode_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var data = new UserEntityDTOLoginCode { Email = "user1@example.com", Code = "invalid_code" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };

        _userServiceMock.Setup(service => service.GetUserInfo(data.Email)).Returns(user);
        _ssoSessionServiceMock.Setup(service => service.ValidSso(user.Id, data.Code)).Returns(false);

        // Act
        var result = _controller.VerifySsoCode(data);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Wrong SSO code", badRequestResult.Value);
    }

    [Fact]
    public void LoginUserByPassword_InvalidUser_ReturnsNotFound()
    {
        // Arrange
        var data = new UserEntityDTOLoginPassword { Email = "nonexistent@example.com", Password = "password" };
        _userServiceMock.Setup(service => service.VerifyUser(data)).Returns((UserEntity)null);

        // Act
        var result = _controller.LoginUserByPassword(data);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void CreateNewUser_EmailAlreadyUsed_ReturnsBadRequest()
    {
        // Arrange
        var data = new UserEntityDTOCreate { Email = "user1@example.com", Password = "password" };
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };

        _userServiceMock.Setup(service => service.GetUserInfo(data.Email)).Returns(existingUser);

        // Act
        var result = _controller.CreateNewUser(data);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email address is already used", badRequestResult.Value);
    }

    private void AddTestEntities()
    {
        var user1 = new UserEntity { Id = Guid.NewGuid(), Email = "user1@example.com" };
        var user2 = new UserEntity { Id = Guid.NewGuid(), Email = "user2@example.com" };
        _userServiceMock.Setup(service => service.CreateNewUser(It.IsAny<UserEntityDTOCreate>())).Returns(user1);
        _userServiceMock.Setup(service => service.CreateNewUser(It.IsAny<UserEntityDTOCreate>())).Returns(user2);

        var refreshToken1 = new RefreshToken { Id = Guid.NewGuid(), Token = "token1" };
        var refreshToken2 = new RefreshToken { Id = Guid.NewGuid(), Token = "token2" };
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(It.IsAny<Guid>())).Returns(refreshToken1);
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(It.IsAny<Guid>())).Returns(refreshToken2);

        _userServiceMock.Object.CreateNewUser(new UserEntityDTOCreate { Email = user1.Email });
        _userServiceMock.Object.CreateNewUser(new UserEntityDTOCreate { Email = user2.Email });
        _refreshTokenServiceMock.Object.GenerateRefreshToken(Guid.NewGuid());
        _refreshTokenServiceMock.Object.GenerateRefreshToken(Guid.NewGuid());
    }
}
