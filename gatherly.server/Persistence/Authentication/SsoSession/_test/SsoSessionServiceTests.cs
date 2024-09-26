using gatherly.server.Models.Authentication.SsoSession;
using Moq;
using Xunit;
using gatherly.server.Persistence.Authentication.SsoSession;
using ISession = NHibernate.ISession;

namespace gatherly.server.Tests.Persistence.Authentication.SsoSession
{
    public class SsoSessionServiceTests
    {
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISsoSessionRepository> _ssoSessionRepositoryMock;
        private readonly SsoSessionService _ssoSessionService;

        public SsoSessionServiceTests()
        {
            _sessionMock = new Mock<ISession>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _ssoSessionRepositoryMock = new Mock<ISsoSessionRepository>();
            _ssoSessionService = new SsoSessionService(_ssoSessionRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateSsoSessionAsync_ShouldReturnExistingSession_WhenSessionExists()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var existingSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = userId,
                UserEmail = email,
                VerificationCode = "12345",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _ssoSessionRepositoryMock.Setup(repo => repo.GetSso(userId))
                .ReturnsAsync(existingSession);
            
            var result = await _ssoSessionService.CreateSsoSessionAsync(userId, email);

            Assert.NotNull(result);
            Assert.Equal(existingSession.UserId, result.UserId);
            _ssoSessionRepositoryMock.Verify(repo => repo.CreateSso(It.IsAny<Models.Authentication.SsoSession.SsoSession>()), Times.Never);
        }

        [Fact]
        public async Task CreateSsoSessionAsync_ShouldCreateNewSession_WhenNoExistingSession()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            Models.Authentication.SsoSession.SsoSession createdSession = null;
            _ssoSessionRepositoryMock.Setup(repo => repo.GetSso(userId))
                .ReturnsAsync((Models.Authentication.SsoSession.SsoSession)null);
            _ssoSessionRepositoryMock.Setup(repo => repo.CreateSso(It.IsAny<Models.Authentication.SsoSession.SsoSession>()))
                .Callback<Models.Authentication.SsoSession.SsoSession>(s => createdSession = s);

            var result = await _ssoSessionService.CreateSsoSessionAsync(userId, email);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(email, result.UserEmail);
            Assert.NotNull(createdSession);
            Assert.Equal(result.VerificationCode, createdSession.VerificationCode);
        }

        [Fact]
        public async Task ValidateSsoSessionAsync_ShouldReturnFalse_WhenSessionIsNull()
        {
            var userId = Guid.NewGuid();
            var code = "12345";
            _ssoSessionRepositoryMock.Setup(repo => repo.GetSso(userId))
                .ReturnsAsync((Models.Authentication.SsoSession.SsoSession)null);

            var result = await _ssoSessionService.ValidateSsoSessionAsync(userId, code);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidateSsoSessionAsync_ShouldReturnFalse_WhenCodeIsInvalid()
        {
            var userId = Guid.NewGuid();
            var code = "12345";
            var existingSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = userId,
                UserEmail = "test@example.com",
                VerificationCode = "54321", // Different verification code
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _ssoSessionRepositoryMock.Setup(repo => repo.GetSso(userId))
                .ReturnsAsync(existingSession);

            var result = await _ssoSessionService.ValidateSsoSessionAsync(userId, code);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidateSsoSessionAsync_ShouldReturnTrue_WhenCodeIsValid()
        {
            var userId = Guid.NewGuid();
            var code = "12345";
            var existingSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = userId,
                UserEmail = "test@example.com",
                VerificationCode = code, // Same verification code
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _ssoSessionRepositoryMock.Setup(repo => repo.GetSso(userId))
                .ReturnsAsync(existingSession);

            var result = await _ssoSessionService.ValidateSsoSessionAsync(userId, code);

            Assert.True(result);
            _ssoSessionRepositoryMock.Verify(repo => repo.DeleteSso(existingSession.Id), Times.Once);
        }
    }
}
