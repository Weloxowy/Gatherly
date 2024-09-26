using gatherly.server.Models.Authentication.RecoverySession;

namespace gatherly.server.Persistence.Authentication.RecoverySession;

/// <summary>
/// 
/// </summary>
public class RecoverySessionService : IRecoverySessionService
{
    private readonly IRecoverySessionRepository _recoverySessionRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="recoverySessionRepository"></param>
    public RecoverySessionService(IRecoverySessionRepository recoverySessionRepository)
    {
        _recoverySessionRepository = recoverySessionRepository;
    }
    
    public async Task<Models.Authentication.RecoverySession.RecoverySession> CreateSession(Guid userId, string email)
    {
        var existingSession = await _recoverySessionRepository.GetSessionByUserId(userId);
        if (existingSession != null) return existingSession;
        var recoverySession = new Models.Authentication.RecoverySession.RecoverySession
        {
            UserId = userId,
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            IsOpened = false
        };
        await _recoverySessionRepository.CreateSession(recoverySession);
        return recoverySession;
    }

    public async Task<bool> OpenRecoverySession(Guid id)
    {
        var existingSession = await _recoverySessionRepository.GetSessionByRecoveryId(id);
        if (existingSession == null) return false;
        existingSession.IsOpened = true;
        existingSession.ExpiryDate = existingSession.ExpiryDate.AddMinutes(5);
        await _recoverySessionRepository.UpdateSession(existingSession);
        return true;
    }

    public async Task<bool> CloseRecoverySession(Guid id)
    {
        var existingSession = await _recoverySessionRepository.GetSessionByUserId(id);
        if (existingSession == null) return false;
        await _recoverySessionRepository.DeleteSession(existingSession.Id);
        return true;
    }
}