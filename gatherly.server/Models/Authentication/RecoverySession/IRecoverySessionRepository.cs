namespace gatherly.server.Models.Authentication.RecoverySession;

public interface IRecoverySessionRepository
{
    Task CreateSession(RecoverySession recoverySession);
    Task<RecoverySession?> GetSessionByUserId(Guid userId);
    Task<RecoverySession?> GetSessionByRecoveryId(Guid Id);
    Task UpdateSession(RecoverySession recoverySession);
    Task DeleteSession(Guid sessionId);

}