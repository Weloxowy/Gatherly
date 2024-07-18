using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace gatherly.server.Controllers.Authentication;

public class AuthenticationControllerTests
{
    private readonly Mock<IBlacklistTokenService> _blacklistTokenServiceMock;
    private readonly AuthenticationController _controller;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<ISsoSessionService> _ssoSessionServiceMock;
    private readonly Mock<ITokenEntityService> _tokenEntityServiceMock;

    private readonly Mock<IUserEntityService> _userServiceMock;

    // Definicje user1 i user2 na poziomie klasy
    private UserEntity user1;
    private UserEntity user2;


    public AuthenticationControllerTests()
    {
        _userServiceMock = new Mock<IUserEntityService>();
        _ssoSessionServiceMock = new Mock<ISsoSessionService>();
        _tokenEntityServiceMock = new Mock<ITokenEntityService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _blacklistTokenServiceMock = new Mock<IBlacklistTokenService>();

        _controller = new AuthenticationController(
            _userServiceMock.Object,
            _ssoSessionServiceMock.Object,
            _tokenEntityServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _blacklistTokenServiceMock.Object
        );
    }

    private void AddTestEntities()
    {
        // Dodawanie dwóch encji UserEntity
        var user1 = new UserEntity { Id = Guid.NewGuid(), Email = "user1@example.com" };
        var user2 = new UserEntity { Id = Guid.NewGuid(), Email = "user2@example.com" };
        _userServiceMock.Setup(service => service.CreateNewUser(It.IsAny<UserEntityDTOCreate>())).Returns(user1);
        _userServiceMock.Setup(service => service.CreateNewUser(It.IsAny<UserEntityDTOCreate>())).Returns(user2);

        // Dodawanie dwóch encji RefreshToken
        var refreshToken1 = new RefreshToken { Id = Guid.NewGuid(), Token = "token1" };
        var refreshToken2 = new RefreshToken { Id = Guid.NewGuid(), Token = "token2" };
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(refreshToken1);
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns(refreshToken2);


        // Act
        _userServiceMock.Object.CreateNewUser(new UserEntityDTOCreate { Email = user1.Email });
        _userServiceMock.Object.CreateNewUser(new UserEntityDTOCreate { Email = user2.Email });
        _refreshTokenServiceMock.Object.GenerateRefreshToken(Guid.NewGuid());
        _refreshTokenServiceMock.Object.GenerateRefreshToken(Guid.NewGuid());
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
        var email = "test@example.com";
        _userServiceMock.Setup(service => service.GetUserInfo(email)).Returns((UserEntity)null);

        // Act
        var result = _controller.SendSsoCode(email);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("User profile not found", notFoundResult.Value);
    }

    [Fact]
    public void SendSsoCode_InternalServerError_ReturnsStatusCode500()
    {
        // Arrange
        var email = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email };
        _userServiceMock.Setup(service => service.GetUserInfo(email)).Returns(user);
        _ssoSessionServiceMock.Setup(service => service.CreateSso(user.Id, email)).Throws(new Exception());

