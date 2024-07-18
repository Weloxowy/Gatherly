using gatherly.server.Models.Authentication.SsoSession;

namespace gatherly.server.Persistence.Authentication.SsoSession;

public class SsoSessionService : ISsoSessionService
{
    private readonly ISsoSessionRepository _ssoSessionRepository =
        new SsoSessionRepository(NHibernateHelper.SessionFactory);


    public Models.Authentication.SsoSession.SsoSession CreateSso(Guid userId, string email)
    {
        return _ssoSessionRepository.CreateSso(userId, email);
    }

    public Models.Authentication.SsoSession.SsoSession CreateSso(string email)
    {
        return _ssoSessionRepository.CreateSso(email);
    }

    public bool ValidSso(Guid userId, string code)
    {
        return _ssoSessionRepository.ValidSso(userId, code);
    }

    public bool ValidSso(string email, string code)
    {
        return _ssoSessionRepository.ValidSso(email, code);
    }

    public bool IsTokenAlive(Guid sessionId)
    {
        return _ssoSessionRepository.IsTokenAlive(sessionId);
    }

    public Models.Authentication.SsoSession.SsoSession SsoDetails(string sessionId)
    {
        return _ssoSessionRepository.SsoDetails(sessionId);
    }

    public void ExpireSso(Guid sessionId)
    {
        _ssoSessionRepository.ExpireSso(sessionId);
    }
}