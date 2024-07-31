namespace gatherly.server.Models.Authentication.RecoverySession;

public interface IRecoverySessionService
{
    public RecoverySession CreateSession(Guid id, string email);
    public bool OpenRecoverySession(Guid id);
    public bool CloseRecoverySession(string email);
}