        // Act
        var result = _controller.SendSsoCode(email);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var statusCodeResult = result as ObjectResult;
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("There was a problem while creating a SSO token. Please try again later", statusCodeResult.Value);
    }

    [Fact]
    public void SendSsoCode_ValidRequest_ReturnsOk()
    {
        // Arrange
        var email = "user1@example.com";
        var ssoCode = new SsoSession { VerificationCode = "123456" };
        _userServiceMock.Setup(service => service.GetUserInfo(email)).Returns(user1);
        _ssoSessionServiceMock.Setup(service => service.CreateSso(user1.Id, email)).Returns(ssoCode);

        // Act
        var result = _controller.SendSsoCode(email);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(new { SSOCode = ssoCode.VerificationCode }, okResult.Value);
    }


    [Fact]
    public void VerifySsoCode_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var loginData = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "123456" };
        _userServiceMock.Setup(service => service.GetUserInfo(loginData.Email)).Returns((UserEntity)null);

        // Act
        var result = _controller.VerifySsoCode(loginData);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("User profile not found", notFoundResult.Value);
    }

    [Fact]
    public void VerifySsoCode_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var loginData = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "123456" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = loginData.Email };
        _userServiceMock.Setup(service => service.GetUserInfo(loginData.Email)).Returns(user);
        _ssoSessionServiceMock.Setup(service => service.ValidSso(user.Id, loginData.Code)).Returns(false);

        // Act
        var result = _controller.VerifySsoCode(loginData);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.Equal("Wrong SSO code", badRequestResult.Value);
    }

    [Fact]
    public void VerifySsoCode_ValidRequest_ReturnsOk()
    {
        // Arrange
        var loginData = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "123456" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = loginData.Email };
        var refreshToken = new RefreshToken { Token = "refreshToken", Id = Guid.NewGuid() };
        var jwt = "jwtToken";

        _userServiceMock.Setup(service => service.GetUserInfo(loginData.Email)).Returns(user);
        _ssoSessionServiceMock.Setup(service => service.ValidSso(user.Id, loginData.Code)).Returns(true);
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(user.Id)).Returns(refreshToken);
        _tokenEntityServiceMock.Setup(service => service.GenerateToken(user, refreshToken.Id.ToString())).Returns(jwt);

        // Act
        var result = _controller.VerifySsoCode(loginData);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal($"User {user.Email} is authorized. {refreshToken.Token}, {jwt}", okResult.Value);
    }

    [Fact]
    public void LoginUserByPassword_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var loginData = new UserEntityDTOLoginPassword { Email = "test@example.com", Password = "password" };
        _userServiceMock.Setup(service => service.VerifyUser(loginData)).Returns((UserEntity)null);

        // Act
        var result = _controller.LoginUserByPassword(loginData);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("User profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task LoginUserByPassword_ValidRequest_ReturnsOk()
    {
        // Arrange
        var registerData = new UserEntityDTOCreate { Email = "test@example.com", Password = "password", Name = "test" };
        var loginData = new UserEntityDTOLoginPassword { Email = registerData.Email, Password = registerData.Password };
        var user = new UserEntity
        {
            Id = Guid.NewGuid(), Email = registerData.Email, PasswordHash = registerData.Password,
            UserRole = UserRole.User, LastTimeLogged = DateTime.Now, AvatarName = "default", Name = registerData.Name
        };
        var refreshToken = new RefreshToken();
        var jwt = "jwtToken";

        _userServiceMock.Setup(service => service.CreateNewUser(registerData)).Returns(user);
        _userServiceMock.Setup(service => service.VerifyUser(loginData)).Returns(user);
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(user.Id)).Returns(refreshToken);
        _tokenEntityServiceMock.Setup(service => service.GenerateToken(user, refreshToken.Id.ToString())).Returns(jwt);

        // Act
        var registerResult = _controller.CreateNewUser(registerData);
        var loginResult = _controller.LoginUserByPassword(loginData);

        // Assert
        Assert.IsType<OkObjectResult>(loginResult);
        var okResult = loginResult as OkObjectResult;
        Assert.Equal($"User {user.Email} is authorized. {refreshToken.Token}, {jwt}", okResult.Value);
    }

    [Fact]
    public void LoginUserByPassword_InternalServerError_ReturnsStatusCode500()
    {
        // Arrange
        var loginData = new UserEntityDTOLoginPassword { Email = "test@example.com", Password = "password" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = loginData.Email };

        _userServiceMock.Setup(service => service.VerifyUser(loginData)).Returns(user);
        _refreshTokenServiceMock.Setup(service => service.GenerateRefreshToken(user.Id)).Throws(new Exception());

        // Act
        var result = _controller.LoginUserByPassword(loginData);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var statusCodeResult = result as ObjectResult;
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("There was a problem while proceeding a SSO token. Please try again later",
            statusCodeResult.Value);
    }
}