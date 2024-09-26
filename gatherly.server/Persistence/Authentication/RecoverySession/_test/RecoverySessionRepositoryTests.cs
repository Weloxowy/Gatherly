using Moq;
using NHibernate;
using Xunit;
using ISession = NHibernate.ISession;

namespace gatherly.server.Persistence.Authentication.RecoverySession;

public class RecoverySessionRepositoryTests
{
    private readonly RecoverySessionRepository _repository;
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public RecoverySessionRepositoryTests()
    {
        // Setup mock objects
        _sessionMock = new Mock<ISession>();
        _transactionMock = new Mock<ITransaction>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sessionMock.Setup(s => s.BeginTransaction()).Returns(_transactionMock.Object);

        // Create instance of repository with mock session and unit of work
        _repository = new RecoverySessionRepository(_sessionMock.Object, _unitOfWorkMock.Object);
    }

    /// <summary>
    /// Test for CreateSession (Create successful)
    /// </summary>
    [Fact]
    public async Task CreateSession_ShouldSaveSessionAndCommitTransaction()
    {
        var session = new Models.Authentication.RecoverySession.RecoverySession
        {
            ExpiryDate = DateTime.UtcNow,
            IsOpened = false,
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        _unitOfWorkMock.Setup(u => u.BeginTransaction());
        _unitOfWorkMock.Setup(u => u.Commit());
        _unitOfWorkMock.Setup(u => u.Rollback());

        await _repository.CreateSession(session);
        
        _sessionMock.Verify(s => s.SaveAsync(session, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    /// <summary>
    /// Test for GetSessionByUserId (Operation successful)
    /// </summary>
    [Fact]
    public async Task GetSessionByUserId_ShouldReturnValidSession()
    {
        var userId = Guid.NewGuid();
        var validRecoverySession = new Models.Authentication.RecoverySession.RecoverySession
        {
            UserId = userId,
            Id = Guid.NewGuid(),
            IsOpened = false,
            ExpiryDate = DateTime.UtcNow
        };
        var queryMock = new Mock<IQuery>();
        queryMock.Setup(q => q.ListAsync<Models.Authentication.RecoverySession.RecoverySession>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Models.Authentication.RecoverySession.RecoverySession> { validRecoverySession });
        _sessionMock.Setup(s => s.CreateQuery(It.IsAny<string>()))
            .Returns(queryMock.Object);

        var result = await _repository.GetSessionByUserId(userId);

        Assert.NotNull(result);
        Assert.Equal(validRecoverySession.UserId, result.UserId);
    }
    
    /// <summary>
    /// Test for GetSessionByUserId (Operation returns null)
    /// </summary>
    [Fact]
    public async Task GetSessionByUserId_ShouldReturnNull_WhenNoValidSessionExists()
    {
        var userId = Guid.NewGuid();
        var validRecoverySession = new Models.Authentication.RecoverySession.RecoverySession
        {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            IsOpened = false,
            ExpiryDate = DateTime.UtcNow
        };
        var queryMock = new Mock<IQuery>();
        queryMock.Setup(q => q.ListAsync<Models.Authentication.RecoverySession.RecoverySession>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Models.Authentication.RecoverySession.RecoverySession> { validRecoverySession });
        _sessionMock.Setup(s => s.CreateQuery(It.IsAny<string>()))
            .Returns(queryMock.Object);

        var result = await _repository.GetSessionByUserId(userId);

        Assert.Null(result);
    }
    
    /// <summary>
    /// Test for GetSessionByRecoveryId (Operation returns session)
    /// </summary>
    [Fact]
    public async Task GetSessionByRecoveryId_ShouldReturnValidSession()
    {
        var id = Guid.NewGuid();
        var validRecoverySession = new Models.Authentication.RecoverySession.RecoverySession
        {
            UserId = Guid.NewGuid(),
            Id = id,
            IsOpened = false,
            ExpiryDate = DateTime.UtcNow
        };
        var queryMock = new Mock<IQuery>();
        queryMock.Setup(q => q.ListAsync<Models.Authentication.RecoverySession.RecoverySession>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Models.Authentication.RecoverySession.RecoverySession> { validRecoverySession });
        _sessionMock.Setup(s => s.CreateQuery(It.IsAny<string>()))
            .Returns(queryMock.Object);

        var result = await _repository.GetSessionByRecoveryId(id);

        Assert.NotNull(result);
        Assert.Equal(validRecoverySession.UserId, result.UserId);
    }
    
    /// <summary>
    /// Test for GetSessionByRecoveryId (Operation returns null)
    /// </summary>
    [Fact]
    public async Task GetSessionByRecoveryId_ShouldReturnNull_WhenNoValidSessionExists()
    {
        var id = Guid.NewGuid();
        var validRecoverySession = new Models.Authentication.RecoverySession.RecoverySession
        {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            IsOpened = false,
            ExpiryDate = DateTime.UtcNow
        };
        var queryMock = new Mock<IQuery>();
        queryMock.Setup(q => q.ListAsync<Models.Authentication.RecoverySession.RecoverySession>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Models.Authentication.RecoverySession.RecoverySession> { validRecoverySession });
        _sessionMock.Setup(s => s.CreateQuery(It.IsAny<string>()))
            .Returns(queryMock.Object);
        
        var result = await _repository.GetSessionByRecoveryId(id);

        Assert.Null(result);
    }
    
    /// <summary>
    /// Test for DeleteSession (Operation successful)
    /// </summary>
    [Fact]
    public async Task DeleteSession_ShouldDeleteSessionAndCommitTransaction()
    {
        var sessionId = Guid.NewGuid();
        var recoverySession = new Models.Authentication.RecoverySession.RecoverySession{ UserId = sessionId };
        _sessionMock.Setup(s => s.GetAsync<Models.Authentication.RecoverySession.RecoverySession>(sessionId,new CancellationToken()))
            .ReturnsAsync(recoverySession); // Upewnij się, że używasz async
        _unitOfWorkMock.Setup(u => u.BeginTransaction());
        _unitOfWorkMock.Setup(u => u.Commit());
        _unitOfWorkMock.Setup(u => u.Rollback());

        await _repository.DeleteSession(sessionId);
        
        _sessionMock.Verify(s => s.DeleteAsync(recoverySession, new CancellationToken()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    /// <summary>
    /// Test for DeleteSession (Operation returns null)
    /// </summary>
    [Fact]
    public async Task DeleteSession_ShouldNotCommitTransaction_WhenSessionNotFound()
    {
        var sessionId = Guid.NewGuid();
        _sessionMock.Setup(s => s.Get<Models.Authentication.RecoverySession.RecoverySession>(sessionId))
            .Returns((Models.Authentication.RecoverySession.RecoverySession?)null);

        await _repository.DeleteSession(sessionId);

        _sessionMock.Verify(
            s => s.DeleteAsync(It.IsAny<Models.Authentication.RecoverySession.RecoverySession>(), new CancellationToken()),
            Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(new CancellationToken()), Times.Never);
    }
    
}