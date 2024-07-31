using gatherly.server.Models.Authentication.RecoverySession;

namespace gatherly.server.Persistence.Authentication.RecoverySession;

public class RecoverySessionService : IRecoverySessionService
{
    private readonly RecoverySessionRepository _recoverySessionRepository =
        new (NHibernateHelper.SessionFactory);
    public Models.Authentication.RecoverySession.RecoverySession CreateSession(Guid userId, string email)
    {
        return _recoverySessionRepository.CreateSession(userId, email);
    }

    public bool OpenRecoverySession(Guid id)
    {
        return _recoverySessionRepository.OpenRecoverySession(id);
    }

    public bool CloseRecoverySession(string email)
    {
        return _recoverySessionRepository.CloseRecoverySession(email);
    }
}