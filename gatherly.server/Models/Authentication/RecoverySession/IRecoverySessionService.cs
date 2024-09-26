namespace gatherly.server.Models.Authentication.RecoverySession;

public interface IRecoverySessionService
{
    public Task<RecoverySession> CreateSession(Guid id, string email);
    public Task<bool> OpenRecoverySession(Guid id);
    public Task<bool> CloseRecoverySession(Guid id);
}