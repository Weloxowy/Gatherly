using System.Security.Cryptography;
using gatherly.server.Models.Authentication.SsoSession;

namespace gatherly.server.Persistence.Authentication.SsoSession;

/// <summary>
///     Service layer for managing SSO (Single Sign-On) sessions.
/// </summary>
public class SsoSessionService : ISsoSessionService
{
    private readonly ISsoSessionRepository _ssoSessionRepository;

    public SsoSessionService(ISsoSessionRepository ssoSessionRepository)
    {
        _ssoSessionRepository = ssoSessionRepository;
    }

    /// <summary>
        /// Function creates new SsoSession.
        /// </summary>
        /// <param name="userId">ID of an existing user</param>
        /// <param name="email">Email address of an existing user</param>
        /// <returns></returns>
        public async Task<Models.Authentication.SsoSession.SsoSession> CreateSsoSessionAsync(Guid userId, string email)
        {
            var existingSession = await _ssoSessionRepository.GetSso(userId);
            if (existingSession != null)
            {
                return existingSession;
            }
            var ssoSession = new Models.Authentication.SsoSession.SsoSession
            {
                UserId = userId,
                UserEmail = email,
                VerificationCode = GenerateVerificationCode(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
            await _ssoSessionRepository.CreateSso(ssoSession);
            return ssoSession;
        }

        /// <summary>
        /// Function validates existing SsoSession.
        /// </summary>
        /// <param name="userId">ID of an existing user</param>
        /// <param name="code">Code to validate</param>
        /// <returns></returns>
        public async Task<bool> ValidateSsoSessionAsync(Guid userId, string code)
        {
            var validSession = await _ssoSessionRepository.GetSso(userId);
            if (validSession == null || validSession.VerificationCode != code || validSession.ExpiresAt <= DateTime.UtcNow)
            {
                return false;
            }
            await _ssoSessionRepository.DeleteSso(validSession.Id);
            return true;
        }

        private static string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
        
}