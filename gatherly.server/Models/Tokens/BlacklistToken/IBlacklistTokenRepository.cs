namespace gatherly.server.Models.Tokens.BlacklistToken;

public interface IBlacklistTokenRepository
{
    public void AddToBlacklist(string token, Guid userId, DateTime timeOfRemoval); //dodawanie do blacklisty
    public void RemoveFromBlacklist(string token); //usuwanie z blacklisty
    public bool IsTokenBlacklisted(string token); //sprawdzanie czy token jest na blackliscie
}