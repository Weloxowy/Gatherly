using gatherly.server.Models.Authentication.RecoverySession;
using NHibernate;

namespace gatherly.server.Persistence.Authentication.RecoverySession;

public class RecoverySessionRepository : IRecoverySessionRepository
{
    private readonly ISessionFactory _sessionFactory;

    public RecoverySessionRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }


    public Models.Authentication.RecoverySession.RecoverySession CreateSession(Guid userId, string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
                var existingSession = session.Query<Models.Authentication.RecoverySession.RecoverySession>()
                    .FirstOrDefault(x => x.UserId.Equals(userId));
                
                if (existingSession != null)
                {
                    return existingSession;
                }
                using (var transaction = session.BeginTransaction())
                {
                    var newRecoverySession = new Models.Authentication.RecoverySession.RecoverySession
                    {
                        UserId = userId,
                        IsOpened = false,
                        ExpiryDate = DateTime.Now.AddMinutes(10)
                    };

                    session.Save(newRecoverySession);
                    transaction.Commit();
                    return newRecoverySession;
                }
        }
    }

    public bool OpenRecoverySession(Guid id)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var existingSession = session.Query<Models.Authentication.RecoverySession.RecoverySession>()
                .FirstOrDefault(x => x.Id.Equals(id));
                
            if (existingSession == null)
            {
                return false;
            }
            
            using (var transaction = session.BeginTransaction())
            {
                existingSession.IsOpened = true;
                existingSession.ExpiryDate.AddMinutes(5);
                
                session.Save(existingSession);
                transaction.Commit();
                return true;
            }
        }
    }

    public bool CloseRecoverySession(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var existingSession = session.Query<Models.Authentication.RecoverySession.RecoverySession>()
                .Join(session.Query<Models.Authentication.UserEntity.UserEntity>(),
                    recoverySession => recoverySession.UserId,
                    userEntity => userEntity.Id,
                    (recoverySession, userEntity) => new { RecoverySession = recoverySession, UserEntity = userEntity })
                .Where(joined => joined.UserEntity.Email == email)
                .Select(joined => joined.RecoverySession)
                .FirstOrDefault();
                
            if (existingSession == null)
            {
                return false;
            }
            
            using (var transaction = session.BeginTransaction())
            {
                session.Delete(existingSession);
                transaction.Commit();
                return true;
            }
        }
    }
    
}