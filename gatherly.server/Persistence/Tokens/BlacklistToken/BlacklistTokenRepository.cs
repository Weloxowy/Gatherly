using gatherly.server.Models.Tokens.BlacklistToken;
using NHibernate;

namespace gatherly.server.Persistence.Tokens.BlacklistToken;

/// <summary>
///     Repository for managing token blacklisting.
/// </summary>
public class BlacklistTokenRepository : IBlacklistTokenRepository
{
    private readonly ISessionFactory _sessionFactory;

    public BlacklistTokenRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    ///     Adds a token to the blacklist.
    /// </summary>
    /// <param name="token">The token to be blacklisted.</param>
    /// <param name="userId">The ID of the user associated with the token.</param>
    /// <param name="timeOfRemoval">The time when the token should be removed from the blacklist.</param>
    public void AddToBlacklist(string token, Guid userId, DateTime timeOfRemoval)
    {
        try
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var refreshToken = new Models.Tokens.BlacklistToken.BlacklistToken
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

    /// <summary>
    ///     Removes a token from the blacklist.
    /// </summary>
    /// <param name="token">The token to be removed from the blacklist.</param>
    public void RemoveFromBlacklist(string token)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var blacklistToken = session.Get<Models.Tokens.BlacklistToken.BlacklistToken>(token);
            if (blacklistToken != null)
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(blacklistToken);
                    transaction.Commit();
                }
        }
    }

    /// <summary>
    ///     Checks if a token is blacklisted.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns><c>true</c> if the token is blacklisted; otherwise, <c>false</c>.</returns>
    public bool IsTokenBlacklisted(string token)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var blacklistToken = session.Get<Models.Tokens.BlacklistToken.BlacklistToken>(token);
            if (blacklistToken == null) return true;
        }

        return false;
    }
}