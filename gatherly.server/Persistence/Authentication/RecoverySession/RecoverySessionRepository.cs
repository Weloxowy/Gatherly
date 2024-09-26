using gatherly.server.Models.Authentication.RecoverySession;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace gatherly.server.Persistence.Authentication.RecoverySession;

public class RecoverySessionRepository : IRecoverySessionRepository
{
    private readonly ISession _session;
    private readonly IUnitOfWork _unitOfWork;

    public RecoverySessionRepository(ISession session, IUnitOfWork unitOfWork)
    {
        _session = session;
        _unitOfWork = unitOfWork;
    }
    
    public async Task CreateSession(Models.Authentication.RecoverySession.RecoverySession recoverySession)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _session.SaveAsync(recoverySession);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<Models.Authentication.RecoverySession.RecoverySession?> GetSessionByUserId(Guid userId)
    {
        return await _session.Query<Models.Authentication.RecoverySession.RecoverySession>()
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.ExpiryDate > DateTime.UtcNow);
    }
    
    public async Task<Models.Authentication.RecoverySession.RecoverySession?> GetSessionByRecoveryId(Guid id)
    {
        return await _session.Query<Models.Authentication.RecoverySession.RecoverySession>()
            .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.ExpiryDate > DateTime.UtcNow);
    }

    public async Task UpdateSession(Models.Authentication.RecoverySession.RecoverySession recoverySession)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _session.UpdateAsync(recoverySession);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task DeleteSession(Guid sessionId)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var recoverySession = await _session.GetAsync<Models.Authentication.RecoverySession.RecoverySession>(sessionId);
            if (recoverySession != null)
            {
                await _session.DeleteAsync(recoverySession);
                _unitOfWork.Commit();
            }
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
    
}