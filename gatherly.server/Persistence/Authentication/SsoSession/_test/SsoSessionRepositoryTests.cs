using gatherly.server.Persistence.Authentication.SsoSession;
using Moq;
using NHibernate;
using Xunit;
using ISession = NHibernate.ISession;

namespace gatherly.server.Tests.Persistence.Authentication.SsoSession;

public class SsoSessionRepositoryTests
{
    private readonly SsoSessionRepository _repository;
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public SsoSessionRepositoryTests()
    {
        _sessionMock = new Mock<ISession>();
        _transactionMock = new Mock<ITransaction>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sessionMock.Setup(s => s.BeginTransaction()).Returns(_transactionMock.Object);
        _repository = new SsoSessionRepository(_sessionMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateSso_ShouldSaveSessionAndCommitTransaction()
    {
        var ssoSession = new Models.Authentication.SsoSession.SsoSession
        {
            UserId = Guid.NewGuid(),
            UserEmail = "test@example.com",
            VerificationCode = "1234",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _unitOfWorkMock.Setup(u => u.BeginTransaction());
        _unitOfWorkMock.Setup(u => u.Commit());
        _unitOfWorkMock.Setup(u => u.Rollback());

        await _repository.CreateSso(ssoSession);

        _sessionMock.Verify(s => s.SaveAsync(ssoSession, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    [Fact]
    public async Task GetSso_ShouldReturnValidSession()
    {
        var userId = Guid.NewGuid();
        var validSsoSession = new Models.Authentication.SsoSession.SsoSession
        {
            UserId = userId,
            UserEmail = "test@example.com",
            VerificationCode = "1234",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        var queryMock = new Mock<IQuery>();
        queryMock.Setup(q => q.ListAsync<Models.Authentication.SsoSession.SsoSession>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Models.Authentication.SsoSession.SsoSession> { validSsoSession });
        _sessionMock.Setup(s => s.CreateQuery(It.IsAny<string>()))
            .Returns(queryMock.Object);

        var result = await _repository.GetSso(userId);

        Assert.NotNull(result);
        Assert.Equal(validSsoSession.UserEmail, result.UserEmail);
    }



    [Fact]
    public async Task GetSso_ShouldReturnNull_WhenNoValidSessionExists()
    {
        var userId = Guid.NewGuid();
        var sessions = new List<Models.Authentication.SsoSession.SsoSession>().AsQueryable();
        _sessionMock.Setup(s => s.Query<Models.Authentication.SsoSession.SsoSession>()).Returns(sessions);
        
        var result = await _repository.GetSso(userId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteSso_ShouldDeleteSessionAndCommitTransaction()
    {
        var sessionId = Guid.NewGuid();
        var ssoSession = new Models.Authentication.SsoSession.SsoSession { UserId = sessionId };
        _sessionMock.Setup(s => s.GetAsync<Models.Authentication.SsoSession.SsoSession>(sessionId,new CancellationToken()))
            .ReturnsAsync(ssoSession);
        _unitOfWorkMock.Setup(u => u.BeginTransaction());
        _unitOfWorkMock.Setup(u => u.Commit());
        _unitOfWorkMock.Setup(u => u.Rollback());

        await _repository.DeleteSso(sessionId);
        
        _sessionMock.Verify(s => s.DeleteAsync(ssoSession, new CancellationToken()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    [Fact]
    public async Task DeleteSso_ShouldNotCommitTransaction_WhenSessionNotFound()
    {
        var sessionId = Guid.NewGuid();
        _sessionMock.Setup(s => s.Get<Models.Authentication.SsoSession.SsoSession>(sessionId))
            .Returns((Models.Authentication.SsoSession.SsoSession?)null);
        
        await _repository.DeleteSso(sessionId);
        
        _sessionMock.Verify(
            s => s.DeleteAsync(It.IsAny<Models.Authentication.SsoSession.SsoSession>(), new CancellationToken()),
            Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(new CancellationToken()), Times.Never);
    }
}