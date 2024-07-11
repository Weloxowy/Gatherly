using System.IdentityModel.Tokens.Jwt;
using gatherly.server.Models.Authentication.SsoSession;
using NHibernate;

namespace gatherly.server.Persistence.Authentication.SsoSession;

public class SsoSessionRepository : ISsoSessionRepository
{
    
    private readonly ISessionFactory _sessionFactory;

    public SsoSessionRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }
    
    private static string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public Models.Authentication.SsoSession.SsoSession CreateSso(Guid userId, string email)
{
    using (var session = _sessionFactory.OpenSession())
    {
        using (var transaction = session.BeginTransaction())
        {
            var existingSsoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                .FirstOrDefault(x => x.UserEmail == email && !x.IsVerified && x.ExpiresAt > DateTime.Now);

            if (existingSsoSession != null)
            {
                Console.WriteLine($"Returning existing active SSO session for email: {email}");
                return existingSsoSession;
            }

            Random random = new Random();
            var ssoSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = userId,
                UserEmail = email,
                VerificationCode = GenerateVerificationCode(),
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10),
                IsVerified = false
            };

            session.Save(ssoSession);
            transaction.Commit();
            return ssoSession;
        }
    }
}

public Models.Authentication.SsoSession.SsoSession CreateSso(string email)
{
    using (var session = _sessionFactory.OpenSession())
    {
        using (var transaction = session.BeginTransaction())
        {
            var existingSsoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                .FirstOrDefault(x => x.UserEmail == email && !x.IsVerified && x.ExpiresAt > DateTime.Now);

            if (existingSsoSession != null)
            {
                Console.WriteLine($"Returning existing active SSO session for email: {email}");
                return existingSsoSession;
            }

            Random random = new Random();
            var ssoSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = null,
                UserEmail = email,
                VerificationCode = GenerateVerificationCode(),
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10),
                IsVerified = false
            };

            session.Save(ssoSession);
            transaction.Commit();
            return ssoSession;
        }
    }
}

    public bool ValidSso(Guid userId, string code)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            { 
                //orderBy zapobiegnie walidacji po losowym rekordzie gdyby ktorys sie nie usunal
                var ssoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                    .Where(x => x.UserId.HasValue && x.UserId.Value == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (ssoSession == null)
                {
                    Console.WriteLine($"SSO session not found for userId: {userId}");
                    return false;
                }

                if (ssoSession.VerificationCode != code)
                {
                    Console.WriteLine($"SSO session code mismatch. Expected: {ssoSession.VerificationCode}, Received: {code}");
                    return false;
                }

                if (ssoSession.ExpiresAt <= DateTime.Now)
                {
                    Console.WriteLine($"SSO session expired at: {ssoSession.ExpiresAt}");
                    return false;
                }

                ssoSession.IsVerified = true;
                session.Delete(ssoSession);
                transaction.Commit();
                return true;
            }
        }
    }


    public bool ValidSso(string email, string code)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var ssoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                    .FirstOrDefault(x => x.UserEmail.Equals(email));
                if (ssoSession != null && ssoSession.VerificationCode.Equals(code) 
                                       && ssoSession.ExpiresAt > DateTime.Now)
                {
                    
                    session.Delete(ssoSession);
                    transaction.Commit();
                    return true;
                }
                return false;
            }
        }
    }

    public bool IsTokenAlive(Guid sessionId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var sso = session.Query<Models.Authentication.SsoSession.SsoSession>().FirstOrDefault(s => s.Id.Equals(sessionId));
            if (sso == null)
            {
                return false;
            }
            return true;
        }
    }

    public Models.Authentication.SsoSession.SsoSession SsoDetails(string sessionId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var sso = session.Get<Models.Authentication.SsoSession.SsoSession>(sessionId);
            if (sso == null)
            {
                return null;
            }
            return sso; 
        }
    }

    public void ExpireSso(Guid sessionId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var ssoSession = session.Get<Models.Authentication.SsoSession.SsoSession>(sessionId);
                if (ssoSession != null)
                {
                    session.Delete(ssoSession);
                    transaction.Commit();
                }
            }
        }
    }
    
}