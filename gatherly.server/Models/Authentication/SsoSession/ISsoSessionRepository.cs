namespace gatherly.server.Models.Authentication.SsoSession;

public interface ISsoSessionRepository
{
    Task CreateSso(SsoSession ssoSession);
    Task<SsoSession?> GetSso(Guid userId);
    Task DeleteSso(Guid sessionId);
    
}