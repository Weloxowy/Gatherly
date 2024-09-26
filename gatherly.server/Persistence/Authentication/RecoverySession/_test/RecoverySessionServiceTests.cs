using gatherly.server.Models.Authentication.RecoverySession;
using Moq;
using Xunit;
using ISession = NHibernate.ISession;

namespace gatherly.server.Persistence.Authentication.RecoverySession
{
    public class RecoverySessionServiceTests
    {
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRecoverySessionRepository> _recoverySessionRepositoryMock;
        private readonly RecoverySessionService _recoverySessionService;

        public RecoverySessionServiceTests()
        {
            _sessionMock = new Mock<ISession>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _recoverySessionRepositoryMock = new Mock<IRecoverySessionRepository>();
            _recoverySessionService = new RecoverySessionService(_recoverySessionRepositoryMock.Object);
        }

        /// <summary>
        /// Test for CreateSessionAsync (Operation successful)
        /// </summary>
        [Fact]
        public async Task CreateSessionAsync_ShouldReturnExistingSession_WhenSessionExists()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var existingSession = new Models.Authentication.RecoverySession.RecoverySession()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IsOpened = false,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5)
            };
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync(existingSession);

            var result = await _recoverySessionService.CreateSession(userId, email);

            Assert.NotNull(result);
            Assert.Equal(existingSession.UserId, result.UserId);
        }

        /// <summary>
        /// Test for CreateSessionAsync (Operation returns null)
        /// </summary>
        [Fact]
        public async Task CreateSessionAsync_ShouldCreateNewSession_WhenNoExistingSession()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            Models.Authentication.RecoverySession.RecoverySession createdSession = null;
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync((Models.Authentication.RecoverySession.RecoverySession)null);
            _recoverySessionRepositoryMock.Setup(repo => repo.CreateSession(It.IsAny<Models.Authentication.RecoverySession.RecoverySession>()))
                .Callback<Models.Authentication.RecoverySession.RecoverySession>(s => createdSession = s);

            var result = await _recoverySessionService.CreateSession(userId, email);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.NotNull(createdSession);
        }

        /// <summary>
        /// Test for GetOpenSessionAsync (Operation returns false)
        /// </summary>
        [Fact]
        public async Task GetOpenSessionAsync_ShouldReturnFalse_WhenSessionIsNull()
        {
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync((Models.Authentication.RecoverySession.RecoverySession)null);

            var result = await _recoverySessionService.OpenRecoverySession(id);

            Assert.False(result);
        }
        
        /// <summary>
        /// Test for GetCloseSessionAsync (Operation returns false)
        /// </summary>
        [Fact]
        public async Task GetCloseSessionAsync_ShouldReturnFalse_WhenSessionIsNull()
        {
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync((Models.Authentication.RecoverySession.RecoverySession)null);

            var result = await _recoverySessionService.CloseRecoverySession(id);

            Assert.False(result);
        }

        /// <summary>
        /// Test for OpenSessionAsync (Operation returns false)
        /// </summary>
        [Fact]
        public async Task OpenSessionAsync_ShouldReturnFalse_WhenSessionClose()
        {
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var existingSession = new Models.Authentication.RecoverySession.RecoverySession()
            {
                Id = userId,
                UserId = userId,
                IsOpened = true,
                ExpiryDate = DateTime.UtcNow
            };
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync(existingSession);

            var result = await _recoverySessionService.OpenRecoverySession(id);

            Assert.False(result);
        }
        
        /// <summary>
        /// Test for OpenSessionAsync (Operation returns true)
        /// </summary>
        [Fact]
        public async Task OpenSessionAsync_ShouldReturnTrue_WhenSessionClose()
        {
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var existingSession = new Models.Authentication.RecoverySession.RecoverySession()
            {
                Id = id,
                UserId = userId,
                IsOpened = false,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5)
            };
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByRecoveryId(id))
                .ReturnsAsync(existingSession);

            var result = await _recoverySessionService.OpenRecoverySession(existingSession.Id);

            Assert.True(result);
        }
        
        /// <summary>
        /// Test for CloseSessionAsync (Operation returns false)
        /// </summary>
        [Fact]
        public async Task CloseSessionAsync_ShouldReturnFalse_WhenSessionOpen()
        {
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var existingSession = new Models.Authentication.RecoverySession.RecoverySession()
            {
                Id = userId,
                UserId = userId,
                IsOpened = false,
                ExpiryDate = DateTime.UtcNow
            };
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByRecoveryId(id))
                .ReturnsAsync(existingSession);

            var result = await _recoverySessionService.CloseRecoverySession(id);

            Assert.False(result);
        }
        
        /// <summary>
        /// Test for CloseSessionAsync (Operation returns true)
        /// </summary>
        [Fact]
        public async Task CloseSessionAsync_ShouldReturnTrue_WhenSessionClose()
        {
            var userId = Guid.NewGuid();
            var existingSession = new Models.Authentication.RecoverySession.RecoverySession()
            {
                Id = userId,
                UserId = userId,
                IsOpened = true,
                ExpiryDate = DateTime.UtcNow
            };
            _recoverySessionRepositoryMock.Setup(repo => repo.GetSessionByUserId(userId))
                .ReturnsAsync(existingSession);

            var result = await _recoverySessionService.CloseRecoverySession(userId);
            
            Assert.True(result);
        }

    }
}
