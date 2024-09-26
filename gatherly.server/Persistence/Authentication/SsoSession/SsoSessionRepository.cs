using gatherly.server.Models.Authentication.SsoSession;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace gatherly.server.Persistence.Authentication.SsoSession;

/// <summary>
///     Repository layer for managing SSO (Single Sign-On) sessions.
/// </summary>
public class  SsoSessionRepository : ISsoSessionRepository
{
    private readonly ISession _session;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor for SsoSessionRepository.
    /// </summary>
    /// <param name="session">Object of ISession interface</param>
    /// <param name="unitOfWork">Object of IUnitOfWork interface</param>
    public SsoSessionRepository(ISession session, IUnitOfWork unitOfWork)
    {
        _session = session;
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    ///     Creates a new SSO session entity in database.
    /// </summary>
    /// <param name="ssoSession">SsoSession entity.</param>
    public async Task CreateSso(Models.Authentication.SsoSession.SsoSession ssoSession)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _session.SaveAsync(ssoSession);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    /// <summary>
    ///     Gets SsoSession entity from database.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>SsoSession entity.</returns>
    public async Task<Models.Authentication.SsoSession.SsoSession?> GetSso(Guid userId)
    {
        return await _session.Query<Models.Authentication.SsoSession.SsoSession>()
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    ///     Deletes SsoSession entity in database.
    /// </summary>
    /// <param name="sessionId">The ID of the SSO session.</param>
    public async Task DeleteSso(Guid sessionId)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var ssoSession = await _session.GetAsync<Models.Authentication.SsoSession.SsoSession>(sessionId);
            if (ssoSession != null)
            {
                await _session.DeleteAsync(ssoSession);
                _unitOfWork.Commit();
            }
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}