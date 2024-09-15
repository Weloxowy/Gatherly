namespace gatherly.server.Models.Authentication.SsoSession;

public interface ISsoSessionRepository
{
    SsoSession CreateSso(Guid userId, string email);
    SsoSession CreateSso(string email);
    bool ValidSso(Guid userId, string code);
    bool ValidSso(string email, string code);
    bool IsTokenAlive(Guid sessionId);
    SsoSession SsoDetails(string sessionId);
    void ExpireSso(Guid sessionId);
}