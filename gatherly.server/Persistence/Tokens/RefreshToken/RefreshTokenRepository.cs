using gatherly.server.Models.Tokens.RefreshToken;
using Microsoft.EntityFrameworkCore;
using NHibernate;

namespace gatherly.server.Persistence.Tokens.RefreshToken;

/// <summary>
///     Repository for managing refresh tokens.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ISessionFactory _sessionFactory;

    public RefreshTokenRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    ///     Generates a new refresh token for a specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The generated refresh token.</returns>
    public Models.Tokens.RefreshToken.RefreshToken GenerateRefreshToken(Guid userId)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var refreshToken = new Models.Tokens.RefreshToken.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                Expiration = DateTime.Now.AddMinutes(60),
                IsRevoked = false
            };

            session.Save(refreshToken);
            transaction.Commit();

            return refreshToken;
        }
    }

    /// <summary>
    ///     Gets a refresh token by its token string.
    /// </summary>
    /// <param name="token">The refresh token string.</param>
    /// <returns>The refresh token if found and not revoked; otherwise, null.</returns>
    public async Task<Models.Tokens.RefreshToken.RefreshToken?> GetRefreshToken(string token)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var refreshTokens = await session.Query<Models.Tokens.RefreshToken.RefreshToken>()
                .Where(rt => rt.Token == token && rt.IsRevoked == false)
                .ToListAsync();
            return refreshTokens.Count == 0 ? null : refreshTokens[0];
        }
    }

    /// <summary>
    ///     Revokes a refresh token.
    /// </summary>
    /// <param name="token">The refresh token string.</param>
    public async Task RevokeRefreshToken(string token)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var refreshToken = await session.QueryOver<Models.Tokens.RefreshToken.RefreshToken>()
                .Where(rt => rt.Token == token)
                .SingleOrDefaultAsync();

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await session.UpdateAsync(refreshToken);
                await transaction.CommitAsync();
            }
        }
    }
}