namespace gatherly.server.Models.Authentication.SsoSession;

public interface ISsoSessionService
{
    public Task<SsoSession> CreateSsoSessionAsync(Guid userId, string email);

    public Task<bool> ValidateSsoSessionAsync(Guid userId, string code);
  
}