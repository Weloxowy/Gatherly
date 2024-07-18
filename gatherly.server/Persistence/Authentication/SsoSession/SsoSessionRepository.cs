using gatherly.server.Models.Authentication.SsoSession;
using NHibernate;

namespace gatherly.server.Persistence.Authentication.SsoSession;

/// <summary>
///     Repository for managing SSO (Single Sign-On) sessions.
/// </summary>
public class SsoSessionRepository : ISsoSessionRepository
{
    private readonly ISessionFactory _sessionFactory;

    public SsoSessionRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    ///     Creates a new SSO session for a user specified by user ID and email.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="email">The email of the user.</param>
    /// <returns>The created SSO session.</returns>
    public Models.Authentication.SsoSession.SsoSession CreateSso(Guid userId, string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var existingSsoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                    .FirstOrDefault(x => x.UserEmail == email && x.ExpiresAt > DateTime.Now);

                if (existingSsoSession != null)
                {
                    Console.WriteLine($"Returning existing active SSO session for email: {email}");
                    return existingSsoSession;
                }

                var random = new Random();
                var ssoSession = new Models.Authentication.SsoSession.SsoSession
                {
                    UserId = userId,
                    UserEmail = email,
                    VerificationCode = GenerateVerificationCode(),
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(10)
                };

                session.Save(ssoSession);
                transaction.Commit();
                return ssoSession;
            }
        }
    }

    /// <summary>
    ///     Creates a new SSO session for a user specified by email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The created SSO session.</returns>
    public Models.Authentication.SsoSession.SsoSession CreateSso(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var existingSsoSession = session.Query<Models.Authentication.SsoSession.SsoSession>()
                    .FirstOrDefault(x => x.UserEmail == email && x.ExpiresAt > DateTime.Now);

                if (existingSsoSession != null)
                {
                    Console.WriteLine($"Returning existing active SSO session for email: {email}");
                    return existingSsoSession;
                }

                var random = new Random();
                var ssoSession = new Models.Authentication.SsoSession.SsoSession
                {
                    UserId = null,
                    UserEmail = email,
                    VerificationCode = GenerateVerificationCode(),
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(10)
                };

                session.Save(ssoSession);
                transaction.Commit();
                return ssoSession;
            }
        }
    }

    /// <summary>
    ///     Validates an SSO session by user ID and verification code.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="code">The verification code.</param>
    /// <returns><c>true</c> if the SSO session is valid; otherwise, <c>false</c>.</returns>
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
                    Console.WriteLine(
                        $"SSO session code mismatch. Expected: {ssoSession.VerificationCode}, Received: {code}");
                    return false;
                }

                if (ssoSession.ExpiresAt <= DateTime.Now)
                {
                    Console.WriteLine($"SSO session expired at: {ssoSession.ExpiresAt}");
                    return false;
                }

                session.Delete(ssoSession);
                transaction.Commit();
                return true;
            }
        }
    }

    /// <summary>
    ///     Validates an SSO session by email and verification code.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="code">The verification code.</param>
    /// <returns><c>true</c> if the SSO session is valid; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    ///     Checks if an SSO session token is still alive.
    /// </summary>
    /// <param name="sessionId">The ID of the SSO session.</param>
    /// <returns><c>true</c> if the SSO session token is alive; otherwise, <c>false</c>.</returns>
    public bool IsTokenAlive(Guid sessionId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var sso = session.Query<Models.Authentication.SsoSession.SsoSession>()
                .FirstOrDefault(s => s.Id.Equals(sessionId));
            if (sso == null) return false;
            return true;
        }
    }

    /// <summary>
    ///     Gets the details of an SSO session by session ID.
    /// </summary>
    /// <param name="sessionId">The ID of the SSO session.</param>
    /// <returns>The SSO session details.</returns>
    public Models.Authentication.SsoSession.SsoSession SsoDetails(string sessionId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var sso = session.Get<Models.Authentication.SsoSession.SsoSession>(sessionId);
            if (sso == null) return null;
            return sso;
        }
    }

    /// <summary>
    ///     Expires an SSO session by session ID.
    /// </summary>
    /// <param name="sessionId">The ID of the SSO session to expire.</param>
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

    /// <summary>
    ///     Generates a verification code for SSO.
    /// </summary>
    /// <returns>The generated verification code.</returns>
    private static string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}