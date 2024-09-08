using System.Diagnostics.CodeAnalysis;
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
                Expiration = DateTime.UtcNow.AddMinutes(60),
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
    [SuppressMessage("ReSharper.DPA", "DPA0008: Large number of DB connections", MessageId = "count: 16")]
    public Models.Tokens.RefreshToken.RefreshToken? GetRefreshToken(string token)
    {
        try
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var refreshTokens = session.Query<Models.Tokens.RefreshToken.RefreshToken>()
                    .Where(rt => rt.Token == token && rt.IsRevoked == false)
                    .ToList(); // Synchroniczne pobieranie danych

                return refreshTokens.Count == 0 ? null : refreshTokens[0];
            }
        }
        catch (Exception ex)
        {
            // Zaloguj błąd
            Console.WriteLine($"Error retrieving refresh token: {ex.Message}");
            throw; // Opcjonalnie: ponownie rzuć wyjątek lub zwróć null
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