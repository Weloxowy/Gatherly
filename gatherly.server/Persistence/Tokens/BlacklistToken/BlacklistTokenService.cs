using gatherly.server.Models.Tokens.BlacklistToken;

namespace gatherly.server.Persistence.Tokens.BlacklistToken;

public class BlacklistTokenService : IBlacklistTokenService
{
    private readonly BlacklistTokenRepository _blacklistTokenRepository = new(NHibernateHelper.SessionFactory);

    public void AddToBlacklist(string token, Guid userId, DateTime timeOfRemoval)
    {
        _blacklistTokenRepository.AddToBlacklist(token, userId, timeOfRemoval);
    }

    public void RemoveFromBlacklist(string token)
    {
        _blacklistTokenRepository.RemoveFromBlacklist(token);
    }

    public bool IsTokenBlacklisted(string token)
    {
        return _blacklistTokenRepository.IsTokenBlacklisted(token);
    }
}