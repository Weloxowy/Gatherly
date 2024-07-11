using gatherly.server.Models.Tokens.BlacklistToken;
using NHibernate;

namespace gatherly.server.Persistence.Tokens.BlacklistToken;

public class BlacklistTokenRepository : IBlacklistTokenRepository
{
    private readonly ISessionFactory _sessionFactory;

    public BlacklistTokenRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public void AddToBlacklist(string token, Guid userId, DateTime timeOfRemoval)
    {
        try
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var refreshToken = new Models.Tokens.BlacklistToken.BlacklistToken()
                    {
                        Token = token,
                        UserId = userId,
                        EndOfBlacklisting = timeOfRemoval.AddMinutes(30)
                    };
                    session.Save(refreshToken);
                    transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public void RemoveFromBlacklist(string token)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var blacklistToken = session.Get<Models.Tokens.BlacklistToken.BlacklistToken>(token);
            if (blacklistToken != null)
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(blacklistToken);
                    transaction.Commit();
                }
            }
        }
    }

    public bool IsTokenBlacklisted(string token)
    {
            using (var session = _sessionFactory.OpenSession())
            {
                var blacklistToken = session.Get<Models.Tokens.BlacklistToken.BlacklistToken>(token);
                if (blacklistToken == null)
                {
                    return true;
                }
            }
            return false;
    }
}