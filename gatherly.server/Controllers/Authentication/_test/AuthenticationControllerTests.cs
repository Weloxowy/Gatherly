using gatherly.server.Controllers.Authentication;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using gatherly.server.Persistence.Tokens;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _controller;
    private readonly Mock<IBlacklistTokenService> _mockBlacklistTokenService;
    private readonly Mock<IMailEntityService> _mockMailEntityService;
    private readonly Mock<IRecoverySessionService> _mockRecoverySessionService;
    private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
    private readonly Mock<IResponseCookies> _mockResponseCookies;
    private readonly Mock<ISsoSessionService> _mockSsoSessionService;
    private readonly Mock<ITokenEntityService> _mockTokenEntityService;
    private readonly Mock<TokenHelper> _mockTokenHelper;
    private readonly Mock<IUserEntityService> _mockUserService;

    public AuthenticationControllerTests()
    {
        _mockUserService = new Mock<IUserEntityService>();
        _mockSsoSessionService = new Mock<ISsoSessionService>();
        _mockMailEntityService = new Mock<IMailEntityService>();
        _mockTokenEntityService = new Mock<ITokenEntityService>();
        _mockRefreshTokenService = new Mock<IRefreshTokenService>();
        _mockBlacklistTokenService = new Mock<IBlacklistTokenService>();
        _mockRecoverySessionService = new Mock<IRecoverySessionService>();
        _mockResponseCookies = new Mock<IResponseCookies>();
        _mockTokenHelper = new Mock<TokenHelper>();

        _controller = new AuthenticationController(
            _mockUserService.Object,
            _mockSsoSessionService.Object,
            _mockTokenEntityService.Object,
            _mockRefreshTokenService.Object,
            _mockBlacklistTokenService.Object,
            _mockMailEntityService.Object,
            _mockRecoverySessionService.Object,
            _mockTokenHelper.Object
        );

        var mockResponse = new Mock<HttpResponse>();
        mockResponse.Setup(r => r.Cookies).Returns(_mockResponseCookies.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    /// <summary>
    /// Test case for a valid email
    /// </summary>
    [Fact]
    public async Task SendSsoCode_ValidEmail_ReturnsOk()
    {
        var email = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email };
        var ssoCode = "123456";
        var ssoSession = new SsoSession();
        _mockUserService.Setup(s => s.GetUserInfo(email))
            .ReturnsAsync(user);
        _mockSsoSessionService.Setup(s => s.CreateSsoSessionAsync(user.Id, email))
            .ReturnsAsync(ssoSession);
        _mockMailEntityService.Setup(m => m.SendSsoCodeEmailAsync(user, ssoSession))
            .Returns(Task.CompletedTask);

        var result = await _controller.SendSsoCode(email);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Email was send", okResult.Value);
    }


    /// <summary>
    /// Test case for an invalid email (user not found)
    /// </summary>
    [Fact]
    public async Task SendSsoCode_UserNotFound_ReturnsNotFound()
    {
        var email = "notfound@example.com";
        _mockUserService.Setup(s => s.GetUserInfo(email))
            .ReturnsAsync((UserEntity)null);
        
        var result = await _controller.SendSsoCode(email);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User profile not found", notFoundResult.Value);
    }
    
    /// <summary>
    /// Test case for a server error during SSO code generation
    /// </summary>
    [Fact]
    public async Task SendSsoCode_SsoSessionServiceThrowsException_ReturnsServerError()
    {
        var email = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email };
        _mockUserService.Setup(s => s.GetUserInfo(email))
            .ReturnsAsync(user);
        _mockSsoSessionService.Setup(s => s.CreateSsoSessionAsync(user.Id, email))
            .ThrowsAsync(new Exception("Internal error"));
        
        var result = await _controller.SendSsoCode(email);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("There was a problem while creating a SSO token. Please try again later", statusCodeResult.Value);
    }
    
    /// <summary>
    /// Test for VerifySsoCode (Valid SSO code and login successful)
    /// </summary>
    [Fact]
    public async Task VerifySsoCode_ValidSsoCode_ReturnsOk()
    {
        var data = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "SSO123" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email))
            .ReturnsAsync(user);
        _mockSsoSessionService.Setup(s => s.ValidateSsoSessionAsync(user.Id, data.Code))
            .ReturnsAsync(true);
        _mockTokenHelper.Setup(t => t.GenerateTokens(user))
            .Returns(("jwtToken123", "refreshToken123"));

        var result = await _controller.VerifySsoCode(data);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Login successfully", okResult.Value);
    }
    
    /// <summary>
    /// Test for VerifySsoCode (User not found)
    /// </summary>
    [Fact]
    public async Task VerifySsoCode_UserNotFound_ReturnsNotFound()
    {
        var data = new UserEntityDTOLoginCode { Email = "notfound@example.com", Code = "SSO123" };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email))
            .ReturnsAsync((UserEntity)null);

        var result = await _controller.VerifySsoCode(data);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User profile not found", notFoundResult.Value);
    }
    
    /// <summary>
    /// Test for VerifySsoCode (Invalid SSO code)
    /// </summary>
    [Fact]
    public async Task VerifySsoCode_InvalidSsoCode_ReturnsBadRequest()
    {
        var data = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "wrongCode" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email))
            .ReturnsAsync(user);
        _mockSsoSessionService.Setup(s => s.ValidateSsoSessionAsync(user.Id, data.Code))
            .ReturnsAsync(false);

        var result = await _controller.VerifySsoCode(data);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Wrong SSO code", badRequestResult.Value);
    }
    
    /// <summary>
    /// Test for VerifySsoCode (Server error during token generation)
    /// </summary>
    [Fact]
    public async Task VerifySsoCode_ServerError_ReturnsServerError()
    {
        var data = new UserEntityDTOLoginCode { Email = "test@example.com", Code = "SSO123" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email))
            .ReturnsAsync(user);
        _mockSsoSessionService.Setup(s => s.ValidateSsoSessionAsync(user.Id, data.Code))
            .ReturnsAsync(true);
        _mockTokenEntityService.Setup(t => t.GenerateToken(user, "x")).Throws(new Exception("Token generation failed"));
        
        var result = await _controller.VerifySsoCode(data);
        
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("There was a problem while creating a SSO token. Please try again later", statusCodeResult.Value);
    }
    
    /// <summary>
    /// Test for LoginUserByPassword (Valid credentials and login successful)
    /// </summary>
    [Fact]
    public async Task LoginUserByPassword_ValidCredentials_ReturnsOk()
    {
        var data = new UserEntityDTOLoginPassword { Email = "test@example.com", Password = "password123" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.IsUserExists(data.Email))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.VerifyUser(data))
            .ReturnsAsync(user);
        _mockTokenHelper.Setup(t => t.GenerateTokens(user))
            .Returns(("jwtToken123", "refreshToken123"));

        var result = await _controller.LoginUserByPassword(data);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Login successfully", okResult.Value);
    }
    
    /// <summary>
    /// Test for LoginUserByPassword (User not found)
    /// </summary>
    [Fact]
    public async Task LoginUserByPassword_UserNotFound_ReturnsNotFound()
    {
        var data = new UserEntityDTOLoginPassword { Email = "notfound@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.IsUserExists(data.Email))
            .ReturnsAsync(false);

        var result = await _controller.LoginUserByPassword(data);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User profile not found", notFoundResult.Value);
    }
    
    /// <summary>
    /// Test for LoginUserByPassword (Invalid credentials)
    /// </summary>
    [Fact]
    public async Task LoginUserByPassword_InvalidCredentials_ReturnsUnauthorized()
    {
        var data = new UserEntityDTOLoginPassword { Email = "test@example.com", Password = "wrongPassword" };
        _mockUserService.Setup(s => s.IsUserExists(data.Email))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.VerifyUser(data))
            .ReturnsAsync((UserEntity)null);

        var result = await _controller.LoginUserByPassword(data);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Credentials are invalid", unauthorizedResult.Value);
    }

    /// <summary>
    /// Test for LoginUserByPassword (Server error during token generation)
    /// </summary>
    [Fact]
    public async Task LoginUserByPassword_ServerError_ReturnsServerError()
    {
        var data = new UserEntityDTOLoginPassword { Email = "test@example.com", Password = "password123" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.IsUserExists(data.Email))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.VerifyUser(data))
            .ReturnsAsync(user);
        _mockTokenEntityService.Setup(t => t.GenerateToken(user, It.IsAny<string>()))
            .Throws(new Exception("Token generation failed"));

        var result = await _controller.LoginUserByPassword(data);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("There was a problem while proceeding a validation. Please try again later",
            statusCodeResult.Value);
    }

    /// <summary>
    /// Test for CreateNewUser (Valid credentials and login successful)
    /// </summary>
    [Fact]
    public async Task CreateNewUser_ValidData_ReturnsOk()
    {
        var data = new UserEntityDTOCreate { Email = "newuser@example.com", Password = "password123" };
        var newUser = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.IsUserExists(data.Email)).ReturnsAsync(false);
        _mockUserService.Setup(s => s.CreateNewUser(data)).ReturnsAsync(newUser);
        _mockTokenHelper.Setup(t => t.GenerateTokens(newUser))
            .Returns(("jwtToken123", "refreshToken123"));
        
        var result = await _controller.CreateNewUser(data);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Login successfully", okResult.Value);
    }

    /// <summary>
    /// Test for CreateNewUser (User found)
    /// </summary>
    [Fact]
    public async Task CreateNewUser_UserExists_ReturnsBadRequest()
    {
        var data = new UserEntityDTOCreate { Email = "existinguser@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.IsUserExists(data.Email)).ReturnsAsync(true);

        var result = await _controller.CreateNewUser(data);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email address is already used", badRequestResult.Value);
    }

    /// <summary>
    /// Test for Logout (Logout successful)
    /// </summary>
    [Fact]
    public async Task Logout_ValidRequest_ReturnsOk()
    {
        var user = new UserEntity { Id = Guid.NewGuid(), Email = "test@example.com" };
        _mockTokenEntityService.Setup(t => t.GetEmailFromRequestCookie(It.IsAny<HttpContext>()))
            .Returns(user.Email);
        _mockUserService.Setup(s => s.GetUserInfo(user.Email)).ReturnsAsync(user);
        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockRequestCookies.Setup(c => c["Authorization"]).Returns("Bearer someJwtToken");
        mockRequestCookies.Setup(c => c["RefreshToken"]).Returns("refreshToken123");
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Cookies).Returns(mockRequestCookies.Object);

        var result = await _controller.Logout();
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User logged out successfully.", okResult.Value);
    }

    /// <summary>
    /// Test for SendRecover (Send successful)
    /// </summary>
    [Fact]
    public async Task SendRecover_ValidEmail_ReturnsOk()
    {
        var email = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email };
        _mockUserService.Setup(s => s.GetUserInfo(email)).ReturnsAsync(user);
 
        var result = await _controller.SendRecover(email);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Email was send successfully", okResult.Value);
    }

    /// <summary>
    /// Test for SendRecover (User not found)
    /// </summary>
    [Fact]
    public async Task SendRecover_UserNotFound_ReturnsNotFound()
    {
        var email = "nonexistent@example.com";
        _mockUserService.Setup(s => s.GetUserInfo(email)).ReturnsAsync((UserEntity)null);

        var result = await _controller.SendRecover(email);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User profile not found", notFoundResult.Value);
    }

    /// <summary>
    /// Test for ValidRecover (Valid successful)
    /// </summary>
    [Fact]
    public async Task ValidRecover_ValidId_ReturnsOk()
    {
        var sessionId = Guid.NewGuid().ToString();
        _mockRecoverySessionService.Setup(r => r.OpenRecoverySession(Guid.Parse(sessionId))).ReturnsAsync(true);

        var result = await _controller.ValidRecover(sessionId);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test for ValidRecover (User not found)
    /// </summary>
    [Fact]
    public async Task ValidRecover_InvalidId_ReturnsNotFound()
    {
        var sessionId = Guid.NewGuid().ToString();
        _mockRecoverySessionService.Setup(r => r.OpenRecoverySession(Guid.Parse(sessionId))).ReturnsAsync(false);

        var result = await _controller.ValidRecover(sessionId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Active recovery session not found", notFoundResult.Value);
    }

    /// <summary>
    /// Test for ApplyNewPasswordRecover (Valid successful)
    /// </summary>
    [Fact]
    public async Task ApplyNewPasswordRecover_ValidRequest_ReturnsOk()
    {
        var data = new UserEntityDTOResetPassword { Email = "test@example.com", NewPassword = "newPass123" };
        var user = new UserEntity { Id = Guid.NewGuid(), Email = data.Email };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email)).ReturnsAsync(user);
        _mockRecoverySessionService.Setup(r => r.CloseRecoverySession(user.Id)).ReturnsAsync(true);
        _mockUserService.Setup(s => s.ChangeUserPassword(data)).ReturnsAsync(user);

        var result = await _controller.ApplyNewPasswordRecover(data);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Recovery was successful", okResult.Value);
    }

    /// <summary>
    /// Test for ApplyNewPasswordRecover (User not found)
    /// </summary>
    [Fact]
    public async Task ApplyNewPasswordRecover_UserNotFound_ReturnsNotFound()
    {
        var data = new UserEntityDTOResetPassword { Email = "nonexistent@example.com", NewPassword = "newPass123" };
        _mockUserService.Setup(s => s.GetUserInfo(data.Email)).ReturnsAsync((UserEntity?)null);

        var result = await _controller.ApplyNewPasswordRecover(data);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User with this email does not exists.", notFoundResult.Value);
    }
}