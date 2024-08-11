using System.Security.Claims;
using gatherly.server.Models.Authentication.UserEntity;
using Xunit;

namespace gatherly.server.Persistence.Tokens.TokenEntity._test;

public class TokenEntityRepositoryTests
{
    private readonly TokenEntityRepository _tokenRepository;

    public TokenEntityRepositoryTests()
    {
        // Set a mock environment variable for the secret key
        Environment.SetEnvironmentVariable("SECRET", "supersecretkey123");
        _tokenRepository = new TokenEntityRepository();
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new UserEntity
        {
            Email = "user@example.com",
            Name = "Test User",
            UserRole = UserRole.Admin
        };
        var jti = Guid.NewGuid().ToString();

        // Act
        var token = _tokenRepository.GenerateToken(user, jti);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var user = new UserEntity
        {
            Email = "user@example.com",
            Name = "Test User",
            UserRole = UserRole.Admin
        };
        var jti = Guid.NewGuid().ToString();
        var token = _tokenRepository.GenerateToken(user, jti);

        // Act
        var claimsPrincipal = _tokenRepository.ValidateToken(token);

        // Assert
        Assert.NotNull(claimsPrincipal);
        Assert.Equal(user.Email, claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [Fact]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var claimsPrincipal = _tokenRepository.ValidateToken(invalidToken);

        // Assert
        Assert.Null(claimsPrincipal);
    }
    
    [Fact]
    public void GetEmailFromRequestCookie_ValidToken_ReturnsEmail()
    {
        // Arrange
        var user = new UserEntity
        {
            Email = "user@example.com",
            Name = "Test User",
            UserRole = UserRole.Admin
        };
        var jti = Guid.NewGuid().ToString();
        var token = _tokenRepository.GenerateToken(user, jti);

        var httpContext = new DefaultHttpContext();
        //httpContext.Request.Cookies.Append("Authorization", $"Bearer {token}");

        // Act
        var email = _tokenRepository.GetEmailFromRequestCookie(httpContext);

        // Assert
        Assert.Equal(user.Email, email);
    }

    [Fact]
    public void GetEmailFromRequestCookie_NoAuthorizationCookie_ReturnsNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        var email = _tokenRepository.GetEmailFromRequestCookie(httpContext);

        // Assert
        Assert.Null(email);
    }
    
}
